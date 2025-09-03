using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;
using writerbase.UI.MainWindowFunctions;

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
        
        _projectListView.SelectedItemChanged += (args) => OpenProjectFunctions.OnProjectSelected(args, _projectManager, _projectInfoLabel);
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
        newProjectButton.Clicked += () => NewProjectFunctions.OnNewProject(_projectManager, _statusLabel, RefreshProjectList);
        
        var openProjectButton = new Button("F2 - Open Project") 
        {
            X = Pos.Right(newProjectButton) + 1,
            Y = 0
        };
        openProjectButton.Clicked += () => OpenProjectFunctions.OnOpenProject(_projectManager, _statusLabel);

        var helpButton = new Button("F3 - Help")
        {
            X = Pos.Right(openProjectButton) + 1,
            Y = 0
        };
        helpButton.Clicked += () => UtilityFunctions.OnHelp(_statusLabel);
        
        var exportButton = new Button("F4 - Export Project")
        {
            X = Pos.Right(helpButton) + 1,
            Y = 0
        };
        exportButton.Clicked += () => ExportProjectFunctions.OnExportProject(_projectManager, _exportService, _statusLabel);
        
        var exitButton = new Button("Esc - Quit")
        {
            X = Pos.Right(exportButton) + 1,
            Y = 0
        };
        exitButton.Clicked += () => UtilityFunctions.OnExit(_statusLabel);
        
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
        
        // Apply color scheme to all child components
        ApplyColorSchemeToChildren();
    }
    
    // Apply color scheme to all child UI elements
    private void ApplyColorSchemeToChildren()
    {
        var colorScheme = new Terminal.Gui.ColorScheme
        {
            Normal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            Focus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.Black, Terminal.Gui.Color.White),
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.White, Terminal.Gui.Color.Black)
        };
        
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
    
    // Handle keyboard input and shortcuts for the main window
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        return KeyboardNavigationFunctions.ProcessKey(keyEvent, _projectManager, _projectListView, _projectInfoLabel, _statusLabel, _exportService, RefreshProjectList);
    }
}
