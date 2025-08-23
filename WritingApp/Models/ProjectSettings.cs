namespace WritingApp.Models;

public class ProjectSettings
{
    public bool AutoSave { get; set; } = true;
    
    public int AutoSaveIntervalSeconds { get; set; } = 300; // 5 minutes
    
    public string Theme { get; set; } = "dark";
    
    public int FontSize { get; set; } = 12;
    
    public int TabSize { get; set; } = 4;
    
    public bool ShowWordCount { get; set; } = true;
    
    public bool ShowCharacterCount { get; set; } = true;
    
    public int DailyWordGoal { get; set; } = 1000;
    
    public bool DistractionFreeMode { get; set; } = false;
}
