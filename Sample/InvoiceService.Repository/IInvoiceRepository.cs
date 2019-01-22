namespace InvoiceService.Repository
{
    public interface IInvoiceRepository
    {
        Invoice Get(int salesOrderId);
    }
}