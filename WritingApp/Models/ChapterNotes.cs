namespace WritingApp.Models;

public class ChapterNotes
{
    public string PlotNotes { get; set; } = string.Empty;
    
    public string CharacterNotes { get; set; } = string.Empty;
    
    public string ResearchNotes { get; set; } = string.Empty;
    
    public string RevisionNotes { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new();
    
    public DateTime LastModified { get; set; } = DateTime.Now;
}
