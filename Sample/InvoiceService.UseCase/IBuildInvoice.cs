using System.Collections.Generic;

namespace InvoiceService.UseCase
{
    public interface IBuildInvoice
    {
        IList<SalesOrder> GetSalesOrders();

        byte[] Build(int salesOrderId);
    }
}
