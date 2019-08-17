using GrapeCity.Documents.Excel;

namespace DioDocs.FastReportBuilder
{
    public class Environments
    {
        public static void SetLicenseKey(string key)
        {
            Workbook.SetLicenseKey(key);
        }
    }
}