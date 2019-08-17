using System;
using System.IO;
using System.Threading.Tasks;
using InvoiceService.Function.ReportBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace InvoiceService.Function
{
    public static class Function01
    {
        [FunctionName("Function01")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

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

            return new OkObjectResult(invoiceBuilder.ToString());
        }
    }
}
