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
            //Console.WriteLine("Please press any key.");
            //Console.ReadLine();

            var summary = BenchmarkRunner.Run<InvoiceServiceBenchmark>();

            //Console.WriteLine("Completed. Please press any key.");
            //Console.ReadLine();
        }
    }

    public class InvoiceServiceBenchmark
    {
        private static HttpClient HttpClient { get; } = new HttpClient();

        private void Run(string url)
        {
            HttpClient.GetAsync(url).GetAwaiter().GetResult();
        }

        //[Benchmark]
        //public void OnLocal()
        //{
        //    var invoiceBuilder = new InvoiceBuilder
        //    {
        //        Init = () =>
        //        {
        //        },
        //        GetRequestStream = () => new MemoryStream(Properties.Resources.Request),
        //        GetTemplateStream = (template) => new MemoryStream(Properties.Resources.Invoice),
        //        Output = (pdf) => Stream.Null.Write(pdf, 0, pdf.Length)
        //    };
        //    invoiceBuilder.Build();
        //}

        [Benchmark]
        public void OnConsumptionPlan()
        {
            Run("https://ddt2consumption.azurewebsites.net/api/Function01?code=NWcagqU7clZLprhOq7hAEJ/46FnDqf3FLkeLEjapiwS5XmM9NIojvg==");
        }

        [Benchmark]
        public void OnAppServicePlan()
        {
            Run("https://ddt2-app-service-functions.azurewebsites.net/api/Function01?code=V9mNBl1tqo9jV3fpkVEtqjP5aSgdyQXNVtYndjhgSXxZXLY140Tbaw==");
        }
    }

    [SimpleJob(targetCount: 50)]
    public class BuildReport
    {
        private HttpClient HttpClient { get; } = new HttpClient();

        private int MaxRange = 1;

        //[Benchmark]
        //public void Local()
        //{
        //    //Parallel.ForEach(
        //    //    Enumerable.Range(1, MaxRange).ToArray(),
        //    //    x =>
        //    //    {
        //    //        var invoiceBuilder = new InvoiceBuilder
        //    //        {
        //    //            Init = () =>
        //    //            {
        //    //            },
        //    //            GetRequestStream = () => new MemoryStream(Properties.Resources.Request),
        //    //            GetTemplateStream = (template) => new MemoryStream(Properties.Resources.Invoice),
        //    //            Output = (pdf) =>
        //    //            {
        //    //                using (var stream = new MemoryStream())
        //    //                {
        //    //                    stream.Write(pdf, 0, pdf.Length);
        //    //                }
        //    //            }
        //    //        };
        //    //        invoiceBuilder.Build();
        //    //    });
        //    var invoiceBuilder = new InvoiceBuilder
        //    {
        //        Init = () =>
        //        {
        //        },
        //        GetRequestStream = () => new MemoryStream(Properties.Resources.Request),
        //        GetTemplateStream = (template) => new MemoryStream(Properties.Resources.Invoice),
        //        Output = (pdf) =>
        //        {
        //            using (var stream = new MemoryStream())
        //            {
        //                stream.Write(pdf, 0, pdf.Length);
        //            }
        //        }
        //    };
        //    invoiceBuilder.Build();

        //}

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

        //[Benchmark]
        //public void RemoteAppService()
        //{
        //    //Parallel.ForEach(
        //    //    Enumerable.Range(1, MaxRange).ToArray(),
        //    //    x => HttpClient.GetAsync("http://ddt2.azurewebsites.net/api/values").GetAwaiter().GetResult());
        //    HttpClient.GetAsync("http://ddt2.azurewebsites.net/api/values").GetAwaiter().GetResult();
        //}

        [Benchmark]
        public void RemoteFunctions()
        {
            //Parallel.ForEach(
            //    Enumerable.Range(1, MaxRange).ToArray(),
            //    x => HttpClient.GetAsync("https://ddt2functions.azurewebsites.net/api/Function01?code=tf3SIcLLjy3RobgimzCvhr7QXa6c8q8oRGKzKHrLrFDfoOZ05kUVBQ==").GetAwaiter().GetResult());
            HttpClient
                .GetAsync("https://ddt2-app-service-functions.azurewebsites.net/api/Function01?code=V9mNBl1tqo9jV3fpkVEtqjP5aSgdyQXNVtYndjhgSXxZXLY140Tbaw==")
                .GetAwaiter().GetResult();
        }
    }
}
