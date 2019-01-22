using System.Diagnostics;
using System.Net.Mime;
using InvoiceService.UseCase;
using Microsoft.AspNetCore.Mvc;
using InvoiceService.Web.Models;

namespace InvoiceService.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBuildInvoice _buildInvoice;

        public HomeController(IBuildInvoice buildInvoice)
        {
            _buildInvoice = buildInvoice;
        }

        public IActionResult Index()
        {
            return View(_buildInvoice.GetSalesOrders());
        }

        public IActionResult BuildReport([FromQuery]int salesOrderId)
        {
            var report = _buildInvoice.Build(salesOrderId);
            return File(report, MediaTypeNames.Application.Pdf, "Invoice.pdf");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
