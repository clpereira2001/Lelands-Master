use Lelands
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGetTagsForEvent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spGetTagsForEvent
GO
--
create procedure [dbo].spGetTagsForEvent
	@event_id bigint=-1, @status bit = 0 /*0-125, 1-3*/
as
begin
	set nocount on;
	select t.ID, t.Title, COUNT(at.AuctionID) as AuctionCount	
	from dbo.[Event] e join dbo.Auction a on a.Event_ID=e.ID
	join dbo.AuctionTags at on at.AuctionID = a.ID
	join dbo.Tags t on t.ID = at.TagID
	where ((IsNull(@event_id,-1)=-1 and e.IsCurrent=1) or e.ID=@event_id)				
		and ISNULL(a.Lot,-1)<>-1
		and a.AuctionType_ID<>3
		and t.IsViewable = 1
		and ((@status=0 and a.[Status] in (1,2,5)) or a.[Status]=3)	
	group by t.ID, t.Title
end
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_Tag]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].spAuction_View_Tag
GO
--
create procedure [dbo].spAuction_View_Tag
	@eventID bigint, @tagID bigint,
	@statusmode int=-1 /*-1~1235, 1-125, 2-3 */,
	@sort int=0, @order bit=0, @pageindex int=0, @pagesize int =20, @totalamount int output
as
begin
	set nocount on;
	--select 
	--	CAST(0 as bigint) as Auction_ID,
	--	CAST(' ' as varchar(510)) as EventTitle,
	--	CAST(' ' as varchar(200)) as MainCategoryTitle,
	--	CAST(' ' as varchar(510)) as CategoryTitle,
	--	CAST (0 as int) as Bids,
	--	CAST (0 as money) as CurrentBid,
	--	CAST(' ' as varchar(100)) as Estimate,
	--	CAST(0 as bit) as IsBold,
	--	CAST(0 as bit) as IsFeatured,
	--	CAST(0 as bit) as IsUnsold,
	--	CAST(0 as bit) as IsPulledOut,
	--	CAST(0 as smallint) as Lot,
	--	CAST(0 as money) as Price,
	--	CAST(0 as money) as PriceRealized, 
	--	CAST(0 as tinyint) as [AuctionStatus],
	--	CAST(' ' as varchar(255)) as ThumbnailPath,
	--	CAST(' ' as varchar(512)) as Title		
	--	set @totalamount = 0;
	
	create table #result(RN int not null identity(1,1), Auction_ID bigint not null);
	if (@order=0) begin
		if (@sort<>1) begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID and e.ID=@eventID
					join dbo.AuctionTags at on at.AuctionID = a.ID and at.TagID = @tagID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID					
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1					
					and ISNULL(a.Lot,-1)<>-1
					and ((@statusmode=-1 and a.[Status] in (1,2,3,5)) or (@statusmode=1 and ((e.CloseStep=0 and a.[Status] in (1,2,5)) or (e.CloseStep=1 and a.[status] in (1,2,3,5))))or (@statusmode=2 and a.[Status]=3))
				order by case @sort 					
					when 2 then IsNUll(case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end,a.Price)
					when 3 then IsNull(case when e.IsCurrent=1 then arc.Bids else ar.Bids end,0)
					else IsNull(a.Lot,0) end asc, a.Event_ID desc
		end else begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID and e.ID=@eventID
					join dbo.AuctionTags at on at.AuctionID = a.ID and at.TagID = @tagID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1					
					and ISNULL(a.Lot,-1)<>-1
					and ((@statusmode=-1 and a.[Status] in (1,2,3,5)) or (@statusmode=1 and ((e.CloseStep=0 and a.[Status] in (1,2,5)) or (e.CloseStep=1 and a.[status] in (1,2,3,5))))or (@statusmode=2 and a.[Status]=3))
				order by a.Title asc
		end
	end else begin
		if (@sort<>1) begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID and e.ID=@eventID
					join dbo.AuctionTags at on at.AuctionID = a.ID and at.TagID = @tagID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID					
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1					
					and ISNULL(a.Lot,-1)<>-1
					and ((@statusmode=-1 and a.[Status] in (1,2,3,5)) or (@statusmode=1 and ((e.CloseStep=0 and a.[Status] in (1,2,5)) or (e.CloseStep=1 and a.[status] in (1,2,3,5))))or (@statusmode=2 and a.[Status]=3))
				order by case @sort 					
					when 2 then IsNUll(case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end,a.Price)
					when 3 then IsNull(case when e.IsCurrent=1 then arc.Bids else ar.Bids end,0)
					else IsNull(a.Lot,0) end desc, a.Event_ID desc
		end else begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID and e.ID=@eventID
					join dbo.AuctionTags at on at.AuctionID = a.ID and at.TagID = @tagID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1					
					and ISNULL(a.Lot,-1)<>-1
					and ((@statusmode=-1 and a.[Status] in (1,2,3,5)) or (@statusmode=1 and ((e.CloseStep=0 and a.[Status] in (1,2,5)) or (e.CloseStep=1 and a.[status] in (1,2,3,5))))or (@statusmode=2 and a.[Status]=3))
				order by a.Title desc
		end
	end
		
	select
		a.ID as Auction_ID,
		e.Title as EventTitle,
		mc.Name as MainCategoryTitle,
		c.Title as CategoryTitle,
		ISNULL(case when e.IsCurrent=1 then arc.Bids else ar.Bids end, 0) as Bids,
		IsNull(case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, IsNUll(i.Amount, 0)) as CurrentBid,
		a.Estimate,
		a.IsBold,
		a.IsFeatured,
		a.IsUnsold,
		IsNUll(a.PulledOut,0) as IsPulledOut,
		a.Lot,
		a.Price,
		IsNUll(i.Cost,0) as PriceRealized,
		a.[Status] as [AuctionStatus],
		IsNull(img.ThumbNailPath,'') as ThumbnailPath,
		a.Title
	from #result r
		join dbo.Auction a on a.ID = r.Auction_ID
		left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID					
		left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
		join dbo.[Event] e on e.ID = a.Event_ID
		join dbo.[EventCategory] ec on ec.ID = a.Category_ID
		join dbo.[MainCategories] mc on mc.ID = ec.MainCategory_ID
		join dbo.[Category] c on c.ID = ec.Category_ID
		left join dbo.Invoice i on i.Auction_ID=a.ID and i.UserInvoices_ID is not null
		left join dbo.[Image] img on img.Auction_ID=a.ID and img.[Default]=1		
	where r.RN between
		case when @pageindex=0 then @pageindex*@pagesize else @pageindex*@pagesize+1 end
		and @pageindex*@pagesize+@pagesize
	order by r.RN
			
	select @totalamount = COUNT(*) from #result;
	drop table #result
end
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------