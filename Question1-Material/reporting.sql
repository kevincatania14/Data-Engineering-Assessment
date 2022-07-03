use assessment_db;
go

--b
select top (10) Amount, c.Email, b.Name, Date
from dbo.[Transaction] t
join dbo.Customer c
on t.CustomerId = c.Id
join dbo.Brand b
on c.BrandId = b.Id
where TransactionTypeId = (select Id 
	from dbo.TransactionType
	where Name = 'Deposit')
order by t.amount desc;
go

--c
select b.Name BrandName, ps.Name PaymentStatus, count(*) TotalNumber, sum(t.Amount) AmountTotal
from dbo.[Transaction] t
join dbo.Customer c
on t.CustomerId = c.Id
join dbo.Brand b
on c.BrandId = b.Id
join dbo.PaymentStatus ps
on t.PaymentStatusId = ps.Id
where t.PaymentStatusId = (select Id
	from dbo.PaymentStatus
	where Name = 'Failed')
and TransactionTypeId = (select Id 
	from dbo.TransactionType
	where Name = 'Deposit')
group by b.Name, ps.Name;
go

--d
select b.Name BrandName, p.Name ProductName, sum(t.TurnoverEUR) TurnoverEURTotal, sum(t.TotalAccountingRevenue) TotalAccountingRevenue
from dbo.[Transaction] t
join dbo.Customer c
on t.CustomerId = c.Id
join dbo.Brand b
on c.BrandId = b.Id
join dbo.Product p
on t.ProductId = p.Id
where t.Date between '2021-01-01' and '2021-01-06'
group by b.Name, p.Name;
go

--e
select p.Name ProductName, avg(t.GameWinEUR) AverageGameWinEUR
from dbo.[Transaction] t
join dbo.Product p
on t.ProductId = p.Id
group by p.Name;
go

--f
select c.Id, c.Email, sum(t.TurnoverEUR) TotalTurnoverEUR
from dbo.[Transaction] t
join dbo.Customer c
on t.CustomerId = c.id
group by c.Id, c.Email
having sum(t.TurnoverEUR) > 500;
go