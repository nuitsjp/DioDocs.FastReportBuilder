// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using InvoiceService.Function.ReportBuilder;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceService.Function
{
    public static class CreateReportFunction
    {
        private static readonly string BlobStorageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [FunctionName("CreateReportFunction")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream input,
            [Queue("diodocsqueue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> msg,
            ILogger log)
        {
            var createdEvent = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();
            CloudBlobClient blobClient = null;
            var invoiceBuilder = new InvoiceBuilder
            {
                Init = () =>
                {
                    var storageAccount = CloudStorageAccount.Parse(BlobStorageConnectionString);
                    blobClient = storageAccount.CreateCloudBlobClient();
                },
                GetRequestStream = () => input,
                GetTemplateStream = (template) =>
                {
                    var container = blobClient.GetContainerReference("excel");
                    var blockBlob = container.GetBlockBlobReference(template);
                    return blockBlob.OpenReadAsync().GetAwaiter().GetResult();
                },
                Output = (pdf) =>
                {
                    var container = blobClient.GetContainerReference("pdf");

                    var uri = new Uri(createdEvent.Url);
                    var cloudBlob = new CloudBlob(uri);

                    var blobName = cloudBlob.Name;
                    var blockBlob = container.GetBlockBlobReference(blobName + ".pdf");
                    blockBlob.UploadFromByteArrayAsync(pdf, 0, pdf.Length).GetAwaiter().GetResult();
                }
            };
            invoiceBuilder.Build();

            msg.Add($"Completed. {invoiceBuilder}");

        }
    }
}
