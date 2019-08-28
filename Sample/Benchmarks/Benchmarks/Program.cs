using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DioDocs.FastReportBuilder;
using GrapeCity.Documents.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveFileFormat = DioDocs.FastReportBuilder.SaveFileFormat;

namespace Benchmarks
{
    class Program
    {
        private static readonly byte[] InvoiceJsonBaseExcel = File.ReadAllBytes("InvoiceJsonBase.xlsx");

        private static readonly byte[] InvoiceJsonBaseJson = File.ReadAllBytes("InvoiceJsonBase.json");

        static void Main(string[] args)
        {
            Workbook.SetLicenseKey(Secrets.DioDocsKey);
            //using (var jsonTextReader = new JsonTextReader(new StreamReader(new MemoryStream(InvoiceJsonBaseJson))))
            //using (var template = new MemoryStream(InvoiceJsonBaseExcel))
            //{
            //    var builder = new JsonReportBuilder(template);
            //    builder.Build(JToken.ReadFrom(jsonTextReader), Stream.Null, SaveFileFormat.Pdf);
            //}
            var summary = BenchmarkRunner.Run<BuildReport>();
        }
    }

    [RPlotExporter, RankColumn]
    public class BuildReport
    {
        private static readonly byte[] InvoiceTypeBaseExcel = File.ReadAllBytes("InvoiceTypeBase.xlsx");

        private static readonly byte[] InvoiceTypeBaseXml = File.ReadAllBytes("InvoiceTypeBase.xml");

        private static readonly byte[] InvoiceJsonBaseExcel = File.ReadAllBytes("InvoiceJsonBase.xlsx");

        private static readonly string InvoiceJsonBaseJson = File.ReadAllText("InvoiceJsonBase.json");

        [Benchmark]
        public void TypeBase()
        {
            using (var template = new MemoryStream(InvoiceTypeBaseExcel))
            {
                var invoice = JsonConvert.DeserializeObject<Invoice>(InvoiceJsonBaseJson);

                var reportBuilder =
                    new ReportBuilder<InvoiceDetail>(template)
                        // 単一項目のSetterを設定
                        .AddSetter("$SalesOrderId", cell => cell.Value = invoice.SalesOrderId)
                        .AddSetter("$OrderDate", cell => cell.Value = invoice.OrderDate)
                        .AddSetter("$CompanyName", cell => cell.Value = invoice.CompanyName)
                        .AddSetter("$Name", cell => cell.Value = invoice.Name)
                        .AddSetter("$Address", cell => cell.Value = invoice.Address)
                        .AddSetter("$PostalCode", cell => cell.Value = invoice.PostalCode)
                        // テーブルのセルに対するSetterを設定
                        .AddTableSetter("$ProductName", (cell, detail) => cell.Value = detail.ProductName)
                        .AddTableSetter("$UnitPrice", (cell, detail) => cell.Value = detail.UnitPrice)
                        .AddTableSetter("$OrderQuantity", (cell, detail) => cell.Value = detail.OrderQuantity);

                reportBuilder.Build(invoice.InvoiceDetails, Stream.Null, SaveFileFormat.Pdf);
            }
        }

        [Benchmark]
        public void JsonBase()
        {
            using (var template = new MemoryStream(InvoiceJsonBaseExcel))
            {
                var builder = new JsonReportBuilder(template);
                builder.Build(JToken.Parse(InvoiceJsonBaseJson), Stream.Null, SaveFileFormat.Pdf);
            }
        }
    }

    public class Invoice
    {
        public int SalesOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; } = new List<InvoiceDetail>();
    }

    public class InvoiceDetail
    {
        public int OrderQuantity { get; set; }
        public int UnitPrice { get; set; }
        public string ProductName { get; set; }
    }

}
