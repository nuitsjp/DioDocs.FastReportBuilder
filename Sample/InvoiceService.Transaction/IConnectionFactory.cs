using System.Data;

namespace InvoiceService.Transaction
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}