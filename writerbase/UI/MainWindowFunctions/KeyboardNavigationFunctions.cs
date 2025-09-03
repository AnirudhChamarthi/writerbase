using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;
using writerbase.UI.MainWindowFunctions;

namespace writerbase.UI.MainWindowFunctions;

/// <summary>
/// Handles keyboard navigation and shortcuts for the main window.
/// </summary>
public static class KeyboardNavigationFunctions
{
    /// <summary>
    /// Processes keyboard input and handles navigation shortcuts.
    /// </summary>
    /// <param name="keyEvent">The keyboard event</param>
    /// <param name="projectManager">The project manager service</param>
    /// <param name="projectListView">The project list view</param>
    /// <param name="projectInfoLabel">Project info label for display</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    /// <param name="exportService">The export service</param>
    /// <param name="refreshCallback">Callback to refresh the project list</param>
    /// <returns>True if the key event was handled, false otherwise</returns>
    public static bool ProcessKey(KeyEvent keyEvent, ProjectManager projectManager, ListView projectListView, 
        Label projectInfoLabel, Label statusLabel, ExportService exportService, Action? refreshCallback = null)
    {
        // Handle arrow key navigation for project list
        if (keyEvent.Key == Key.CursorUp)
        {
            var projects = projectManager.GetProjectList();
            if (projects.Count > 0)
            {
                var currentIndex = projectListView.SelectedItem;
                // Move up only if not at the top
                if (currentIndex > 0)
                {
                    var newIndex = currentIndex - 1;
                    projectListView.SelectedItem = newIndex;

                    // Update project info display
                    OpenProjectFunctions.OnProjectSelected(new ListViewItemEventArgs(newIndex, projects[newIndex]), projectManager, projectInfoLabel);
                }
                return true; // Consume the key event
            }
        }
        else if (keyEvent.Key == Key.CursorDown)
        {
            var projects = projectManager.GetProjectList();
            if (projects.Count > 0)
            {
                var currentIndex = projectListView.SelectedItem;
                // Move down only if not at the bottom
                if (currentIndex < projects.Count - 1)
                {
                    var newIndex = currentIndex + 1;
                    projectListView.SelectedItem = newIndex;

                    // Update project info display
                    OpenProjectFunctions.OnProjectSelected(new ListViewItemEventArgs(newIndex, projects[newIndex]), projectManager, projectInfoLabel);
                }
                return true; // Consume the key event
            }
        }

        // Handle keyboard shortcuts with function keys
        else if (keyEvent.Key == Key.F1)
        {
            statusLabel.Text = "Shortcut: F1 - New Project";
            NewProjectFunctions.OnNewProject(projectManager, statusLabel, refreshCallback);
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F2)
        {
            statusLabel.Text = "Shortcut: F2 - Open Project";
            OpenProjectFunctions.OnOpenProject(projectManager, statusLabel);
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F3)
        {
            statusLabel.Text = "Shortcut: F3 - Help";
            UtilityFunctions.OnHelp(statusLabel);
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F4)
        {
            statusLabel.Text = "Shortcut: F4 - Export Project";
            ExportProjectFunctions.OnExportProject(projectManager, exportService, statusLabel);
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.Esc)
        {
            statusLabel.Text = "Shortcut: Esc - Quit";
            UtilityFunctions.OnExit(statusLabel);
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.Enter)
        {
            statusLabel.Text = "Enter key - Opening selected project";
            OpenProjectFunctions.OnOpenProject(projectManager, statusLabel);
            return true; // Consume the key event
        }

        return false; // Key event not handled
    }
}
