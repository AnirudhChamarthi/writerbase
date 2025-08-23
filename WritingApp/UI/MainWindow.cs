using Terminal.Gui;
using WritingApp.Models;
using WritingApp.Services;

namespace WritingApp.UI;

public class MainWindow : Window
{
    private readonly ProjectManager _projectManager;
    private Label _statusLabel;
    private Label _projectInfoLabel;
    private ListView _projectListView;
    
    public MainWindow(ProjectManager projectManager)
    {
        _projectManager = projectManager;
        
        Title = "Terminal Writing Application";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();
        
        // Apply white-on-black color scheme
        ColorScheme = new Terminal.Gui.ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.White)
        };
        
        InitializeUI();
        RefreshProjectList();
    }
    
    private void InitializeUI()
    {
        // Header
        var headerLabel = new Label("Terminal Writing Application")
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
        Add(_projectInfoLabel);
        
        // Buttons
        var buttonFrame = new FrameView("Actions")
        {
            X = 0,
            Y = Pos.Bottom(_projectInfoLabel),
            Width = Dim.Fill(),
            Height = 3
        };
        
        var newProjectButton = new Button("[N]ew Project")
        {
            X = 0,
            Y = 0
        };
        newProjectButton.Clicked += OnNewProject;
        
        var openProjectButton = new Button("[O]pen Project")
        {
            X = Pos.Right(newProjectButton) + 1,
            Y = 0
        };
        openProjectButton.Clicked += OnOpenProject;
        
        var chapterManagerButton = new Button("[C]hapter Manager")
        {
            X = Pos.Right(openProjectButton) + 1,
            Y = 0
        };
        chapterManagerButton.Clicked += OnChapterManager;
        
        var settingsButton = new Button("[S]ettings")
        {
            X = Pos.Right(chapterManagerButton) + 1,
            Y = 0
        };
        settingsButton.Clicked += OnSettings;
        
        var helpButton = new Button("[H]elp")
        {
            X = Pos.Right(settingsButton) + 1,
            Y = 0
        };
        helpButton.Clicked += OnHelp;
        
        var exitButton = new Button("[Q]uit")
        {
            X = Pos.Right(helpButton) + 1,
            Y = 0
        };
        exitButton.Clicked += OnExit;
        
        buttonFrame.Add(newProjectButton, openProjectButton, chapterManagerButton, settingsButton, helpButton, exitButton);
        Add(buttonFrame);
        
        // Status bar
        _statusLabel = new Label("Ready")
        {
            X = 0,
            Y = Pos.Bottom(buttonFrame),
            Width = Dim.Fill(),
            Height = 1
        };
        Add(_statusLabel);
        
        // Add keyboard shortcuts
        AddKeyboardShortcuts();
        
        // Apply color scheme to all child components
        ApplyColorSchemeToChildren();
    }
    
    private void ApplyColorSchemeToChildren()
    {
        // Apply the same color scheme to all child components
        var colorScheme = new Terminal.Gui.ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.White)
        };
        
        // Apply to all child views
        foreach (var child in Subviews)
        {
            child.ColorScheme = colorScheme;
        }
    }
    
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
            var description = descField.Text.ToString();
            
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
    
    private void OnOpenProject()
    {
        if (_projectManager.CurrentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project selected", "OK");
            return;
        }
        
        var project = _projectManager.CurrentProject;
        if (project.Chapters.Count == 0)
        {
            var result = MessageBox.Query("No Chapters", 
                "This project has no chapters. Would you like to create one?", 
                "Yes", "No");
                
            if (result == 0)
            {
                OnChapterManager();
            }
        }
        else
        {
            // Open the first chapter for editing
            var firstChapter = project.Chapters.OrderBy(c => c.Order).First();
            var editor = new ChapterEditorWindow(_projectManager, firstChapter);
            Application.Run(editor);
        }
    }
    
    private void OnChapterManager()
    {
        if (_projectManager.CurrentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project selected", "OK");
            return;
        }
        
        var chapterManager = new ChapterManagerWindow(_projectManager);
        Application.Run(chapterManager);
        RefreshProjectList();
    }
    
    private void OnSettings()
    {
        var settingsText = @"
Settings:
- Auto-save: Enabled
- Word count display: Enabled
- Character count display: Enabled
- Theme: Dark

Settings can be configured in the project file.
        ";
        
        var settingsDialog = new Dialog("Settings")
        {
            Width = 60,
            Height = 15
        };
        
        var settingsTextView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2,
            Text = settingsText,
            ReadOnly = true
        };
        
        var closeButton = new Button("Close")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(settingsTextView)
        };
        closeButton.Clicked += () => settingsDialog.Running = false;
        
        settingsDialog.Add(settingsTextView, closeButton);
        Application.Run(settingsDialog);
    }
    
    private void OnExit()
    {
        var result = MessageBox.Query("Exit", "Are you sure you want to exit?", "Yes", "No");
        if (result == 0)
        {
            Application.RequestStop();
        }
    }
    
    private void OnHelp()
    {
                 var helpText = @"
 KEYBOARD SHORTCUTS:
 ===================
 
 Main Window:
 - N/n: New Project
 - O/o: Open Project  
 - C/c: Chapter Manager
 - S/s: Settings
 - H/h: Help (this screen)
 - Q/q: Quit
 
 Chapter Manager:
 - A/a: Add Chapter
 - E/e: Edit Chapter
 - D/d: Delete Chapter
 - C/c: Close
 
 Chapter Editor:
 - Ctrl+S: Save
 - Esc: Exit
 
 Navigation:
 - Tab: Move between controls
 - Enter: Activate focused control
 - Arrow keys: Navigate lists
 
 Both mouse clicks and keyboard shortcuts work!
 Both uppercase and lowercase letters work!
         ";
        
        var helpDialog = new Dialog("Keyboard Shortcuts")
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
    
    private void AddKeyboardShortcuts()
    {
        // Global keyboard event handler for main window
        Application.RootKeyEvent += (e) =>
        {
            // Only handle single key presses (not combinations)
            if (e.IsCtrl || e.IsAlt || e.IsShift) return false;
            
            // Convert to uppercase for case-insensitive comparison
            var keyChar = char.ToUpper((char)e.Key);
            
            switch (keyChar)
            {
                case 'N':
                    OnNewProject();
                    return true;
                    
                case 'O':
                    OnOpenProject();
                    return true;
                    
                case 'C':
                    OnChapterManager();
                    return true;
                    
                case 'S':
                    OnSettings();
                    return true;
                    
                case 'H':
                    OnHelp();
                    return true;
                    
                case 'Q':
                    OnExit();
                    return true;
                    
                default:
                    return false;
            }
        };
    }
}
