using System.ComponentModel.DataAnnotations;

namespace writerbase.Models;

/// <summary>
/// Represents a writing project (novel, short story, etc.).
/// This is the root entity that contains all chapters and project metadata.
/// 
/// This class demonstrates several OOP principles:
/// - Encapsulation: Data and behavior are bundled together
/// - Properties: Clean way to access and modify data
/// - Computed properties: TotalWordCount and TotalCharacterCount are calculated
/// - Data validation: Required attribute ensures Title is not empty
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for this project.
    /// Generated automatically using Guid.NewGuid() for uniqueness.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The title of the writing project (e.g., "My Novel", "Short Story Collection").
    /// This field is required and cannot be empty.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description of the project.
    /// Can contain plot summary, notes, or any additional information.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// When this project was first created.
    /// Automatically set to current date/time when project is instantiated.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// When this project was last modified.
    /// Updated whenever the project is saved.
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Collection of all chapters in this project.
    /// Uses List<Chapter> for easy addition, removal, and ordering.
    /// </summary>
    public List<Chapter> Chapters { get; set; } = new();
    

    
    /// <summary>
    /// Computed property that calculates the total word count across all chapters.
    /// Uses LINQ Sum() method to aggregate word counts from all chapters.
    /// This is a "computed property" - it's calculated on-demand, not stored.
    /// </summary>
    public int TotalWordCount => Chapters.Sum(c => c.WordCount);
    
    /// <summary>
    /// Computed property that calculates the total character count across all chapters.
    /// Similar to TotalWordCount but for characters instead of words.
    /// </summary>
    public int TotalCharacterCount => Chapters.Sum(c => c.CharacterCount);
}
