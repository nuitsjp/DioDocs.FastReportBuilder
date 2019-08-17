using System.IO;
using Newtonsoft.Json.Linq;

namespace InvoiceService.Function.ReportBuilder
{
    public interface IJsonReportBuilder
    {
        void Build(TextReader reader, Stream output, SaveFileFormat saveFileFormat);
        void Build(JToken input, Stream output, SaveFileFormat saveFileFormat);
    }
}