using Cadmus.Chgc.Export;
using Cadmus.Core;
using Cadmus.Core.Storage;
using CadmusChgcApi.Models;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CadmusChgcApi.Controllers;

/// <summary>
/// Export controller.
/// </summary>
/// <seealso cref="ControllerBase" />
[Authorize]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly IRepositoryProvider _repositoryProvider;
    // note that injecting a Serilog logger via MS ILogger<T> requires
    // package Serilog.Extensions.Logging. Also notice you must inject
    // ILogger<T>, not just ILogger.
    private readonly ILogger<ExportController> _logger;

    public ExportController(IRepositoryProvider repositoryProvider,
        ILogger<ExportController> logger)
    {
        _repositoryProvider = repositoryProvider;
        _logger = logger;
    }

    /// <summary>
    /// Exports the item with the specified ID.
    /// </summary>
    /// <param name="id">The group identifier.</param>
    /// <param name="model">The export model, optionally including target XML
    /// code representing an existing document to be patched.</param>
    /// <returns>Result with <c>Xml</c> or <c>Error</c>.</returns>
    [HttpPost("api/export/groups/{id}")]
    public ExportItemModel ExportItem([FromRoute] string id,
        [FromBody] ExportItemBindingModel model)
    {
        ICadmusRepository repository = _repositoryProvider.CreateRepository();

        try
        {
            RamChgcTeiItemComposer composer = new();
            DocItemComposition composition = new();
            if (!string.IsNullOrEmpty(model.TargetXml))
            {
                composition.Document = XDocument.Parse(model.TargetXml);
            }
            composer.Open(composition);

            ItemFilter filter = new()
            {
                PageNumber = 1,
                PageSize = 50,
                GroupId = id
            };
            DataPage<ItemInfo> page = repository.GetItems(filter);
            if (page.Total == 0)
            {
                return new ExportItemModel { Error = $"Group {id} not found" };
            }

            while (page.PageNumber <= page.PageCount)
            {
                foreach (ItemInfo info in page.Items)
                {
                    IItem? item = repository.GetItem(info.Id!);
                    if (item != null) composer.Compose(item);
                }

                filter.PageNumber++;
                page = repository.GetItems(filter);
            }

            return new ExportItemModel
            {
                Xml = composition.Document!
                    .ToString(SaveOptions.OmitDuplicateNamespaces)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error composing item {Id}: {Message}",
                id, ex.Message);
            return new ExportItemModel
            {
                Error = ex.Message
            };
        }
    }
}
