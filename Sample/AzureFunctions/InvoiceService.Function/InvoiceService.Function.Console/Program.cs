using System;
using System.IO;
using System.Linq;
using InvoiceService.Function.ReportBuilder;

namespace InvoiceService.Function.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Build();
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());

            //invoiceBuilder
            //    .MeasurementResults.Select(x => $"{x.Item1}:{x.Item2}")
            //    .ToList()
            //    .ForEach(System.Console.WriteLine);
        }

        private static InvoiceBuilder Build()
        {
            var invoiceBuilder = new InvoiceBuilder
            {
                Init = () => { },
                GetRequestStream = () => new MemoryStream(Properties.Resources.Request),
                GetTemplateStream = (template) => new MemoryStream(Properties.Resources.Invoice),
                Output = (pdf) =>
                {
                    using (var stream = new MemoryStream())
                    {
                        stream.Write(pdf, 0, pdf.Length);
                    }
                }
            };
            invoiceBuilder.Build();
            return invoiceBuilder;
        }
    }
}
