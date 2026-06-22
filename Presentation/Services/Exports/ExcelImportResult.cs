namespace DukkanDepo.Presentation.Services.Exports;

public sealed class ExcelImportResult
{
    public int Added { get; set; }

    public int Updated { get; set; }

    public int Skipped { get; set; }
}