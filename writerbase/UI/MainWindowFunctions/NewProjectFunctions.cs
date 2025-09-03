using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;

namespace writerbase.UI.MainWindowFunctions;

/// <summary>
/// Handles new project creation functionality for the main window.
/// </summary>
public static class NewProjectFunctions
{
    /// <summary>
    /// Creates a new project with user input for title and description.
    /// </summary>
    /// <param name="projectManager">The project manager service</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    /// <param name="refreshCallback">Callback to refresh the project list after creation</param>
    public static void OnNewProject(ProjectManager projectManager, Label statusLabel, Action? refreshCallback = null)
    {
        try
        {
            // Create input dialog
            var inputDialog = new Dialog("New Project")
            {
                Width = 60,
                Height = 12
            };

            var titleLabel = new Label("Project Title:")
            {
                X = 0,
                Y = 0
            };

            var titleField = new TextField("")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill() - 2
            };

            var descriptionLabel = new Label("Description (optional):")
            {
                X = 0,
                Y = 3
            };

            var descriptionField = new TextField("")
            {
                X = 0,
                Y = 4,
                Width = Dim.Fill() - 2,
                Height = 3
            };

            var createButton = new Button("Create Project")
            {
                X = 0,
                Y = 8
            };

            var cancelButton = new Button("Cancel")
            {
                X = Pos.Right(createButton) + 2,
                Y = 8
            };

            createButton.Clicked += () =>
            {
                var title = titleField.Text?.ToString()?.Trim() ?? "";
                var description = descriptionField.Text?.ToString()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.ErrorQuery("Error", "Project title is required", "OK");
                    return;
                }

                try
                {
                    projectManager.CreateProject(title, description);
                    inputDialog.Running = false;
                    statusLabel.Text = $"Created new project: {title}";
                    
                    // Refresh the project list
                    refreshCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to create project: {ex.Message}", "OK");
                }
            };

            cancelButton.Clicked += () => inputDialog.Running = false;

            inputDialog.Add(titleLabel, titleField, descriptionLabel, descriptionField, createButton, cancelButton);
            Application.Run(inputDialog);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to create new project: {ex.Message}", "OK");
        }
    }
}
