using System.ComponentModel.DataAnnotations;

namespace CadmusChgcApi.Models;

public sealed class ImportItemBindingModel : XmlBindingModel
{
    /// <summary>
    /// Gets or sets the pattern optionally used to shorten the URI when
    /// building the item's description. The URI will be replaced by its
    /// first match group.
    /// </summary>
    [MaxLength(1000)]
    public string? UriShortenerPattern { get; set; }
}
