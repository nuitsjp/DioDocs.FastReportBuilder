using System.IO;

namespace DioDocs.FastReportBuilder
{
    public interface IReportBuilderFactory
    {
        IReportBuilder<TReportRow> Create<TReportRow>(Stream excel);
    }
}