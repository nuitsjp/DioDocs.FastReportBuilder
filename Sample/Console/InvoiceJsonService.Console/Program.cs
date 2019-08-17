using System;
using System.Diagnostics;
using System.IO;
using DioDocs.FastReportBuilder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceJsonService.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environments.SetLicenseKey(Secrets.DioDocsKey);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            BuildFromInvoiceRequest();
            //BuildFromInvoiceJson();

            stopwatch.Stop();
            System.Console.WriteLine($"Completed. Elapsed:{stopwatch.Elapsed}");
        }

        private static void BuildFromInvoiceRequest()
        {
            using (var reader = new JsonTextReader(new StreamReader(new FileStream("request.json", FileMode.Open))))
            using (var stream = new FileStream("Invoice.xlsx", FileMode.Open))
            using (var output = File.Create("output.pdf"))
            {
                var request = JToken.Load(reader);
                var data = request["Data"];
                var builder = new JsonReportBuilder(stream);
                builder.Build(data, output, SaveFileFormat.Pdf);
            }
        }

        private static void BuildFromInvoiceJson()
        {
            using (var reader = new StreamReader(new FileStream("Invoice.json", FileMode.Open)))
            using (var stream = new FileStream("Invoice.xlsx", FileMode.Open))
            using (var output = File.Create("output.pdf"))
            {
                var builder = new JsonReportBuilder(stream);
                builder.Build(reader, output, SaveFileFormat.Pdf);
            }
        }
    }
}
