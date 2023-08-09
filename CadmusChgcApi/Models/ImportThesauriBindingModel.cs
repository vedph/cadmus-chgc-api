using System.ComponentModel.DataAnnotations;

namespace CadmusChgcApi.Models;

public class ImportThesauriBindingModel
{
    [RegularExpression("^[rpsRPS]$")]
    public string? Mode { get; set; }

    public int? ExcelSheet { get; set; }

    public int? ExcelRow { get; set; }

    public int? ExcelColumn { get; set; }

    public bool? DryRun { get; set; }
}
