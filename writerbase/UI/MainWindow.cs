using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;

namespace writerbase.UI;

public class MainWindow : Window
{
    private readonly ProjectManager _projectManager;
    private readonly ExportService _exportService;
    private Label _statusLabel = null!;
    private Label _projectInfoLabel = null!;
    private ListView _projectListView = null!;
    
    public MainWindow(ProjectManager projectManager)
    {
        _projectManager = projectManager;
        _exportService = new ExportService();
        
        Title = "writerbase - Terminal Writing Application";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();
        
        // Apply white-on-black color scheme
        ColorScheme = new Terminal.Gui.ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black)
        };
        
        InitializeUI();
        RefreshProjectList();
        
        // Set focus to this window to ensure keyboard shortcuts work
        SetFocus();
    }
    
    // Initialize the main user interface components
    private void InitializeUI()
    {
        // Header
        var headerLabel = new Label("writerbase - Terminal Writing Application")
        {
            X = Pos.Center(),
            Y = 0,
            Width = 30,
            Height = 1
        };
        Add(headerLabel);
        
        // Project list
        var projectFrame = new FrameView("Projects")
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 6
        };
        
        _projectListView = new ListView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        _projectListView.SelectedItemChanged += OnProjectSelected;
        projectFrame.Add(_projectListView);
        Add(projectFrame);
        
        // Project info
        _projectInfoLabel = new Label("No project selected")
        {
            X = 0,
            Y = Pos.Bottom(projectFrame),
            Width = Dim.Fill(),
            Height = 1
        };
        _projectInfoLabel.ColorScheme = ColorScheme;
        Add(_projectInfoLabel);
        
        // Buttons
        var buttonFrame = new FrameView("Actions")
        {
            X = 0,
            Y = Pos.Bottom(_projectInfoLabel),
            Width = Dim.Fill(),
            Height = 3
        };
        
        var newProjectButton = new Button("F1 - New Project")
        {
            X = 0,
            Y = 0
        };
        newProjectButton.Clicked += OnNewProject;
        
        var openProjectButton = new Button("F2 - Open Project") 
        {
            X = Pos.Right(newProjectButton) + 1,
            Y = 0
        };
        openProjectButton.Clicked += OnOpenProject;
        
        // Settings button removed - no functionality implemented
        
        var helpButton = new Button("F3 - Help")
        {
            X = Pos.Right(openProjectButton) + 1,
            Y = 0
        };
        helpButton.Clicked += OnHelp;
        
        var exportButton = new Button("F4 - Export Project")
        {
            X = Pos.Right(helpButton) + 1,
            Y = 0
        };
        exportButton.Clicked += OnExportProject;
        
        var exitButton = new Button("Esc - Quit")
        {
            X = Pos.Right(exportButton) + 1,
            Y = 0
        };
        exitButton.Clicked += OnExit;
        
        buttonFrame.Add(newProjectButton, openProjectButton, helpButton, exportButton, exitButton);
        Add(buttonFrame);
        
        // Status bar
        _statusLabel = new Label("Ready")
        {
            X = 0,
            Y = Pos.Bottom(buttonFrame),
            Width = Dim.Fill(),
            Height = 1
        };
        _statusLabel.ColorScheme = ColorScheme;
        Add(_statusLabel);
        
        // No keyboard shortcuts - mouse-only interface
        
        // Apply color scheme to all child components
        ApplyColorSchemeToChildren();
    }
    
    // Apply color scheme to all child UI elements
    private void ApplyColorSchemeToChildren()
    {
        // Apply the same color scheme to all child components
        var colorScheme = new Terminal.Gui.ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black)
        };
        
        // Apply to all child views
        foreach (var child in Subviews)
        {
            child.ColorScheme = colorScheme;
        }
    }
    
    // Refresh the project list display from the data source
    private void RefreshProjectList()
    {
        var projects = _projectManager.GetProjectList();
        
        if (projects.Count == 0)
        {
            _projectListView.SetSource(new List<string> { "No projects found" });
            _projectInfoLabel.Text = "No projects available";
        }
        else
        {
            _projectListView.SetSource(projects);
            _statusLabel.Text = $"Project count:{projects.Count}";
        }
    }
    
    // Handle project selection change in the list view
    private void OnProjectSelected(ListViewItemEventArgs args)
    {
        if (args.Item < 0) return;
        
        var projects = _projectManager.GetProjectList();
        if (args.Item < projects.Count)
        {
            var projectName = projects[args.Item];
            var project = _projectManager.LoadProject(projectName);
            
            if (project != null)
            {
                _projectInfoLabel.Text = $"Project: {project.Title} | Word Count: {project.TotalWordCount} | Chapters: {project.Chapters.Count}";
                _statusLabel.Text = $"Loaded project: {project.Title}";
            }
        }
    }
    
    // Handle new project creation dialog and logic
    private void OnNewProject()
    {
        var dialog = new Dialog("New Project")
        {
            Width = 60,
            Height = 10
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
        
        var descLabel = new Label("Description (optional):")
        {
            X = 0,
            Y = 3
        };
        
        var descField = new TextField("")
        {
            X = 0,
            Y = 4,
            Width = Dim.Fill() - 2
        };
        
        var okButton = new Button("Create")
        {
            X = 0,
            Y = 6
        };
        okButton.Clicked += () =>
        {
            var title = titleField.Text.ToString();
            var description = descField.Text.ToString() ?? string.Empty;
            
            if (!string.IsNullOrWhiteSpace(title))
            {
                try
                {
                    _projectManager.CreateProject(title, description);
                    RefreshProjectList();
                    _statusLabel.Text = $"Created new project: {title}";
                    dialog.Running = false;
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to create project: {ex.Message}", "OK");
                }
            }
        };
        
        var cancelButton = new Button("Cancel")
        {
            X = Pos.Right(okButton) + 1,
            Y = 6
        };
        cancelButton.Clicked += () => dialog.Running = false;
        
        dialog.Add(titleLabel, titleField, descLabel, descField, okButton, cancelButton);
        Application.Run(dialog);
    }
    

    
    // Handle opening the selected project in the chapter manager
    private void OnOpenProject()
    {
        if (_projectManager.CurrentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project selected", "OK");
            return;
        }
        
        var openProjectWindow = new OpenProjectWindow(_projectManager);
        Application.Run(openProjectWindow);
        RefreshProjectList();
    }
    
    // OnSettings method removed - no functionality implemented
    
    // Handle application exit with confirmation dialog
    private void OnExit()
    {
        var result = MessageBox.Query("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (result == 0)
        {
            Application.RequestStop();
        }
    }
    
    // Display help information dialog
    private void OnHelp()
    {
        var helpText = "Help information will be displayed here.";
        
        var helpDialog = new Dialog("Mouse-Only Interface Help")
        {
            Width = 70,
            Height = 20
        };
        
        var helpTextView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2,
            Text = helpText,
            ReadOnly = true
        };
        
        var closeButton = new Button("Close")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(helpTextView)
        };
        closeButton.Clicked += () => helpDialog.Running = false;
        
        helpDialog.Add(helpTextView, closeButton);
        Application.Run(helpDialog);
    }
    
    // Handle project export functionality
    private void OnExportProject()
    {
        if (_projectManager.CurrentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project selected", "OK");
            return;
        }
        
        var exportDialog = new Dialog("Export Project")
        {
            Width = 50,
            Height = 8
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
            ExportToDocx();
        };
        
        var cancelButton = new Button("Cancel")
        {
            X = Pos.Center(),
            Y = 5
        };
        cancelButton.Clicked += () => exportDialog.Running = false;
        
        exportDialog.Add(formatLabel, docxButton, cancelButton);
        Application.Run(exportDialog);
    }
    
    private void ExportToDocx()
    {
        try
        {
            var project = _projectManager.CurrentProject;
            if (project == null)
            {
                MessageBox.ErrorQuery("Error", "No project to export", "OK");
                return;
            }
            
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
                            var overwriteResult = MessageBox.Query("File Exists", 
                                $"File '{filePath}' already exists. Do you want to overwrite it?", 
                                "Yes", "No");
                            
                            if (overwriteResult == 0) // User clicked "Yes"
                            {
                                _exportService.ExportToDocx(project, filePath);
                                saveDialog.Running = false;
                                MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                                _statusLabel.Text = $"Exported to {filePath}";
                            }
                            // If user clicked "No", do nothing and keep dialog open
                        }
                        else
                        {
                            // File doesn't exist, proceed with export
                            _exportService.ExportToDocx(project, filePath);
                            saveDialog.Running = false;
                            MessageBox.Query("Success", $"Project exported to {filePath}", "OK");
                            _statusLabel.Text = $"Exported to {filePath}";
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
    

    
    // AddEscKeyFunctionality method removed - mouse-only interface
    
    // Handle keyboard input and shortcuts for the main window
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        // Debug: Always show when any key is pressed
        _projectInfoLabel.Text = $"Key pressed: {keyEvent.Key}";
        
        // Handle arrow key navigation for project list
        if (keyEvent.Key == Key.CursorUp)
        {
            _projectInfoLabel.Text = "UP ARROW PRESSED!";
            var projects = _projectManager.GetProjectList();
            if (projects.Count > 0)
            {
                var currentIndex = _projectListView.SelectedItem;
                // Move up only if not at the top
                if (currentIndex > 0)
                {
                    var newIndex = currentIndex - 1;
                    _projectListView.SelectedItem = newIndex;
                    
                    // Debug output
                    _projectInfoLabel.Text = $"Up: current={currentIndex}, new={newIndex}, total={projects.Count}";
                    
                    // Update project info display
                    OnProjectSelected(new ListViewItemEventArgs(newIndex, projects[newIndex]));
                }
                return true; // Consume the key event
            }
        }
        else if (keyEvent.Key == Key.CursorDown)
        {
            _projectInfoLabel.Text = "DOWN ARROW PRESSED!";
            var projects = _projectManager.GetProjectList();
            if (projects.Count > 0)
            {
                var currentIndex = _projectListView.SelectedItem;
                // Move down only if not at the bottom
                if (currentIndex < projects.Count - 1)
                {
                    var newIndex = currentIndex + 1;
                    _projectListView.SelectedItem = newIndex;
                    
                    // Debug output
                    _projectInfoLabel.Text = $"Down: current={currentIndex}, new={newIndex}, total={projects.Count}";
                    
                    // Update project info display
                    OnProjectSelected(new ListViewItemEventArgs(newIndex, projects[newIndex]));
                }
                return true; // Consume the key event
            }
        }
        

        

        
        // Handle keyboard shortcuts with function keys
        else if (keyEvent.Key == Key.F1)
        {
            _projectInfoLabel.Text = "Shortcut: F1 - New Project";
            OnNewProject();
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F2)
        {
            _projectInfoLabel.Text = "Shortcut: F2 - Open Project";
            OnOpenProject();
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F3)
        {
            _projectInfoLabel.Text = "Shortcut: F3 - Help";
            OnHelp();
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.F4)
        {
            _projectInfoLabel.Text = "Shortcut: F4 - Export Project";
            OnExportProject();
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.Esc)
        {
            _projectInfoLabel.Text = "Shortcut: Esc - Quit";
            OnExit();
            return true; // Consume the key event
        }
        else if (keyEvent.Key == Key.Enter)
        {
            _projectInfoLabel.Text = "Enter key - Opening selected project";
            OnOpenProject();
            return true; // Consume the key event
        }
        
        // Let other keys be processed normally
        return base.ProcessKey(keyEvent);
    }
}
