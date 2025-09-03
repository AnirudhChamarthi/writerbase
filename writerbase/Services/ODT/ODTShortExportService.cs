using writerbase.Models;
using System.Xml;

namespace writerbase.Services.ODT;

/// <summary>
/// Service class responsible for exporting projects to ODT format in short story style.
/// Creates OpenDocument files with project name as title and continuous text with chapter breaks.
/// </summary>
public class ODTShortExportService
{
    /// <summary>
    /// Exports a project to ODT format using OpenDocument Format.
    /// Creates an ODT document with project name as title and continuous text with chapter breaks.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToOdt(Project project, string filePath)
    {
        try
        {
            // Delete existing file if it exists (to handle overwrite)
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            // Create ODT file structure
            var odtContent = CreateODTContent(project);
            var odtStyles = CreateODTStyles();
            
            // Create the ODT file (ZIP archive)
            using var archive = System.IO.Compression.ZipFile.Open(filePath, System.IO.Compression.ZipArchiveMode.Create);
            
            // Add content.xml
            var contentEntry = archive.CreateEntry("content.xml");
            using (var contentStream = contentEntry.Open())
            using (var contentWriter = new StreamWriter(contentStream))
            {
                contentWriter.Write(odtContent);
            }
            
            // Add styles.xml
            var stylesEntry = archive.CreateEntry("styles.xml");
            using (var stylesStream = stylesEntry.Open())
            using (var stylesWriter = new StreamWriter(stylesStream))
            {
                stylesWriter.Write(odtStyles);
            }
            
            // Add META-INF/manifest.xml
            var manifestEntry = archive.CreateEntry("META-INF/manifest.xml");
            using (var manifestStream = manifestEntry.Open())
            using (var manifestWriter = new StreamWriter(manifestStream))
            {
                manifestWriter.Write(CreateManifest());
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export to ODT: {ex.Message}");
        }
    }
    
    private string CreateODTContent(Project project)
    {
        var orderedChapters = project.Chapters.OrderBy(c => c.Order).ToList();
        
        var content = new System.Text.StringBuilder();
        content.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        content.AppendLine("<office:document-content xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\"");
        content.AppendLine("                        xmlns:text=\"urn:oasis:names:tc:opendocument:xmlns:text:1.0\"");
        content.AppendLine("                        xmlns:style=\"urn:oasis:names:tc:opendocument:xmlns:style:1.0\"");
        content.AppendLine("                        xmlns:fo=\"urn:oasis:names:tc:opendocument:xmlns:fo:1.0\">");
        content.AppendLine("  <office:body>");
        content.AppendLine("    <office:text>");
        
        // Title page with spacing
        for (int i = 0; i < 10; i++)
        {
            content.AppendLine("      <text:p/>");
        }
        
        // Project title (centered, Title style)
        content.AppendLine($"      <text:p text:style-name=\"Title\">{XmlEscape(project.Title)}</text:p>");
        
        // Page break
        content.AppendLine("      <text:p text:style-name=\"PageBreak\"/>");
        
        // Chapters as continuous text with double newlines between chapters
        for (int i = 0; i < orderedChapters.Count; i++)
        {
            var chapter = orderedChapters[i];
            
            // Chapter content - split by line breaks
            if (!string.IsNullOrWhiteSpace(chapter.Content))
            {
                var lines = chapter.Content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        content.AppendLine($"      <text:p text:style-name=\"Normal\">{XmlEscape(line)}</text:p>");
                    }
                    else
                    {
                        content.AppendLine("      <text:p/>");
                    }
                }
            }
            
            // Add double newline between chapters (except after the last chapter)
            if (i < orderedChapters.Count - 1)
            {
                content.AppendLine("      <text:p/>"); // First newline
                content.AppendLine("      <text:p/>"); // Second newline
            }
        }
        
        content.AppendLine("    </office:text>");
        content.AppendLine("  </office:body>");
        content.AppendLine("</office:document-content>");
        
        return content.ToString();
    }
    
    private string CreateODTStyles()
    {
        var styles = new System.Text.StringBuilder();
        styles.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        styles.AppendLine("<office:document-styles xmlns:office=\"urn:oasis:names:tc:opendocument:xmlns:office:1.0\"");
        styles.AppendLine("                       xmlns:text=\"urn:oasis:names:tc:opendocument:xmlns:text:1.0\"");
        styles.AppendLine("                       xmlns:style=\"urn:oasis:names:tc:opendocument:xmlns:style:1.0\"");
        styles.AppendLine("                       xmlns:fo=\"urn:oasis:names:tc:opendocument:xmlns:fo:1.0\">");
        styles.AppendLine("  <office:styles>");
        
        // Title style - Courier, Bold, 32pt, Centered
        styles.AppendLine("    <style:style style:name=\"Title\" style:family=\"paragraph\">");
        styles.AppendLine("      <style:paragraph-properties fo:text-align=\"center\"/>");
        styles.AppendLine("      <style:text-properties fo:font-family=\"Courier New\" fo:font-size=\"32pt\" fo:font-weight=\"bold\"/>");
        styles.AppendLine("    </style:style>");
        
        // Normal style (Chapter content) - Courier
        styles.AppendLine("    <style:style style:name=\"Normal\" style:family=\"paragraph\">");
        styles.AppendLine("      <style:text-properties fo:font-family=\"Courier New\"/>");
        styles.AppendLine("    </style:style>");
        
        // Page break style
        styles.AppendLine("    <style:style style:name=\"PageBreak\" style:family=\"paragraph\">");
        styles.AppendLine("      <style:paragraph-properties fo:break-before=\"page\"/>");
        styles.AppendLine("    </style:style>");
        
        styles.AppendLine("  </office:styles>");
        styles.AppendLine("</office:document-styles>");
        
        return styles.ToString();
    }
    
    private string CreateManifest()
    {
        return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<manifest:manifest xmlns:manifest=""urn:oasis:names:tc:opendocument:xmlns:manifest:1.0"">
  <manifest:file-entry manifest:media-type=""application/vnd.oasis.opendocument.text"" manifest:full-path=""/""/>
  <manifest:file-entry manifest:media-type=""text/xml"" manifest:full-path=""content.xml""/>
  <manifest:file-entry manifest:media-type=""text/xml"" manifest:full-path=""styles.xml""/>
</manifest:manifest>";
    }
    
    private string XmlEscape(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
