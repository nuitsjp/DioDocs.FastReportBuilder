using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpoint = "https://invoicefunction.azurewebsites.net/api/CreateReport?code=aDV2o2YaMYC3ly5xaid4NW3Mh3fXdsq1J5ZC53YkxQqxYb6DfWm7UA==";
            var jsonContent = 
                new StreamContent(File.Open("Request.json", FileMode.Open));

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(endpoint,jsonContent);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
