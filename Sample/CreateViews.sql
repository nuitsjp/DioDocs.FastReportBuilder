DROP VIEW IF EXISTS Invoice.InvoiceDetail;
DROP VIEW IF EXISTS Invoice.Invoice;
DROP VIEW IF EXISTS Invoice.SalesOrder;
DROP VIEW IF EXISTS Invoice.SalesOrderSummary;
DROP SCHEMA IF EXISTS Invoice;
GO


CREATE SCHEMA Invoice;
GO

CREATE VIEW Invoice.SalesOrderSummary
AS
SELECT
	SalesOrderID,
	COUNT(SalesOrderID) AS DetailCount,
	SUM(LineTotal) AS TotalPrice
FROM
	SalesLT.SalesOrderDetail
GROUP BY
	SalesOrderID
GO

CREATE VIEW Invoice.SalesOrder
AS
SELECT
	SalesOrderHeader.SalesOrderID AS SalesOrderId,
	SalesOrderHeader.OrderDate,
	Customer.CompanyName,
	Customer.FirstName + ' ' + Customer.LastName AS Name,
	DetailCount,
	TotalPrice
FROM
	SalesLT.SalesOrderHeader
	INNER JOIN SalesLT.Customer
		ON SalesOrderHeader.CustomerID = Customer.CustomerID
	INNER JOIN SalesLT.Address
		ON SalesOrderHeader.BillToAddressID = Address.AddressID
	INNER JOIN Invoice.SalesOrderSummary
		ON SalesOrderHeader.SalesOrderID = SalesOrderSummary.SalesOrderID
GO

CREATE VIEW Invoice.Invoice
AS
SELECT
	SalesOrderHeader.SalesOrderID AS SalesOrderId,
	SalesOrderHeader.OrderDate,
	Customer.CompanyName,
	Customer.FirstName + ' ' + Customer.LastName AS Name,
	AddressLine1 + ' ' + AddressLine2 + ' ' + City + ' ' + StateProvince AS Address,
	PostalCode
FROM
	SalesLT.SalesOrderHeader
	INNER JOIN SalesLT.Customer
		ON SalesOrderHeader.CustomerID = Customer.CustomerID
	INNER JOIN SalesLT.Address
		ON SalesOrderHeader.BillToAddressID = Address.AddressID
GO


CREATE VIEW Invoice.InvoiceDetail
AS
SELECT
	SalesOrderID,
	SalesOrderDetailID,
	OrderQty AS OrderQuantity,
	UnitPrice,
	Product.Name AS ProductName
FROM
	SalesLT.SalesOrderDetail
	INNER JOIN SalesLT.Product
		ON	SalesOrderDetail.ProductID = Product.ProductID	
GO