using Terminal.Gui;

namespace writerbase.UI.MainWindowFunctions;

/// <summary>
/// Handles utility functions for the main window.
/// </summary>
public static class UtilityFunctions
{
    /// <summary>
    /// Shows the help dialog with application information and keyboard shortcuts.
    /// </summary>
    /// <param name="statusLabel">Status label for user feedback</param>
    public static void OnHelp(Label statusLabel)
    {
        try
        {
            var helpDialog = new Dialog("Help")
            {
                Width = 80,
                Height = 20
            };

            var helpText = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill() - 2,
                Height = Dim.Fill() - 3,
                ReadOnly = true,
                Text = @"WriterBase - Terminal Writing Application

KEYBOARD SHORTCUTS:
F1 - New Project
F2 - Open Project  
F3 - Help
F4 - Export Project
Esc - Quit

ARROW KEYS:
Use Up/Down arrows to navigate through projects

MOUSE NAVIGATION:
- Click on projects to select them
- Use buttons for all actions
- Right-click context menus available

PROJECT MANAGEMENT:
- Create new projects with title and description
- Open projects to manage chapters
- Export projects to DOCX or ODT formats

EXPORT FORMATS:
DOCX (Microsoft Word):
  - Novel: Title page, TOC, chapter headings
  - Short Story: Title page, continuous text

ODT (OpenDocument):
  - Novel: Title page, TOC, chapter headings  
  - Short Story: Title page, continuous text

All exports use Courier font and preserve line breaks from the terminal editor.

For more information, visit the project repository."
            };

            var closeButton = new Button("Close")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(helpText) + 1
            };
            closeButton.Clicked += () => helpDialog.Running = false;

            helpDialog.Add(helpText, closeButton);
            Application.Run(helpDialog);
            statusLabel.Text = "Help dialog closed";
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to show help: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Handles application exit with confirmation dialog.
    /// </summary>
    /// <param name="statusLabel">Status label for user feedback</param>
    public static void OnExit(Label statusLabel)
    {
        try
        {
            var exitResult = MessageBox.Query("Exit", "Are you sure you want to exit?", "Yes", "No");
            if (exitResult == 0) // User clicked "Yes"
            {
                statusLabel.Text = "Exiting application...";
                Application.RequestStop();
            }
            else
            {
                statusLabel.Text = "Exit cancelled";
            }
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to exit: {ex.Message}", "OK");
        }
    }
}
