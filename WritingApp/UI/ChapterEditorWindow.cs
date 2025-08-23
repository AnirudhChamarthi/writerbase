using Terminal.Gui;
using WritingApp.Models;
using WritingApp.Services;

namespace WritingApp.UI;

public class ChapterEditorWindow : Window
{
    private readonly ProjectManager _projectManager;
    private readonly Chapter _chapter;
    private TextView _contentEditor;
    private TextField _titleField;
    private Label _wordCountLabel;
    private Label _characterCountLabel;
    private Label _statusLabel;
    
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
            HotNormal = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.Black),
            HotFocus = Terminal.Gui.Attribute.Make(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.White)
        };
        
        InitializeUI();
        LoadChapterContent();
    }
    
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
            Height = Dim.Fill() - 4
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
        
        // Status bar
        _statusLabel = new Label("Ready")
        {
            X = 0,
            Y = Pos.Bottom(statsFrame),
            Width = Dim.Fill(),
            Height = 1
        };
        Add(_statusLabel);
        
        // Add keyboard shortcuts
        AddKeyboardShortcuts();
    }
    
    private void LoadChapterContent()
    {
        _titleField.Text = _chapter.Title;
        _contentEditor.Text = _chapter.Content;
        UpdateStatistics();
    }
    
    private void OnContentChanged()
    {
        UpdateStatistics();
        _statusLabel.Text = "Modified - Press Ctrl+S to save";
    }
    
    private void UpdateStatistics()
    {
        var content = _contentEditor.Text.ToString();
        var wordCount = string.IsNullOrWhiteSpace(content) ? 0 : content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var characterCount = content?.Length ?? 0;
        
        _wordCountLabel.Text = $"Words: {wordCount}";
        _characterCountLabel.Text = $"Characters: {characterCount}";
    }
    
    private void AddKeyboardShortcuts()
    {
        // Save shortcut (Ctrl+S)
        Application.RootKeyEvent += (e) =>
        {
            if (e.Key == Key.S && e.IsCtrl)
            {
                SaveChapter();
                return true;
            }
            return false;
        };
    }
    
    private void SaveChapter()
    {
        try
        {
            var title = _titleField.Text.ToString();
            var content = _contentEditor.Text.ToString();
            
            _projectManager.UpdateChapter(_chapter.Id, title, content);
            _statusLabel.Text = $"Saved at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Save Error", $"Failed to save chapter: {ex.Message}", "OK");
        }
    }
    
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        // Handle additional keyboard shortcuts
        switch (keyEvent.Key)
        {
            case Key.Esc:
                // Ask for confirmation before closing if unsaved changes
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
                        return true;
                }
                return true;
                
            case Key.F1:
                ShowHelp();
                return true;
        }
        
        return base.ProcessKey(keyEvent);
    }
    
    private void ShowHelp()
    {
        var helpText = @"
Keyboard Shortcuts:
Ctrl+S    - Save chapter
Esc       - Close editor
F1        - Show this help

Writing Tips:
- Use Tab for indentation
- Press Enter for new paragraphs
- Word count updates automatically
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
