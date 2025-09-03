using writerbase.Models;
using Newtonsoft.Json;
using System.Security;

namespace writerbase.Services;

/// <summary>
/// Service class responsible for managing writing projects.
/// This class handles all project-related operations including:
/// - Creating new projects
/// - Loading existing projects
/// - Saving projects to disk
/// - Managing chapters within projects
/// 
/// This demonstrates the Service Layer pattern in our architecture:
/// - Separates business logic from UI components
/// - Handles data persistence (JSON files)
/// - Provides a clean interface for project operations
/// - Manages application state (current project)
/// </summary>
public class ProjectManager
{
    /// <summary>
    /// Directory where all project files are stored.
    /// Uses the user's profile directory for cross-platform compatibility.
    /// </summary>
    private readonly string _projectsDirectory;
    
    /// <summary>
    /// Currently loaded project. Can be null if no project is loaded.
    /// This maintains application state across different UI screens.
    /// </summary>
    private Project? _currentProject;
    
    /// <summary>
    /// Initializes the ProjectManager and sets up the projects directory.
    /// Creates the directory if it doesn't exist.
    /// </summary>
    // Initialize the ProjectManager and set up the projects directory
    public ProjectManager()
    {
        // Get the user's profile directory (e.g., C:\Users\Username on Windows)
        // Combine it with "writerbase\Projects" to create our project storage location
        _projectsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "writerbase", "Projects");
        
