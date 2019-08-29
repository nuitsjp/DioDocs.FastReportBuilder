using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InvoiceService.Function.ReportBuilder;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService.Function.AppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
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

            return invoiceBuilder.MeasurementResults.Select(x => $"{x.Item1}:{x.Item2}").ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
