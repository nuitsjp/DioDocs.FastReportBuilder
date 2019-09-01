using System.IO;
using Newtonsoft.Json.Linq;

namespace DioDocs.FastReportBuilder
{
    public interface IJsonReportBuilder
    {
        void Build(TextReader reader, Stream output, SaveFileFormat saveFileFormat);
    }
}