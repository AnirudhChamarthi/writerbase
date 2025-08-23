using Terminal.Gui;
using WritingApp.Models;
using WritingApp.Services;

namespace WritingApp.UI;

public class ChapterManagerWindow : Window
{
    private readonly ProjectManager _projectManager;
    private ListView _chapterListView;
    private Label _statusLabel;
    private Project? _currentProject;
    
    public ChapterManagerWindow(ProjectManager projectManager)
    {
        _projectManager = projectManager;
        _currentProject = projectManager.CurrentProject;
        
        Title = "Chapter Manager";
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
        RefreshChapterList();
    }
    
    private void InitializeUI()
    {
        // Chapter list
        var listFrame = new FrameView("Chapters")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 3
        };
        
        _chapterListView = new ListView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        _chapterListView.SelectedItemChanged += OnChapterSelected;
        listFrame.Add(_chapterListView);
        Add(listFrame);
        
        // Buttons
        var buttonFrame = new FrameView("Actions")
        {
            X = 0,
            Y = Pos.Bottom(listFrame),
            Width = Dim.Fill(),
            Height = 3
        };
        
        var addButton = new Button("[A]dd Chapter")
        {
            X = 0,
            Y = 0
        };
        addButton.Clicked += OnAddChapter;
        
        var editButton = new Button("[E]dit Chapter")
        {
            X = Pos.Right(addButton) + 1,
            Y = 0
        };
        editButton.Clicked += OnEditChapter;
        
        var deleteButton = new Button("[D]elete Chapter")
        {
            X = Pos.Right(editButton) + 1,
            Y = 0
        };
        deleteButton.Clicked += OnDeleteChapter;
        
        var closeButton = new Button("[C]lose")
        {
            X = Pos.Right(deleteButton) + 1,
            Y = 0
        };
        closeButton.Clicked += OnClose;
        
        buttonFrame.Add(addButton, editButton, deleteButton, closeButton);
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
    }
    
    private void RefreshChapterList()
    {
        _currentProject = _projectManager.CurrentProject;
        
        if (_currentProject == null)
        {
            _chapterListView.SetSource(new List<string> { "No project loaded" });
            _statusLabel.Text = "No project loaded";
            return;
        }
        
        var chapters = _currentProject.Chapters
            .OrderBy(c => c.Order)
            .Select(c => $"{c.Order}. {c.Title} ({c.WordCount} words)")
            .ToList();
            
        _chapterListView.SetSource(chapters);
        _statusLabel.Text = $"Project: {_currentProject.Title} | Total Words: {_currentProject.TotalWordCount}";
    }
    
    private void OnChapterSelected(ListViewItemEventArgs args)
    {
        // Handle chapter selection
    }
    
    private void OnAddChapter()
    {
        if (_currentProject == null)
        {
            MessageBox.ErrorQuery("Error", "No project loaded", "OK");
            return;
        }
        
        var dialog = new Dialog("Add Chapter")
        {
            Width = 60,
            Height = 8
        };
        
        var titleLabel = new Label("Chapter Title:")
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
        
        var okButton = new Button("OK")
        {
            X = 0,
            Y = 3
        };
        okButton.Clicked += () =>
        {
            var title = titleField.Text.ToString();
            if (!string.IsNullOrWhiteSpace(title))
            {
                _projectManager.AddChapter(title);
                RefreshChapterList();
                dialog.Running = false;
            }
        };
        
        var cancelButton = new Button("Cancel")
        {
            X = Pos.Right(okButton) + 1,
            Y = 3
        };
        cancelButton.Clicked += () => dialog.Running = false;
        
        dialog.Add(titleLabel, titleField, okButton, cancelButton);
        Application.Run(dialog);
    }
    
    private void OnEditChapter()
    {
        if (_currentProject == null || _chapterListView.SelectedItem < 0)
        {
            MessageBox.ErrorQuery("Error", "No chapter selected", "OK");
            return;
        }
        
        var selectedIndex = _chapterListView.SelectedItem;
        var orderedChapters = _currentProject.Chapters.OrderBy(c => c.Order).ToList();
        
        if (selectedIndex >= orderedChapters.Count)
        {
            MessageBox.ErrorQuery("Error", "Invalid chapter selection", "OK");
            return;
        }
        
        var chapter = orderedChapters[selectedIndex];
        
        // Open chapter editor
        var editor = new ChapterEditorWindow(_projectManager, chapter);
        Application.Run(editor);
        RefreshChapterList();
    }
    
    private void OnDeleteChapter()
    {
        if (_currentProject == null || _chapterListView.SelectedItem < 0)
        {
            MessageBox.ErrorQuery("Error", "No chapter selected", "OK");
            return;
        }
        
        var selectedIndex = _chapterListView.SelectedItem;
        var orderedChapters = _currentProject.Chapters.OrderBy(c => c.Order).ToList();
        
        if (selectedIndex >= orderedChapters.Count)
        {
            MessageBox.ErrorQuery("Error", "Invalid chapter selection", "OK");
            return;
        }
        
        var chapter = orderedChapters[selectedIndex];
        
        var result = MessageBox.Query("Confirm Delete", 
            $"Are you sure you want to delete '{chapter.Title}'?", 
            "Yes", "No");
            
        if (result == 0)
        {
            _projectManager.DeleteChapter(chapter.Id);
            RefreshChapterList();
        }
    }
    
    private void OnClose()
    {
        Application.RequestStop();
    }
    
    private void AddKeyboardShortcuts()
    {
        // Global keyboard event handler for chapter manager
        Application.RootKeyEvent += (e) =>
        {
            // Only handle single key presses (not combinations)
            if (e.IsCtrl || e.IsAlt || e.IsShift) return false;
            
            // Convert to uppercase for case-insensitive comparison
            var keyChar = char.ToUpper((char)e.Key);
            
            switch (keyChar)
            {
                case 'A':
                    OnAddChapter();
                    return true;
                    
                case 'E':
                    OnEditChapter();
                    return true;
                    
                case 'D':
                    OnDeleteChapter();
                    return true;
                    
                case 'C':
                    OnClose();
                    return true;
                    
                default:
                    return false;
            }
        };
    }
}
