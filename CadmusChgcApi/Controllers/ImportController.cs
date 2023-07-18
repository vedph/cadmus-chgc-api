using Cadmus.Chgc.Import;
using Cadmus.Core;
using Cadmus.Core.Storage;
using Cadmus.Index;
using CadmusChgcApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
    private readonly ILogger<ExportController> _logger;
    private readonly IItemIndexWriter _indexWriter;

    public ImportController(IRepositoryProvider repositoryProvider,
        IItemIndexWriter indexWriter,
        ILogger<ExportController> logger)
    {
        _repositoryProvider = repositoryProvider;
        _indexWriter = indexWriter;
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
}
