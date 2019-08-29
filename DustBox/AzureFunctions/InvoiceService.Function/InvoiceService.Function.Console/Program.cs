using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InvoiceService.Function.ReportBuilder;

namespace InvoiceService.Function.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Parallel.ForEach(
            //    Enumerable.Range(1, 100),
            //    x => Build());
            Enumerable.Range(1, 200).ToList().ForEach(x => Build(x));
            //Build();
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());
            //System.Console.WriteLine(Build().MeasurementResults.Where(x => x.Item1 == "SavePdf").Select(x => $"{x.Item1}:{x.Item2}").Single());

            //invoiceBuilder
            //    .MeasurementResults.Select(x => $"{x.Item1}:{x.Item2}")
            //    .ToList()
            //    .ForEach(System.Console.WriteLine);
        }

        private static InvoiceBuilder Build(int number)
        {
            System.Console.WriteLine($"Build No.{number}");
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
