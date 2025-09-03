using System.ComponentModel.DataAnnotations;

namespace writerbase.Models;

public class Chapter
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public int Order { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime LastModified { get; set; } = DateTime.Now;
    

    
    public int WordCount => string.IsNullOrWhiteSpace(Content) ? 0 : Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    
    public int CharacterCount => Content?.Length ?? 0;
    
    public int CharacterCountNoSpaces => Content?.Replace(" ", "").Replace("\n", "").Replace("\r", "").Length ?? 0;
}
