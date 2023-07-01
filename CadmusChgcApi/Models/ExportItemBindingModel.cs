using System.ComponentModel.DataAnnotations;

namespace CadmusChgcApi.Models;

public class ExportItemBindingModel
{
    [MaxLength(50000)]
    public string? TargetXml { get; set; }
}
