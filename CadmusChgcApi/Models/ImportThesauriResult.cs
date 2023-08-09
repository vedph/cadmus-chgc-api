namespace CadmusChgcApi.Models;

public class ImportThesauriResult
{
    public IList<string> ImportedIds { get; set; }
    public string? Error { get; set; }

    public ImportThesauriResult()
    {
        ImportedIds = new List<string>();
    }
}
