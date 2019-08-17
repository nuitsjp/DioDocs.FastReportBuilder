using System;
using System.Collections.Generic;

namespace InvoiceService.Repository
{
    public class Invoice
    {
        public int SalesOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public IList<InvoiceDetail> InvoiceDetails { get; } = new List<InvoiceDetail>();
    }
}