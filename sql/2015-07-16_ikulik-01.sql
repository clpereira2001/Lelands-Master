use Lelands
GO
------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM dbo.Tags WHERE ID=2)
begin
	SET IDENTITY_INSERT [dbo].Tags ON
	insert into dbo.Tags (ID, Title, IsSystem, IsViewable) values (2, 'Sales', 1, 1)
	SET IDENTITY_INSERT [dbo].Tags OFF
end
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_Sales]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spAuction_View_Sales
GO
--
create procedure [dbo].spAuction_View_Sales
	@eventID bigint, @tagID bigint	
as
begin
	set nocount on;		
	select
		a.ID as Auction_ID,
		e.Title as EventTitle,
		mc.Name as MainCategoryTitle,
		c.Title as CategoryTitle,		
		a.Lot,						
		IsNull(img.ThumbNailPath,'') as ThumbnailPath,
		a.Title,
		a.Description
	from dbo.Auction a join dbo.AuctionTags at on at.AuctionID = a.ID and at.TagID =@tagID		
		join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID = a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID		
		left join dbo.[Image] img on img.Auction_ID=a.ID and img.[Default]=1		
	where e.ID = @eventID				
end
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------