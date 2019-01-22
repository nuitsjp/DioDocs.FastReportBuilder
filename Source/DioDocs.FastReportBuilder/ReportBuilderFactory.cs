using System.IO;

namespace DioDocs.FastReportBuilder
{
    public class ReportBuilderFactory : IReportBuilderFactory
    {
        public IReportBuilder<TReportRow> Create<TReportRow>(Stream template)
        {
            return new ReportBuilder<TReportRow>(template);
        }
    }
}