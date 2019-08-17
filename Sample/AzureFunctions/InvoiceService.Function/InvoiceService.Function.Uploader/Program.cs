using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace InvoiceService.Function.Uploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage - .NET quickstart sample\n");

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ddt2functions;AccountKey=DV73/VK4nbPgyst/DDN2Kliw169Sr5E/F0d8C2SQSIbCJ0W0+bEzZhPYNG4CR7jLQWss/tNjQUylSijycCQieg==;EndpointSuffix=core.windows.net");
            var blobClient = storageAccount.CreateCloudBlobClient();

            Console.WriteLine("Clear containers.");
            ClearContainer(blobClient, "pdf");
            ClearContainer(blobClient, "json");

            Console.WriteLine("Begin upload json.");
            var json = File.ReadAllBytes("request.json");
            var prefix = DateTime.Now.Ticks.ToString();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Parallel.ForEach(
                Enumerable.Range(1, 100),
                x => ProcessAsync(blobClient, prefix, x, json).GetAwaiter().GetResult());
            stopwatch.Stop();
            Console.WriteLine($"Completed. Elapsed:{stopwatch.Elapsed}");
        }

        private static void ClearContainer(CloudBlobClient blobClient, string containerName)
        {
            var container = blobClient.GetContainerReference(containerName);
            var blobs = container.ListBlobs().ToArray();
            var count = blobs.Count();
            foreach (var blob in blobs)
            {
                var cloudBlob = new CloudBlob(blob.Uri);
                container.GetBlobReference(cloudBlob.Name).DeleteIfExists();
            }
        }

        private static Task ProcessAsync(CloudBlobClient blobClient, string prefix, int index, byte[] json)
        {
            var container = blobClient.GetContainerReference("json");
            var blockBlob = container.GetBlockBlobReference($"{prefix}{index:D10}" + ".json");
            return blockBlob.UploadFromByteArrayAsync(json, 0, json.Length);
        }
    }
}
