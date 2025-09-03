using writerbase.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace writerbase.Services.Docx;

/// <summary>
/// Service class responsible for exporting projects to DOCX format in novel style.
/// Creates Word documents with project name as title, table of contents, and chapter headings.
/// </summary>
public class DocxNovelExportService
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
            
            // Add styles part to define custom heading styles
            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            var styles = new Styles();
            
            // Create custom Heading1 style with Courier font
            var heading1Style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Heading1"
            };
            heading1Style.Append(new StyleName() { Val = "Heading 1" });
            heading1Style.Append(new BasedOn() { Val = "Normal" });
            heading1Style.Append(new NextParagraphStyle() { Val = "Normal" });
            heading1Style.Append(new UIPriority() { Val = 9 });
            heading1Style.Append(new PrimaryStyle());
            
            var heading1RunProps = new StyleRunProperties();
            heading1RunProps.Append(new RunFonts() { Ascii = "Courier New", HighAnsi = "Courier New" });
            heading1RunProps.Append(new Bold());
            heading1RunProps.Append(new FontSize() { Val = "32" }); // 16pt font
            heading1Style.Append(heading1RunProps);
            
            // Create custom Heading2 style with Courier font
            var heading2Style = new Style()
            {
                Type = StyleValues.Paragraph,
                StyleId = "Heading2"
            };
            heading2Style.Append(new StyleName() { Val = "Heading 2" });
            heading2Style.Append(new BasedOn() { Val = "Normal" });
            heading2Style.Append(new NextParagraphStyle() { Val = "Normal" });
            heading2Style.Append(new UIPriority() { Val = 9 });
            heading2Style.Append(new PrimaryStyle());
            
            var heading2RunProps = new StyleRunProperties();
            heading2RunProps.Append(new RunFonts() { Ascii = "Courier New", HighAnsi = "Courier New" });
            heading2RunProps.Append(new Bold());
            heading2RunProps.Append(new FontSize() { Val = "28" }); // 14pt font
            heading2Style.Append(heading2RunProps);
            
            styles.Append(heading1Style);
            styles.Append(heading2Style);
            stylesPart.Styles = styles;
            
            var body = mainPart.Document.AppendChild(new Body());
            
            // Get ordered chapters list for use throughout the method
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
            titleParagraph.ParagraphProperties.AppendChild(new ParagraphStyleId() { Val = "Heading1" });
            body.AppendChild(titleParagraph);
            
            // Add page break after title page
            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            
            // Add Table of Contents page
            var tocTitle = new Paragraph(
                new Run(
                    new Text("Table of Contents")
                )
            );
            tocTitle.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );
            tocTitle.ParagraphProperties.AppendChild(new ParagraphStyleId() { Val = "Heading1" });
            body.AppendChild(tocTitle);
            
            // Add spacing after TOC title
            body.AppendChild(new Paragraph());
            
            // Add TOC entries for each chapter
            for (int i = 0; i < orderedChapters.Count; i++)
            {
                var chapter = orderedChapters[i];
                var tocEntry = new Paragraph(
                    new Run(
                        new Text($"Chapter {chapter.Order}: {chapter.Title}")
                    )
                );
                tocEntry.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Heading2" }
                );
                body.AppendChild(tocEntry);
            }
            
            // Add page break after TOC
            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
            
            // Add chapters
            for (int i = 0; i < orderedChapters.Count; i++)
            {
                var chapter = orderedChapters[i];
                
                // Chapter header (bold)
                var chapterHeader = new Paragraph(
                    new Run(
                        new Text($"Chapter {chapter.Order}: {chapter.Title}")
                    )
                );
                chapterHeader.ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Heading2" }
                );
                body.AppendChild(chapterHeader);
                
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
                
                body.AppendChild(new Paragraph()); // Empty line
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to export to DOCX: {ex.Message}");
        }
    }
}