        // Create the directory if it doesn't exist
        // This ensures the app works on first run
        Directory.CreateDirectory(_projectsDirectory);
    }
    
    /// <summary>
    /// Gets the currently loaded project.
    /// Returns null if no project is currently loaded.
    /// </summary>
    public Project? CurrentProject => _currentProject;
    
    /// <summary>
    /// Gets a list of all available project names.
    /// Reads the projects directory and returns filenames without the .json extension.
    /// </summary>
    /// <returns>List of project names that can be loaded</returns>
    // Get a list of all available project names
    public List<string> GetProjectList()
    {
        // Find all .json files in the projects directory
        var projectFiles = Directory.GetFiles(_projectsDirectory, "*.json");
        
        // Extract just the project names (remove .json extension)
        return projectFiles.Select(Path.GetFileNameWithoutExtension).Where(name => name != null).Cast<string>().ToList();
    }
    
    /// <summary>
    /// Creates a new project with the specified title and description.
    /// The project is automatically saved to disk and set as the current project.
    /// </summary>
    /// <param name="title">Project title (required)</param>
    /// <param name="description">Optional project description</param>
    /// <returns>The newly created project</returns>
    // Create a new project with the specified title and description
    public Project CreateProject(string title, string description = "")
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty", nameof(title));
            
        if (title.Length > 100)
            throw new ArgumentException("Project title cannot exceed 100 characters", nameof(title));
            
        if (description?.Length > 1000)
            throw new ArgumentException("Project description cannot exceed 1000 characters", nameof(description));
            
        // Sanitize title for use as filename
        var sanitizedTitle = SanitizeFileName(title);
        if (string.IsNullOrWhiteSpace(sanitizedTitle))
            throw new ArgumentException("Project title contains invalid characters", nameof(title));
        
        // Create a new Project object with the provided information
        var project = new Project
        {
            Title = sanitizedTitle,
            Description = description ?? ""
        };
        
        // Set this as the current project
        _currentProject = project;
        
        // Save it to disk immediately
        SaveProject(project);
        
        return project;
    }
    
    /// <summary>
    /// Loads a project from disk by name.
    /// The project becomes the current project if successfully loaded.
    /// </summary>
    /// <param name="projectName">Name of the project to load</param>
    /// <returns>The loaded project, or null if not found</returns>
    // Load a project from disk by name
    public Project? LoadProject(string projectName)
    {
        // Validate input to prevent path traversal attacks
        if (string.IsNullOrWhiteSpace(projectName))
            return null;
            
        // Sanitize project name to prevent path traversal
        var sanitizedName = SanitizeFileName(projectName);
        if (string.IsNullOrWhiteSpace(sanitizedName))
            return null;
            
        // Construct the full file path for this project
        var filePath = Path.Combine(_projectsDirectory, $"{sanitizedName}.json");
        
        // Additional security check: ensure the path is within our projects directory
        var fullPath = Path.GetFullPath(filePath);
        var projectsDirFullPath = Path.GetFullPath(_projectsDirectory);
        
        if (!fullPath.StartsWith(projectsDirFullPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityException("Invalid project path detected");
        }
        
        // Check if the file exists
        if (!File.Exists(filePath))
            return null;
            
        try
        {
            // Read the JSON file content
            var json = File.ReadAllText(filePath);
            
            // Deserialize JSON back into a Project object
            // Newtonsoft.Json handles the conversion from JSON to C# object
            var project = JsonConvert.DeserializeObject<Project>(json);
            
            // Set as current project
            _currentProject = project;
            
            return project;
        }
        catch (Exception ex)
        {
            // If anything goes wrong during loading, throw a meaningful error
            throw new InvalidOperationException($"Failed to load project: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Saves a project to disk as a JSON file.
    /// If no project is specified, saves the current project.
    /// </summary>
    /// <param name="project">Project to save, or null to save current project</param>
    // Save a project to disk as a JSON file
    public void SaveProject(Project? project = null)
    {
        // Use the specified project or fall back to current project
        var projectToSave = project ?? _currentProject;
        
        // Validate that we have a project to save
        if (projectToSave == null)
            throw new InvalidOperationException("No project to save");
            
        // Construct the file path using the project title (already sanitized)
        var filePath = Path.Combine(_projectsDirectory, $"{projectToSave.Title}.json");
        
        // Additional security check: ensure the path is within our projects directory
        var fullPath = Path.GetFullPath(filePath);
        var projectsDirFullPath = Path.GetFullPath(_projectsDirectory);
        
        if (!fullPath.StartsWith(projectsDirFullPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityException("Invalid project path detected");
        }
        
        // Update the last modified timestamp
        projectToSave.LastModified = DateTime.Now;
        
        try
        {
            // Serialize the project object to JSON
            // Formatting.Indented makes the JSON file human-readable
            var json = JsonConvert.SerializeObject(projectToSave, Formatting.Indented);
            
            // Write the JSON to the file
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            // If saving fails, throw a meaningful error
            throw new InvalidOperationException($"Failed to save project: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Adds a new chapter to the current project.
    /// The chapter is automatically saved with the project.
    /// </summary>
    /// <param name="title">Chapter title</param>
    /// <param name="order">Chapter order (optional, auto-assigned if not specified)</param>
    /// <returns>The newly created chapter</returns>
    public Chapter AddChapter(string title, int order = -1)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Chapter title cannot be empty", nameof(title));
            
        if (title.Length > 200)
            throw new ArgumentException("Chapter title cannot exceed 200 characters", nameof(title));
            
        // Validate that we have a current project
        if (_currentProject == null)
            throw new InvalidOperationException("No current project");
            
        // Create a new chapter
        var chapter = new Chapter
        {
            Title = title.Trim(),
            // If order is specified, use it; otherwise, add to the end
            Order = order >= 0 ? order : _currentProject.Chapters.Count + 1
        };
        
        // Add the chapter to the project
        _currentProject.Chapters.Add(chapter);
        
        // Save the project to persist the new chapter
        SaveProject();
        
        return chapter;
    }
    
    /// <summary>
    /// Sanitizes a filename to prevent path traversal and invalid characters.
    /// </summary>
    /// <param name="fileName">The filename to sanitize</param>
    /// <returns>A safe filename, or empty string if invalid</returns>
    // Sanitize filename to prevent path traversal and invalid characters
    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;
            
        // Remove or replace invalid filename characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = fileName;
        
        foreach (var invalidChar in invalidChars)
        {
            sanitized = sanitized.Replace(invalidChar, '_');
        }
        
        // Remove path traversal attempts
        sanitized = sanitized.Replace("..", "_");
        sanitized = sanitized.Replace("\\", "_");
        sanitized = sanitized.Replace("/", "_");
        
        // Trim whitespace and limit length
        sanitized = sanitized.Trim();
        
        // Ensure it's not empty after sanitization
        if (string.IsNullOrWhiteSpace(sanitized))
            return string.Empty;
            
        // Limit length to prevent issues
        if (sanitized.Length > 50)
            sanitized = sanitized.Substring(0, 50);
            
        return sanitized;
    }
    
    /// <summary>
    /// Updates an existing chapter's title and content.
    /// The changes are automatically saved.
    /// </summary>
    /// <param name="chapterId">ID of the chapter to update</param>
    /// <param name="title">New chapter title</param>
    /// <param name="content">New chapter content</param>
    // Update an existing chapter's title and content
    public void UpdateChapter(string chapterId, string title, string content)
    {
        // Validate that we have a current project
        if (_currentProject == null)
            throw new InvalidOperationException("No current project");
            
        // Find the chapter by its ID
        var chapter = _currentProject.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
            throw new InvalidOperationException("Chapter not found");
            
        // Update the chapter properties
        chapter.Title = title;
        chapter.Content = content;
        chapter.LastModified = DateTime.Now;
        
        // Save the project to persist the changes
        SaveProject();
    }
    
    /// <summary>
    /// Deletes a chapter from the current project.
    /// The deletion is automatically saved.
    /// </summary>
    /// <param name="chapterId">ID of the chapter to delete</param>
    // Delete a chapter from the current project
    public void DeleteChapter(string chapterId)
    {
        // Validate that we have a current project
        if (_currentProject == null)
            throw new InvalidOperationException("No current project");
            
        // Find the chapter by its ID
        var chapter = _currentProject.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
            throw new InvalidOperationException("Chapter not found");
            
        // Remove the chapter from the project
        _currentProject.Chapters.Remove(chapter);
        
        // Save the project to persist the deletion
        SaveProject();
    }
    
    /// <summary>
    /// Reorders chapters in the current project.
    /// The new order is automatically saved.
    /// </summary>
    /// <param name="chapterIds">List of chapter IDs in the desired order</param>
    // Reorder chapters in the current project
    public void ReorderChapters(List<string> chapterIds)
    {
        // Validate that we have a current project
        if (_currentProject == null)
            throw new InvalidOperationException("No current project");
            
        // Update the order of each chapter based on its position in the list
        for (int i = 0; i < chapterIds.Count; i++)
        {
            var chapter = _currentProject.Chapters.FirstOrDefault(c => c.Id == chapterIds[i]);
            if (chapter != null)
            {
                // Set the order to 1-based index (first chapter = 1, second = 2, etc.)
                chapter.Order = i + 1;
            }
        }
        
        // Save the project to persist the new order
        SaveProject();
    }
}
