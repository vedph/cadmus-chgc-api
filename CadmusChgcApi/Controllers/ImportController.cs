using Cadmus.Chgc.Import;
using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.Import.Excel;
using Cadmus.Import;
using Cadmus.Index;
using CadmusChgcApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Cadmus.Api.Models;
using Cadmus.Index.Config;

namespace CadmusChgcApi.Controllers;

/// <summary>
/// Import controller.
/// </summary>
[Authorize]
[ApiController]
public sealed class ImportController : ControllerBase
{
    private readonly IRepositoryProvider _repositoryProvider;
    // note that injecting a Serilog logger via MS ILogger<T> requires
    // package Serilog.Extensions.Logging. Also notice you must inject
    // ILogger<T>, not just ILogger.
    private readonly ILogger<ImportController> _logger;
    private readonly IItemIndexWriter _indexWriter;

    public ImportController(IRepositoryProvider repositoryProvider,
        IItemIndexWriter writer,
        ILogger<ImportController> logger)
    {
        _repositoryProvider = repositoryProvider;
        _indexWriter = writer;
        _logger = logger;
    }

    /// <summary>
    /// Imports empty image items from the specified TEI document for the
    /// manuscript specified by <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The group (manuscript) identifier.</param>
    /// <param name="model">The model.</param>
    /// <returns>Object with count=imported items count or error=error message.
    /// </returns>
    [HttpPost("api/import/groups/{id}")]
    public ImportItemModel Import([FromRoute] string id,
        [FromBody] ImportItemBindingModel model)
    {
        ICadmusRepository repository = _repositoryProvider.CreateRepository();

        try
        {
            XDocument doc = XDocument.Parse(model.Xml ?? "");
            ChgcItemImporter importer = new(repository, _indexWriter)
            {
                UriShortenerPattern = !string.IsNullOrEmpty(model.UriShortenerPattern)
                    ? new Regex(model.UriShortenerPattern)
                    : null
            };
            int added = importer.Import(id, doc);
            return new ImportItemModel
            {
                Count = added
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing item {Id}: {Message}",
                id, ex.Message);
            return new ImportItemModel
            {
                Error = ex.Message
            };
        }
    }

    private static ImportUpdateMode GetMode(char c)
    {
        return char.ToUpperInvariant(c) switch
        {
            'P' => ImportUpdateMode.Patch,
            'S' => ImportUpdateMode.Synch,
            _ => ImportUpdateMode.Replace,
        };
    }

    [Authorize(Roles = "admin")]
    [HttpPost("api/import/thesauri")]
    public ImportThesauriResult UploadThesauri(
        [FromForm(Name = "file")] IFormFile file,
        [FromQuery] ImportThesauriBindingModel model)
    {
        _logger?.LogInformation("User {UserName} importing thesauri from " +
            "{FileName} from {IP} (dry={IsDry})",
            User.Identity!.Name,
            file.FileName,
            HttpContext.Connection.RemoteIpAddress,
            model.DryRun == true);

        ICadmusRepository repository = _repositoryProvider.CreateRepository();

        ExcelThesaurusReaderOptions xlsOptions = new()
        {
            SheetIndex = model.ExcelSheet == null? 0 : model.ExcelSheet.Value - 1,
            RowOffset = model.ExcelRow == null? 0 : model.ExcelRow.Value - 1,
            ColumnOffset = model.ExcelColumn == null? 0 : model.ExcelColumn.Value - 1
        };

        try
        {
            Stream stream = file.OpenReadStream();
            using IThesaurusReader reader =
                Path.GetExtension(file.FileName).ToLowerInvariant() switch
                {
                    ".csv" => new CsvThesaurusReader(stream),
                    ".xls" => new ExcelThesaurusReader(stream, xlsOptions),
                    ".xlsx" => new ExcelThesaurusReader(stream),
                    _ => new JsonThesaurusReader(stream)
                };

            Thesaurus? source;
            ImportUpdateMode mode = GetMode(model.Mode?[0] ?? 'R');
            List<string> ids = [];

            while ((source = reader.Next()) != null)
            {
                if (string.IsNullOrEmpty(source.Id))
                    throw new InvalidOperationException("No ID for thesaurus");

                _logger?.LogInformation("Importing thesaurus ID: {Id}", source.Id);
                ids.Add(source.Id);

                // fetch from repository
                Thesaurus? target = repository.GetThesaurus(source.Id);

                // import
                Thesaurus result = ThesaurusHelper.CopyThesaurus(
                    source, target, mode);

                // save
                if (model.DryRun != true) repository.AddThesaurus(result);
            }

            return new ImportThesauriResult
            {
                ImportedIds = ids
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error importing thesauri: {Message}",
                ex.Message);
            return new ImportThesauriResult
            {
                Error = ex.Message
            };
        }
    }
}
