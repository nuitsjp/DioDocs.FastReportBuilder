using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DioDocs.FastReportBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceFunction
{
    public static class CreateReport
    {
        [FunctionName("CreateReport")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("templates/Invoice.xlsx", FileAccess.Read)]Stream template,
            [Blob("reports", FileAccess.Write)] CloudBlobContainer outputContainer,
            ILogger log)
        {
            await outputContainer.CreateIfNotExistsAsync();

            var fileName = $"{Guid.NewGuid()}.pdf";
            var cloudBlockBlob = outputContainer.GetBlockBlobReference(fileName);

            var builder = new JsonReportBuilder(template);                  // 1. 引数の「template」Streamからビルダーを生成

            using (var input = new StreamReader(req.Body, Encoding.UTF8))   // 2. HTTP RequestボディからTextReaderを生成
            using (var output = await cloudBlockBlob.OpenWriteAsync())
            {
                builder.Build(input, output, SaveFileFormat.Pdf);           // 3. JsonReportBuilder
            }

            return new OkObjectResult(fileName);
        }
    }
}
