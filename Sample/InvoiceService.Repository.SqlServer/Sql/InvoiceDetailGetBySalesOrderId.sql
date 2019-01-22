select
	OrderQuantity,
	UnitPrice,
	ProductName
from
	Invoice.InvoiceDetail
where
	SalesOrderId = @SalesOrderId
