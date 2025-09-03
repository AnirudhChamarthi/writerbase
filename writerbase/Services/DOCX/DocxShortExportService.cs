using writerbase.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace writerbase.Services.Docx;

/// <summary>
/// Service class responsible for exporting projects to DOCX format in short story style.
/// Creates Word documents with project name as title and continuous text with chapter breaks.
/// </summary>
public class DocxShortExportService
{
    /// <summary>
    /// Exports a project to DOCX format using DocumentFormat.OpenXml.
    /// Creates a Word document with project name as title and continuous text with chapter breaks.
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
            
            // Add styles part to define custom title style
            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            var styles = new Styles();
            
            // Create custom Title style with Courier font
            var titleStyle = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Title"
            };
            titleStyle.Append(new StyleName() { Val = "Title" });
            titleStyle.Append(new BasedOn() { Val = "Normal" });
            titleStyle.Append(new NextParagraphStyle() { Val = "Normal" });
            titleStyle.Append(new UIPriority() { Val = 9 });
            titleStyle.Append(new PrimaryStyle());
            
            var titleRunProps = new StyleRunProperties();
            titleRunProps.Append(new RunFonts() { Ascii = "Courier New", HighAnsi = "Courier New" });
            titleRunProps.Append(new Bold());
            titleRunProps.Append(new FontSize() { Val = "32" }); // 16pt font
            titleStyle.Append(titleRunProps);
            
            styles.Append(titleStyle);
            stylesPart.Styles = styles;
            
            var body = mainPart.Document.AppendChild(new Body());
            
            // Get ordered chapters list
            var orderedChapters = project.Chapters.OrderBy(c => c.Order).ToList();
            
            // Add spacing to center text vertically on title page
            for (int i = 0; i < 10; i++)
            {
                body.AppendChild(new Paragraph());
            }
            
            // Add project title
            var titleParagraph = new Paragraph(
                new Run(
                    new Text(project.Title)
                )
            );
            titleParagraph.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );
            titleParagraph.ParagraphProperties.AppendChild(new ParagraphStyleId() { Val = "Title" });
            body.AppendChild(titleParagraph);
            
            // Add page break after title page
            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            
            // Add chapters as continuous text with double newlines between chapters
            for (int i = 0; i < orderedChapters.Count; i++)
            {
                var chapter = orderedChapters[i];
                
                // Chapter content - split by line breaks and create paragraphs for each line
                if (!string.IsNullOrWhiteSpace(chapter.Content))
                {
                    var lines = chapter.Content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var contentRun = new Run(new Text(line));
                            contentRun.RunProperties = new RunProperties();
                            contentRun.RunProperties.Append(new RunFonts() { Ascii = "Courier New", HighAnsi = "Courier New" });
                            
                            var contentParagraph = new Paragraph(contentRun);
                            body.AppendChild(contentParagraph);
                        }
                        else
                        {
                            // Empty line - add empty paragraph
                            body.AppendChild(new Paragraph());
                        }
                    }
                }
                
                // Add double newline between chapters (except after the last chapter)
                if (i < orderedChapters.Count - 1)
                {
                    body.AppendChild(new Paragraph()); // First newline
                    body.AppendChild(new Paragraph()); // Second newline
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export to DOCX: {ex.Message}");
        }
    }
}
