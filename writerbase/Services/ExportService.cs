using writerbase.Models;
using writerbase.Services.Docx;
using writerbase.Services.ODT;

namespace writerbase.Services;

/// <summary>
/// Main export service that coordinates different export formats.
/// Delegates specific format exports to specialized services.
/// </summary>
public class ExportService
{
    private readonly DocxNovelExportService _docxNovelExportService;
    private readonly DocxShortExportService _docxShortExportService;
    private readonly ODTNovelExportService _odtNovelExportService;
    private readonly ODTShortExportService _odtShortExportService;

    /// <summary>
    /// Initializes a new instance of the ExportService with required dependencies.
    /// </summary>
    public ExportService()
    {
        _docxNovelExportService = new DocxNovelExportService();
        _docxShortExportService = new DocxShortExportService();
        _odtNovelExportService = new ODTNovelExportService();
        _odtShortExportService = new ODTShortExportService();
    }

    /// <summary>
    /// Exports a project to DOCX format in novel style with table of contents and chapter headings.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToDocxNovel(Project project, string filePath)
    {
        _docxNovelExportService.ExportToDocx(project, filePath);
    }

    /// <summary>
    /// Exports a project to DOCX format in short story style with continuous text.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToDocxShort(Project project, string filePath)
    {
        _docxShortExportService.ExportToDocx(project, filePath);
    }

    /// <summary>
    /// Exports a project to ODT format in novel style with table of contents and chapter headings.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToOdtNovel(Project project, string filePath)
    {
        _odtNovelExportService.ExportToOdt(project, filePath);
    }

    /// <summary>
    /// Exports a project to ODT format in short story style with continuous text.
    /// </summary>
    /// <param name="project">The project to export</param>
    /// <param name="filePath">The output file path</param>
    public void ExportToOdtShort(Project project, string filePath)
    {
        _odtShortExportService.ExportToOdt(project, filePath);
    }
}
