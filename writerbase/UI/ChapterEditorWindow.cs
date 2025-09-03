using Terminal.Gui;
using writerbase.Models;
using writerbase.Services;

namespace writerbase.UI;

public class ChapterEditorWindow : Window
{
    private readonly ProjectManager _projectManager;
    private readonly Chapter _chapter;
    private TextView _contentEditor = null!;
    private TextField _titleField = null!;
    private Label _wordCountLabel = null!;
    private Label _characterCountLabel = null!;
    private Label _statusLabel = null!;
    
    public ChapterEditorWindow(ProjectManager projectManager, Chapter chapter)
    {
        _projectManager = projectManager;
        _chapter = chapter;
        
        Title = $"Editing: {chapter.Title}";
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
        LoadChapterContent();
    }
    
    // Initialize the chapter editor user interface
    private void InitializeUI()
    {
        // Title bar
        var titleFrame = new FrameView("Chapter Title")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 3
        };
        
        _titleField = new TextField(_chapter.Title)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill() - 2
        };
        
        titleFrame.Add(_titleField);
        Add(titleFrame);
        
        // Content editor
        var editorFrame = new FrameView("Content")
        {
            X = 0,
            Y = Pos.Bottom(titleFrame),
            Width = Dim.Fill(),
            Height = Dim.Fill() - 10
        };
        
        _contentEditor = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            WordWrap = true,
            AllowsTab = true,
            AllowsReturn = true
        };
        
        _contentEditor.TextChanged += OnContentChanged;
        editorFrame.Add(_contentEditor);
        Add(editorFrame);
        
        // Statistics bar
        var statsFrame = new FrameView("Statistics")
        {
            X = 0,
            Y = Pos.Bottom(editorFrame),
            Width = Dim.Fill(),
            Height = 3
        };
        
        _wordCountLabel = new Label("Words: 0")
        {
            X = 0,
            Y = 0
        };
        
        _characterCountLabel = new Label("Characters: 0")
        {
            X = Pos.Right(_wordCountLabel) + 2,
            Y = 0
        };
        
        statsFrame.Add(_wordCountLabel, _characterCountLabel);
        Add(statsFrame);
        
        // Button bar for mouse-only interface
        var buttonFrame = new FrameView("Actions")
        {
            X = 0,
            Y = Pos.Bottom(statsFrame),
            Width = Dim.Fill(),
            Height = 4
        };
        
        var saveButton = new Button("F1 - Save")
        {
            X = 2,
            Y = 1
        };
        saveButton.Clicked += SaveChapter;
        
        var closeButton = new Button("Esc - Close")
        {
            X = Pos.Right(saveButton) + 2,
            Y = 1
        };
        closeButton.Clicked += () => {
            var result = MessageBox.Query("Save Changes", 
                "Do you want to save your changes before closing?", 
                "Save & Close", "Close Without Saving", "Cancel");
                
            switch (result)
            {
                case 0: // Save & Close
                    SaveChapter();
                    Application.RequestStop();
                    break;
                case 1: // Close Without Saving
                    Application.RequestStop();
                    break;
                case 2: // Cancel
                    break;
            }
        };
        
        var helpButton = new Button("Help")
        {
            X = Pos.Right(closeButton) + 2,
            Y = 1
        };
        helpButton.Clicked += ShowHelp;
        
        buttonFrame.Add(saveButton, closeButton, helpButton);
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
    }
    
    // Load chapter content into the editor fields
    private void LoadChapterContent()
    {
        _titleField.Text = _chapter.Title;
        _contentEditor.Text = _chapter.Content;
        UpdateStatistics();
    }
    
    // Handle content changes and update statistics
    private void OnContentChanged()
    {
        UpdateStatistics();
        _statusLabel.Text = "Modified - Click Save button to save";
    }
    
    // Update word and character count statistics
    private void UpdateStatistics()
    {
        var content = _contentEditor.Text.ToString();
        var wordCount = string.IsNullOrWhiteSpace(content) ? 0 : content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var characterCount = content?.Length ?? 0;
        
        _wordCountLabel.Text = $"Words: {wordCount}";
        _characterCountLabel.Text = $"Characters: {characterCount}";
    }
    
    // AddKeyboardShortcuts method removed - using ProcessKey override instead
    
    // Save chapter changes to the project manager
    private void SaveChapter()
    {
        try
        {
            var title = _titleField.Text.ToString() ?? string.Empty;
            var content = _contentEditor.Text.ToString() ?? string.Empty;
            
            _projectManager.UpdateChapter(_chapter.Id, title, content);
            _statusLabel.Text = $"Saved at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Save Error", $"Failed to save chapter: {ex.Message}", "OK");
        }
    }
    
    // Handle keyboard shortcuts for editor operations
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        // Handle F1 for Save
        if (keyEvent.Key == Key.F1)
        {
            SaveChapter();
            return true; // Consume the key event
        }
        
        // Handle Esc for Close
        if (keyEvent.Key == Key.Esc)
        {
            var result = MessageBox.Query("Save Changes", 
                "Do you want to save your changes before closing?", 
                "Save & Close", "Close Without Saving", "Cancel");
                
            switch (result)
            {
                case 0: // Save & Close
                    SaveChapter();
                    Application.RequestStop();
                    break;
                case 1: // Close Without Saving
                    Application.RequestStop();
                    break;
                case 2: // Cancel
                    break;
            }
            return true; // Consume the key event
        }
        
        // Let other keys be processed normally
        return base.ProcessKey(keyEvent);
    }
    
    // Display help information for the editor
    private void ShowHelp()
    {
        var helpText = @"
Mouse-Only Interface:
Save      - Click Save button
Close     - Click Close button
Help      - Click Help button

Writing Tips:
- Use Tab for indentation
- Press Enter for new paragraphs
- Word count updates automatically
- Click buttons to perform actions
        ";
        
        var helpDialog = new Dialog("Help")
        {
            Width = 60,
            Height = 15
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
}
