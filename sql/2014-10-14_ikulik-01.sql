use Lelands
GO
------------------------------------------------------------------
if not exists(select * from sys.columns where Name = N'LongDescription' and Object_ID = Object_ID(N'CommissionRate'))    
begin
	alter table dbo.CommissionRate add LongDescription nvarchar(2000)	
end
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConsignmentContracts]') AND type in (N'U'))
DROP TABLE [dbo].ConsignmentContracts
GO
--
CREATE TABLE [dbo].ConsignmentContracts(	
	ConsignmentID bigint not null,	
	StatusID int not null,
	ContractText nvarchar(max) not null,
	FileName nvarchar(512) not null	
)
go 
alter table dbo.ConsignmentContracts add constraint PK_CONSIGNMENTCONTRACTS primary key (ConsignmentID)
alter table dbo.ConsignmentContracts add constraint FK_CONSIGNMENTCONTRACT_CONSIGNMENT foreign key (ConsignmentID) references dbo.Consignments (ID) on delete cascade
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spInvoice_View_GetConsignmentByConsignments]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spInvoice_View_GetConsignmentByConsignments]
GO
--
create procedure [dbo].[spInvoice_View_GetConsignmentByConsignments]
	@consignment_id bigint
as
begin
	set nocount on;
	select 
		con.ID as Consignment_ID,
		con.ConsDate,
		i.ID as Invoice_ID,
		a.ID as Auction_ID,
		a.Lot,
		a.Title,
		a.Reserve,
		IsNUll(i.Amount, 0) as Amount,
		IsnUll(i.Cost, 0) as Cost,
		cr.[Description] as CommRate,
		e.Title as EventTitle,
		mc.Name as MainCategoryTitle,
		c.Title as CategoryTitle
	from dbo.Consignments con
		join dbo.Auction a on a.Owner_ID=con.[User_ID] and a.Event_ID=con.Event_ID
		left join dbo.Invoice i on a.ID = i.Auction_ID and i.Consignments_ID=con.Id		
		join dbo.CommissionRate cr on cr.RateID=a.CommissionRate_ID
		join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID = a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID
	where con.Id = @consignment_id
	order by case when IsNUll(i.ID,-1)=-1 then 0 else 1 end desc, a.Lot, a.Title
end
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------