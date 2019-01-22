using System.Data;

namespace InvoiceService.Transaction
{
    public interface ITransactionContext
    {
        IDbConnection Connection { get; set; }
        IDbTransaction Transaction { get; set; }
    }
}
