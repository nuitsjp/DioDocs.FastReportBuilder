select
	SalesOrderId,
	OrderDate,
	CompanyName,
	Name,
	DetailCount,
	TotalPrice
from
	Invoice.SalesOrder
order by
	OrderDate desc, SalesOrderId desc