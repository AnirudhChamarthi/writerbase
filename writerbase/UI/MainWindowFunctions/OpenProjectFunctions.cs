using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;
using writerbase.UI;

namespace writerbase.UI.MainWindowFunctions;

/// <summary>
/// Handles opening projects and project selection functionality for the main window.
/// </summary>
public static class OpenProjectFunctions
{
    /// <summary>
    /// Opens the project management window for the currently selected project.
    /// </summary>
    /// <param name="projectManager">The project manager service</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    public static void OnOpenProject(ProjectManager projectManager, Label statusLabel)
    {
        try
        {
            if (projectManager.CurrentProject == null)
            {
                MessageBox.ErrorQuery("Error", "No project selected", "OK");
                return;
            }

            var openProjectWindow = new OpenProjectWindow(projectManager);
            Application.Run(openProjectWindow);
            statusLabel.Text = $"Opened project: {projectManager.CurrentProject.Title}";
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to open project: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles project selection from the project list.
    /// </summary>
    /// <param name="args">ListView item event arguments</param>
    /// <param name="projectManager">The project manager service</param>
    /// <param name="projectInfoLabel">Project info label for display</param>
    public static void OnProjectSelected(ListViewItemEventArgs args, ProjectManager projectManager, Label projectInfoLabel)
    {
        try
        {
            var projectNames = projectManager.GetProjectList();
            if (args.Item >= 0 && args.Item < projectNames.Count)
            {
                var selectedProjectName = projectNames[args.Item];
                var selectedProject = projectManager.LoadProject(selectedProjectName);
                
                if (selectedProject != null)
                {
                    // Display project info in a compact single line format
                    var chapterCount = selectedProject.Chapters?.Count ?? 0;
                    var totalWords = selectedProject.Chapters?.Sum(c => c.Content?.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length ?? 0) ?? 0;

                    projectInfoLabel.Text = $"Project: {selectedProject.Title} | Word Count: {totalWords} | Chapters: {chapterCount}";
                }
                else
                {
                    projectInfoLabel.Text = $"Failed to load project: {selectedProjectName}";
                }
            }
        }
        catch (Exception ex)
        {
            projectInfoLabel.Text = $"Error loading project info: {ex.Message}";
        }
    }
}
