using Castle.DynamicProxy;

namespace InvoiceService.Transaction
{
    public class TransactionInterceptor : IInterceptor
    {
        private readonly ITransactionContext _transactionContext;
        private readonly IConnectionFactory _connectionFactory;

        public TransactionInterceptor(ITransactionContext transactionContext, IConnectionFactory connectionFactory)
        {
            _transactionContext = transactionContext;
            _connectionFactory = connectionFactory;
        }

        public void Intercept(IInvocation invocation)
        {
            using (var connection = _connectionFactory.Create())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    _transactionContext.Connection = connection;
                    _transactionContext.Transaction = transaction;

                    invocation.Proceed();

                    transaction.Commit();
                }
            }
        }
    }
}