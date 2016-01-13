use Lelands
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_BidWatch]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_BidWatch]
GO
--
create procedure [dbo].[spBid_View_BidWatch]
	@user_id bigint, @event_id bigint
as
begin
	set nocount on;	
	select a.ID as Auction_ID, 
		a.Lot, 
		a.Title, 
		a.Status as AuctionStatus,
		'not_set_in_query' as HighBidder,
		ar.CurrentBid,		
		ar.Bids,
		b.Amount,
		b.MaxBid, 
		1 as Quantity,
		cast(case when IsNull(b.ID,-1)=-1 then 2 else case when b.[user_id]=ar.[User_ID] then 1 else 0 end end as tinyint) as IsWatch,
		e.Title as EventTitle,
		c.Title as CategoryTitle,
		mc.Name as MainCategoryTitle
	from dbo.Auction a
		join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID=a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID
		join dbo.BidWatchCurrent bw on bw.Auction_ID=a.ID
		left join dbo.BidCurrent b on b.Auction_ID=a.ID and b.[User_ID]=@user_id and b.IsActive=1
		left join dbo.AuctionResultsCurrent ar on ar.Auction_ID=a.ID
		--left join dbo.[User] u on u.ID=ar.[User_ID]			
	where (IsNull(@event_id,-1)=-1 or e.ID=@event_id) and bw.[User_ID]=@user_id
	order by a.Lot
end
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_GetPastBidWatch]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_GetPastBidWatch]
GO
--
create procedure [dbo].[spBid_View_GetPastBidWatch]
	@user_id bigint, @event_id bigint	
as
begin
	select a.ID, 
		a.Lot, 
		a.Title, 
		a.IsUnsold,
		a.Status as AuctionStatus,
		b.Amount, 
		b.MaxBid, 
		b.DateMade, 
		CAST(ISNULL(i.Amount,0) as money) as WinBid,
		CAST(IsNUll(i.Cost,0) as money) as PriceRealized,
		cast(case when IsNull(i.[User_id],0)=@user_id then 1 else 0 end as bit) as IsWinner,		
		img.ThumbNailPath as ThumbnailPath,
		e.Title as EventTitle,
		mc.Name as MainCategoryTitle,
		c.Title as CategoryTitle,
		COUNT(bl.ID) as BidsCount	
	from Auction a
		join Bid b on b.Auction_ID=a.ID		
		left join Invoice i on i.Auction_ID=a.ID and i.UserInvoices_ID is not null
		join [Event] e on e.ID = a.Event_ID
		join [EventCategory] ec on ec.ID = a.Category_ID
		join [MainCategories] mc on mc.ID = ec.MainCategory_ID
		join [Category] c on c.ID = ec.Category_ID
		left join [Image] img on img.Auction_ID=a.ID and img.[Default]=1		
		left join BidLog bl on bl.Auction_ID=a.ID and bl.IsProxyRaise=0
	where a.Event_ID=@event_id 
		and b.[User_ID]=@user_id
	group by a.ID, a.Lot, a.Title, a.IsUnsold, a.Status, b.Amount, b.MaxBid, b.DateMade, ISNULL(i.Amount,0),
		IsNUll(i.Cost,0), case when IsNull(i.[User_id],0)=@user_id then 1 else 0 end,		
		img.ThumbNailPath, e.Title, mc.Name, c.Title
	order by a.Lot
end
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------