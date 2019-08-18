using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using InvoiceService.Function.ReportBuilder;

namespace InvoiceService.Function.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please press any key.");
            Console.ReadLine();

            var summary = BenchmarkRunner.Run<BuildReport>();

            Console.WriteLine("Completed. Please press any key.");
            Console.ReadLine();
        }
    }

    [SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 50)]
    [RPlotExporter, RankColumn]
    public class BuildReport
    {
        private HttpClient HttpClient { get; } = new HttpClient();

        private int MaxRange = 1;

        [Benchmark]
        public void Local()
        {
            //Parallel.ForEach(
            //    Enumerable.Range(1, MaxRange).ToArray(),
            //    x =>
            //    {
            //        var invoiceBuilder = new InvoiceBuilder
            //        {
            //            Init = () =>
            //            {
            //            },
            //            GetRequestStream = () => new MemoryStream(Properties.Resources.Request),
            //            GetTemplateStream = (template) => new MemoryStream(Properties.Resources.Invoice),
            //            Output = (pdf) =>
            //            {
            //                using (var stream = new MemoryStream())
            //                {
            //                    stream.Write(pdf, 0, pdf.Length);
            //                }
            //            }
            //        };
            //        invoiceBuilder.Build();
            //    });
            var invoiceBuilder = new InvoiceBuilder
            {
                Init = () =>
                {
                },
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

        }

        //[Benchmark]
        //public void LocalFunctions()
        //{
        //    Parallel.ForEach(
        //        Enumerable.Range(1, MaxRange).ToArray(),
        //        x => HttpClient.GetAsync("http://localhost:7071/api/Function01").GetAwaiter().GetResult());
        //}

        //[Benchmark]
        //public void LocalAppService()
        //{
        //    Parallel.ForEach(
        //        Enumerable.Range(1, MaxRange).ToArray(),
        //        x => HttpClient.GetAsync("https://localhost:44341/api/values").GetAwaiter().GetResult());
        //}

        [Benchmark]
        public void RemoteAppService()
        {
            //Parallel.ForEach(
            //    Enumerable.Range(1, MaxRange).ToArray(),
            //    x => HttpClient.GetAsync("http://ddt2.azurewebsites.net/api/values").GetAwaiter().GetResult());
            HttpClient.GetAsync("http://ddt2.azurewebsites.net/api/values").GetAwaiter().GetResult();
        }

        //[Benchmark]
        //public void RemoteFunctions()
        //{
        //    Parallel.ForEach(
        //        Enumerable.Range(1, MaxRange).ToArray(),
        //        x => HttpClient.GetAsync("https://ddt2functions.azurewebsites.net/api/Function01?code=tf3SIcLLjy3RobgimzCvhr7QXa6c8q8oRGKzKHrLrFDfoOZ05kUVBQ==").GetAwaiter().GetResult());
        //}
    }
}
