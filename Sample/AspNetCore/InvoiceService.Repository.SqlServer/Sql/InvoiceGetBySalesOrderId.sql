SELECT
	SalesOrderId,
	OrderDate,
	CompanyName,
	Name,
	Address,
	PostalCode
FROM
	Invoice.Invoice
WHERE
	SalesOrderId = @SalesOrderId