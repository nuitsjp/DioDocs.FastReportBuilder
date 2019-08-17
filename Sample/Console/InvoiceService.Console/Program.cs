﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DioDocs.FastReportBuilder;

namespace InvoiceService.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Workbook.SetLicenseKey(Properties.Settings.Default.DioDocsForExcelKey);

            using (var invoiceStream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.InvoiceXml)))
            using (var template = new MemoryStream(Properties.Resources.Invoice))
            using (var outputStream = new FileStream("result.pdf", FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(Invoice));
                var invoice = (Invoice)serializer.Deserialize(invoiceStream);

                var reportBuilder =
                    new ReportBuilder<InvoiceDetail>(template)
                        // 単一項目のSetterを設定
                        .AddSetter("$SalesOrderId", cell => cell.Value = invoice.SalesOrderId)
                        .AddSetter("$OrderDate", cell => cell.Value = invoice.OrderDate)
                        .AddSetter("$CompanyName", cell => cell.Value = invoice.CompanyName)
                        .AddSetter("$Name", cell => cell.Value = invoice.Name)
                        .AddSetter("$Address", cell => cell.Value = invoice.Address)
                        .AddSetter("$PostalCode", cell => cell.Value = invoice.PostalCode)
                        // テーブルのセルに対するSetterを設定
                        .AddTableSetter("$ProductName", (cell, detail) => cell.Value = detail.ProductName)
                        .AddTableSetter("$UnitPrice", (cell, detail) => cell.Value = detail.UnitPrice)
                        .AddTableSetter("$OrderQuantity", (cell, detail) => cell.Value = detail.OrderQuantity);

                reportBuilder.Build(invoice.InvoiceDetails, outputStream, SaveFileFormat.Pdf);
            }
        }
    }

    public class Invoice
    {
        public int SalesOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; } = new List<InvoiceDetail>();
    }

    public class InvoiceDetail
    {
        public int OrderQuantity { get; set; }
        public int UnitPrice { get; set; }
        public string ProductName { get; set; }
    }
}
