using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;

namespace writerbase.UI.MainWindowFunctions;

/// <summary>
/// Handles project export functionality for the main window.
/// </summary>
public static class ExportProjectFunctions
{
    /// <summary>
    /// Shows the main export dialog with format selection options.
    /// </summary>
    /// <param name="projectManager">The project manager service</param>
    /// <param name="exportService">The export service</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    public static void OnExportProject(ProjectManager projectManager, ExportService exportService, Label statusLabel)
    {
        if (projectManager.CurrentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project selected", "OK");
            return;
        }

        var exportDialog = new Dialog("Export Project")
        {
            Width = 50,
            Height = 10
        };

        var formatLabel = new Label("Select export format:")
        {
            X = 0,
            Y = 0
        };

        var docxButton = new Button("DOCX")
        {
            X = 0,
            Y = 2
        };
        docxButton.Clicked += () => {
            exportDialog.Running = false;
            ExportToDocx(projectManager.CurrentProject, exportService, statusLabel);
        };

        var odtButton = new Button("ODT")
        {
            X = 0,
            Y = 4
        };
        odtButton.Clicked += () => {
            exportDialog.Running = false;
            ExportToOdt(projectManager.CurrentProject, exportService, statusLabel);
        };

        var cancelButton = new Button("Cancel")
        {
            X = Pos.Center(),
            Y = 7
        };
        cancelButton.Clicked += () => exportDialog.Running = false;

        exportDialog.Add(formatLabel, docxButton, odtButton, cancelButton);
        Application.Run(exportDialog);
    }

