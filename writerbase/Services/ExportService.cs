using writerbase.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace writerbase.Services;

/// <summary>
/// Service class responsible for exporting projects to DOCX format.
/// Creates Word documents with proper formatting and structure.
/// </summary>
public class ExportService
{
    /// <summary>
    /// Exports a project to DOCX format using DocumentFormat.OpenXml.
    /// Creates a Word document with project name as title and chapters with bold headers.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToDocx(Project project, string filePath)
    {
        try
        {
            using var document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());
            
            // Add spacing to center text vertically on title page
            for (int i = 0; i < 10; i++)
            {
                body.AppendChild(new Paragraph());
            }
            
            // Add project title
            var titleRun = new Run(
                new Text(project.Title)
            );
            titleRun.RunProperties = new RunProperties(new RunFonts() { Ascii = "Courier New" });
            titleRun.RunProperties.AppendChild(new Bold());
            
            var titleParagraph = new Paragraph(titleRun);
            titleParagraph.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );
            titleParagraph.ParagraphProperties.AppendChild(new ParagraphStyleId() { Val = "Heading1" });
            body.AppendChild(titleParagraph);
            
            // Add project description if available
            if (!string.IsNullOrWhiteSpace(project.Description))
            {
                var descRun = new Run(
                    new Text($"Description: {project.Description}")
                );
                descRun.RunProperties = new RunProperties(new RunFonts() { Ascii = "Courier New" });
                
                var descParagraph = new Paragraph(descRun);
                descParagraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
                body.AppendChild(descParagraph);
                body.AppendChild(new Paragraph()); // Empty line
            }
            
            // Add page break after title page
            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            
            // Add chapters
            var orderedChapters = project.Chapters.OrderBy(c => c.Order).ToList();
            for (int i = 0; i < orderedChapters.Count; i++)
            {
                var chapter = orderedChapters[i];
                
                // Chapter header (bold)
                var chapterRun = new Run(
                    new Text($"Chapter {chapter.Order}: {chapter.Title}")
                );
                chapterRun.RunProperties = new RunProperties(new RunFonts() { Ascii = "Courier New" });
                chapterRun.RunProperties.AppendChild(new Bold());
                
                var chapterHeader = new Paragraph(chapterRun);
                chapterHeader.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Heading2" }
                );
                body.AppendChild(chapterHeader);
                
                // Chapter content
                if (!string.IsNullOrWhiteSpace(chapter.Content))
                {
                    var contentRun = new Run(
                        new Text(chapter.Content)
                    );
                    contentRun.RunProperties = new RunProperties(new RunFonts() { Ascii = "Courier New" });
                    
                    var contentParagraph = new Paragraph(contentRun);
                    body.AppendChild(contentParagraph);
                }
                
                body.AppendChild(new Paragraph()); // Empty line
            }
            

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export to DOCX: {ex.Message}");
        }
    }
}
