using System.ComponentModel.DataAnnotations;

namespace CadmusChgcApi.Models;

public class XmlBindingModel
{
    [MaxLength(50000)]
    public string? Xml { get; set; }
}
