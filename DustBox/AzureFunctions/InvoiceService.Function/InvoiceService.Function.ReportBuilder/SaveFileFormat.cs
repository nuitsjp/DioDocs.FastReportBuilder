namespace InvoiceService.Function.ReportBuilder
{
    /// <summary>
    /// ワークブックを保存する形式
    /// </summary>
    public enum SaveFileFormat
    {
        Xlsx = GrapeCity.Documents.Excel.SaveFileFormat.Xlsx,
        Csv = GrapeCity.Documents.Excel.SaveFileFormat.Csv,
        Pdf = GrapeCity.Documents.Excel.SaveFileFormat.Pdf,
        Xlsm = GrapeCity.Documents.Excel.SaveFileFormat.Xlsm,
    }
}