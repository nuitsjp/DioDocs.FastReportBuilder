using System.Collections.Generic;

namespace InvoiceService.Repository
{
    public interface ISalesOrderRepository
    {
        IEnumerable<SalesOrder> Get();
    }
}