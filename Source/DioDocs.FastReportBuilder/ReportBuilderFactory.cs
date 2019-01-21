using System.IO;

namespace DioDocs.FastReportBuilder
{
    public class ReportBuilderFactory : IReportBuilderFactory
    {
        public IReportBuilder<TReportRow> Create<TReportRow>(Stream excel)
        {
            return new ReportBuilder<TReportRow>(excel);
        }
    }
}