    /// <summary>
    /// Shows the DOCX format selection dialog.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="exportService">The export service</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    private static void ExportToDocx(Project project, ExportService exportService, Label statusLabel)
    {
        try
        {
            // Show DOCX format selection dialog
            var formatDialog = new Dialog("Select DOCX Format")
            {
                Width = 50,
                Height = 10
            };

            var formatLabel = new Label("Choose export format:")
            {
                X = 0,
                Y = 0
            };

            var novelButton = new Button("Novel")
            {
                X = 0,
                Y = 2,
                Width = 20
            };
            novelButton.Clicked += () =>
            {
                formatDialog.Running = false;
                ExportToDocxWithFormat(project, exportService, true, statusLabel);
            };

            var shortStoryButton = new Button("Short Story")
            {
                X = Pos.Right(novelButton) + 2,
                Y = 2,
                Width = 20
            };
            shortStoryButton.Clicked += () =>
            {
                formatDialog.Running = false;
                ExportToDocxWithFormat(project, exportService, false, statusLabel);
            };

            var cancelButton = new Button("Cancel")
            {
                X = Pos.Center(),
                Y = 4
            };
            cancelButton.Clicked += () => formatDialog.Running = false;

            formatDialog.Add(formatLabel, novelButton, shortStoryButton, cancelButton);
            Application.Run(formatDialog);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Export failed: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Shows the ODT format selection dialog.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="exportService">The export service</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    private static void ExportToOdt(Project project, ExportService exportService, Label statusLabel)
    {
        try
        {
            // Show ODT format selection dialog
            var formatDialog = new Dialog("Select ODT Format")
            {
                Width = 50,
                Height = 10
            };

            var formatLabel = new Label("Choose export format:")
            {
                X = 0,
                Y = 0
            };

            var novelButton = new Button("Novel")
            {
                X = 0,
                Y = 2,
                Width = 20
            };
            novelButton.Clicked += () =>
            {
                formatDialog.Running = false;
                ExportToOdtWithFormat(project, exportService, true, statusLabel);
            };

            var shortStoryButton = new Button("Short Story")
            {
                X = Pos.Right(novelButton) + 2,
                Y = 2,
                Width = 20
            };
            shortStoryButton.Clicked += () =>
            {
                formatDialog.Running = false;
                ExportToOdtWithFormat(project, exportService, false, statusLabel);
            };

            var cancelButton = new Button("Cancel")
            {
                X = Pos.Center(),
                Y = 4
            };
            cancelButton.Clicked += () => formatDialog.Running = false;

            formatDialog.Add(formatLabel, novelButton, shortStoryButton, cancelButton);
            Application.Run(formatDialog);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Export failed: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Exports project to DOCX format with the specified style.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="exportService">The export service</param>
    /// <param name="isNovel">Whether to export in novel format</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    private static void ExportToDocxWithFormat(Project project, ExportService exportService, bool isNovel, Label statusLabel)
    {
        try
        {
            // Show file save dialog
            var saveDialog = new Dialog("Save DOCX File")
            {
                Width = 60,
                Height = 8
            };

            var pathLabel = new Label("File path:")
            {
                X = 0,
                Y = 0
            };

            var pathField = new TextField($"{project.Title}.docx")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill() - 2
            };

            var saveButton = new Button("Save")
            {
                X = 0,
                Y = 3
            };
            saveButton.Clicked += () =>
            {
                var filePath = pathField.Text.ToString();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    try
                    {
                        // Check if file already exists
                        if (File.Exists(filePath))
                        {
                            statusLabel.Text = $"DOCX: File exists, showing overwrite dialog...";
                            var overwriteResult = MessageBox.Query("File Exists", 
                                $"File '{filePath}' already exists. Do you want to overwrite it?", 
                                "Yes", "No");

                            statusLabel.Text = $"DOCX: Overwrite result: {overwriteResult}";

                            if (overwriteResult == 0) // User clicked "Yes"
                            {
                                statusLabel.Text = "DOCX: User chose to overwrite, exporting...";
                                if (isNovel)
                                {
                                    exportService.ExportToDocxNovel(project, filePath);
                                }
                                else
                                {
                                    exportService.ExportToDocxShort(project, filePath);
                                }
                                saveDialog.Running = false;
                                MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                                statusLabel.Text = $"Exported to {filePath}";
                            }
                            else
                            {
                                statusLabel.Text = "DOCX: User chose not to overwrite, keeping dialog open";
                            }
                        }
                        else
                        {
                            // File doesn't exist, proceed with export
                            if (isNovel)
                            {
                                exportService.ExportToDocxNovel(project, filePath);
                            }
                            else
                            {
                                exportService.ExportToDocxShort(project, filePath);
                            }
                            saveDialog.Running = false;
                            MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                            statusLabel.Text = $"Exported to {filePath}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Export Error", $"Failed to export: {ex.Message}", "OK");
                    }
                }
            };

            var cancelButton = new Button("Cancel")
            {
                X = Pos.Right(saveButton) + 1,
                Y = 3
            };
            cancelButton.Clicked += () => saveDialog.Running = false;

            saveDialog.Add(pathLabel, pathField, saveButton, cancelButton);
            Application.Run(saveDialog);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Export failed: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Exports project to ODT format with the specified style.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="exportService">The export service</param>
    /// <param name="isNovel">Whether to export in novel format</param>
    /// <param name="statusLabel">Status label for user feedback</param>
    private static void ExportToOdtWithFormat(Project project, ExportService exportService, bool isNovel, Label statusLabel)
    {
        try
        {
            // Show file save dialog
            var saveDialog = new Dialog("Save ODT File")
            {
                Width = 60,
                Height = 8
            };

            var pathLabel = new Label("File path:")
            {
                X = 0,
                Y = 0
            };

            var pathField = new TextField($"{project.Title}.odt")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill() - 2
            };

            var saveButton = new Button("Save")
            {
                X = 0,
                Y = 3
            };
            saveButton.Clicked += () =>
            {
                var filePath = pathField.Text.ToString();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    try
                    {
                        // Check if file already exists
                        if (File.Exists(filePath))
                        {
                            statusLabel.Text = $"ODT: File exists, showing overwrite dialog...";
                            var overwriteResult = MessageBox.Query("File Exists", 
                                $"File '{filePath}' already exists. Do you want to overwrite it?", 
                                "Yes", "No");

                            statusLabel.Text = $"ODT: Overwrite result: {overwriteResult}";

                            if (overwriteResult == 0) // User clicked "Yes"
                            {
                                statusLabel.Text = "ODT: User chose to overwrite, exporting...";
                                if (isNovel)
                                {
                                    exportService.ExportToOdtNovel(project, filePath);
                                }
                                else
                                {
                                    exportService.ExportToOdtShort(project, filePath);
                                }
                                saveDialog.Running = false;
                                MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                                statusLabel.Text = $"Exported to {filePath}";
                            }
                            else
                            {
                                statusLabel.Text = "ODT: User chose not to overwrite, keeping dialog open";
                            }
                        }
                        else
                        {
                            // File doesn't exist, proceed with export
                            if (isNovel)
                            {
                                exportService.ExportToOdtNovel(project, filePath);
                            }
                            else
                            {
                                exportService.ExportToOdtShort(project, filePath);
                            }
                            saveDialog.Running = false;
                            MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                            statusLabel.Text = $"Exported to {filePath}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ErrorQuery("Export Error", $"Failed to export: {ex.Message}", "OK");
                    }
                }
            };

            var cancelButton = new Button("Cancel")
            {
                X = Pos.Right(saveButton) + 1,
                Y = 3
            };
            cancelButton.Clicked += () => saveDialog.Running = false;

            saveDialog.Add(pathLabel, pathField, saveButton, cancelButton);
            Application.Run(saveDialog);
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Export failed: {ex.Message}", "OK");
        }
    }
}
