use Lelands
GO
------------------------------------------------------------------
update dbo.CommissionRate set LongDescription = '15% of the final hammer price for each lot exceeding $1500, 20% for each lot selling for a hammer price from $500 to $1500, and 25% for each lot of a hammer price of less than $500' where RateID = 0
update dbo.CommissionRate set LongDescription = '0% for each lot' where RateID = 1
update dbo.CommissionRate set LongDescription = '1% of the final hammer price for each lot' where RateID = 2
update dbo.CommissionRate set LongDescription = '2% of the final hammer price for each lot' where RateID = 3
update dbo.CommissionRate set LongDescription = '2.5% of the final hammer price for each lot' where RateID = 4
update dbo.CommissionRate set LongDescription = '3% of the final hammer price for each lot' where RateID = 5
update dbo.CommissionRate set LongDescription = '4.41% of the final hammer price for each lot' where RateID = 6
update dbo.CommissionRate set LongDescription = '5% of the final hammer price for each lot' where RateID = 7
update dbo.CommissionRate set LongDescription = '6% of the final hammer price for each lot' where RateID = 8
update dbo.CommissionRate set LongDescription = '7% of the final hammer price for each lot' where RateID = 9
update dbo.CommissionRate set LongDescription = '7.5% of the final hammer price for each lot' where RateID = 10
update dbo.CommissionRate set LongDescription = '8% of the final hammer price for each lot' where RateID = 11
update dbo.CommissionRate set LongDescription = '8.5% of the final hammer price for each lot' where RateID = 12
update dbo.CommissionRate set LongDescription = '10% of the final hammer price for each lot' where RateID = 13
update dbo.CommissionRate set LongDescription = '11% of the final hammer price for each lot' where RateID = 14
update dbo.CommissionRate set LongDescription = '12% of the final hammer price for each lot' where RateID = 15
update dbo.CommissionRate set LongDescription = '12.5% of the final hammer price for each lot' where RateID = 16
update dbo.CommissionRate set LongDescription = '13% of the final hammer price for each lot' where RateID = 17
update dbo.CommissionRate set LongDescription = '14% of the final hammer price for each lot' where RateID = 18
update dbo.CommissionRate set LongDescription = '15% of the final hammer price for each lot' where RateID = 19
update dbo.CommissionRate set LongDescription = '19% of the final hammer price for each lot' where RateID = 20
update dbo.CommissionRate set LongDescription = '20% of the final hammer price for each lot' where RateID = 21
update dbo.CommissionRate set LongDescription = '25% of the final hammer price for each lot' where RateID = 22
update dbo.CommissionRate set LongDescription = '30% of the final hammer price for each lot' where RateID = 23
update dbo.CommissionRate set LongDescription = '41.25% of the final hammer price for each lot' where RateID = 24
update dbo.CommissionRate set LongDescription = '50% of the final hammer price for each lot' where RateID = 25
update dbo.CommissionRate set LongDescription = '55% of the final hammer price for each lot' where RateID = 26
update dbo.CommissionRate set LongDescription = '90% of the final hammer price for each lot' where RateID = 27
update dbo.CommissionRate set LongDescription = '-5% of the final hammer price for each lot' where RateID = 28
update dbo.CommissionRate set LongDescription = '-2.5% of the final hammer price for each lot' where RateID = 29
update dbo.CommissionRate set LongDescription = '8.75% of the final hammer price for each lot' where RateID = 30
update dbo.CommissionRate set LongDescription = '9% of the final hammer price for each lot' where RateID = 31
update dbo.CommissionRate set LongDescription = '40% of the final hammer price for each lot' where RateID = 32
go
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------