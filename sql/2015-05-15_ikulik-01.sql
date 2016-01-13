use Lelands
GO
------------------------------------------------------------------
update dbo.Specialists set IsActive = 0 where ID in (165, 185, 186, 187, 188)
go
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fAuction_GetTags]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].fAuction_GetTags
GO
--
CREATE function [dbo].fAuction_GetTags(@auction_id bigint)
returns VARCHAR(8000)
AS
BEGIN
DECLARE @Result VARCHAR(8000)
SET @Result = ''
	declare @t table(res VARCHAR(8000))
	insert into @t(res)
	select t.Title+','
	from dbo.AuctionTags at join dbo.Tags t on t.ID = at.TagID
	where at.AuctionID = @auction_id	
	select @Result = @Result + res from @t	
	return case when IsNUll(@Result,'')<>'' then substring(@Result, 1, len(@Result)-1) else '' end
END
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Auctions_GetAuctionListForPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_Auctions_GetAuctionListForPage]
GO
--
create procedure [dbo].[sp_Auctions_GetAuctionListForPage]
	 @auction_id bigint=null, @evnt bigint=null, @lot varchar(10)=null, @status int=null, @maincatory int=null, @category varchar(50)=null, @title varchar(50)=null, @price decimal=null, @reserve decimal=null, @commrate int=null, @seller varchar(50)=null, @shipping decimal=null, @prioritydescription int=null, @oldauction_id bigint=null, @enteredby varchar(50)=null, @specialist varchar(50)=null, @highbidder varchar(50)=null, @currentbid varchar(50)=null, @tag_id bigint, @orderby tinyint
as
begin	
	select distinct auc.ID as Auction_ID
	     , ev.Title as [Event]
	     , auc.Lot
	     , ast.Title as [Status]
	     , mc.Name as MainCategory
	     , cat.Title as Category
	     , auc.Title
	     , auc.Price
	     , auc.Reserve
	     , auc.Estimate
	     , auc.Quantity
	     , auc.StartDate 
	     , auc.EndDate
	     , cr.[Description] as CommRate
	     , uO.[Login] as Seller
	     , auc.Shipping
	     , auc.PriorityDescription 
	     , auc.LOA
	     , auc.PulledOut
	     , auc.IsUnsold
	     , case when auc.OldAuction_ID is null then old.InventoryID else auc.OldAuction_ID end as OldAuction_ID
	     , auc.NotifiedOn
	     , uS.[Login] as EnteredBy
	     , sp.FirstName + ' ' +sp.LastName as Specialist
	     , case when ev.IsCurrent=1 then  uHc.[Login] else uH.[Login] end as HighBidder
	     , CAST(case when ev.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end as varchar(100)) as CurrentBid 
	     , 0 as Rate	
	     , dbo.fAuction_GetTags(auc.ID) as Tags
	into #Result
	from dbo.Auction auc
		left join dbo.[AuctionResults] ar on ar.Auction_ID=auc.id		
		left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=auc.id		
		left join dbo.[User] uH on uH.ID=ar.[User_ID]
		left join dbo.[User] uHc on uHc.ID=arc.[User_ID]
		join dbo.[Event] ev on ev.ID=auc.Event_ID
		join dbo.AuctionStatuses ast on ast.Id=auc.[Status]
		join dbo.[EventCategory] ec on ec.ID=auc.Category_ID
		join dbo.MainCategories mc on mc.ID=ec.MainCategory_ID
		join dbo.Category cat on cat.ID=ec.Category_ID
		join dbo.CommissionRate cr on cr.RateID=auc.CommissionRate_ID
		join dbo.[User] uO on uO.ID=auc.Owner_ID
		left join dbo.[User] uS on uS.ID=auc.EnteredBy
		join dbo.[Consignments] con on con.Event_ID=auc.Event_ID and con.[User_ID]=auc.Owner_ID
		left join dbo.[Specialists] sp on sp.ID =con.Specialist_ID		
		left join dbo.[Old_New_AuctionIDs] old on old.AuctionId = auc.Event_ID and old.ID_New = auc.ID
		left join dbo.AuctionTags at on at.AuctionID = auc.ID	
	where (@auction_id is null or auc.ID like '' + cast(@auction_id as varchar) + '%')
		and (@evnt is null or ev.id like '' + CAST(@evnt as varchar) + '%')
		and (ISNULL(auc.Lot,'') like '%'+ISNULL(@lot,'')+'%')
		and (@status is null or auc.[Status]=@status)
		and (@maincatory is null or ec.MainCategory_ID=@maincatory)
		and (ISNULL(cat.Title,'') like '%'+ISNULL(@category,'')+'%')
		and (ISNULL(auc.Title,'') like '%'+ISNULL(@title, '')+'%')
		and (@price is null or auc.Price=@price)
		and (@reserve is null or auc.Reserve=@reserve)
		and (@commrate is null or auc.CommissionRate_ID=@commrate)
		and (IsNUll(uO.[Login],'') like '%'+ISNULL(@seller, '')+'%')
		and (@shipping is null or IsNull(auc.Shipping,0)=@shipping)		
		and (@prioritydescription is null or auc.Priority=@prioritydescription)
		and (@oldauction_id is null or case when auc.OldAuction_ID is null then old.InventoryID else auc.OldAuction_ID end=@oldauction_id)
		and (ISNULL(uS.[Login],'') like '%'+IsNUll(@enteredby,'')+'%')
		and (ISNULL(sp.FirstName + ' ' +sp.LastName,'') like '%'+ISNULL(@specialist,'')+'%')
		and (ISNULL(uh.[Login],'') like '%'+ISNULL(@highbidder,'')+'%')
		and (ISNULL(ar.CurrentBid,'') like '%'+ISNULL(@currentbid,'')+'%')	
		and (@tag_id is null or at.TagID = @tag_id)
	if (@orderby=0) select * from #Result order by rate desc, Auction_ID desc
	else if (@orderby=1) select * from #Result order by rate desc, Auction_ID
	else if (@orderby=2) select * from #Result order by rate desc, [Event] desc, Auction_ID
	else if (@orderby=3) select * from #Result order by rate desc, [Event], Auction_ID
	else if (@orderby=4) select * from #Result order by rate desc, Lot desc, Auction_ID
	else if (@orderby=5) select * from #Result order by rate desc, Lot, Auction_ID
	else if (@orderby=6) select * from #Result order by rate desc, [Status] desc, Auction_ID
	else if (@orderby=7) select * from #Result order by rate desc, [Status], Auction_ID
	else if (@orderby=8) select * from #Result order by rate desc, MainCategory desc, Auction_ID
	else if (@orderby=9) select * from #Result order by rate desc, MainCategory, Auction_ID
	else if (@orderby=10) select * from #Result order by rate desc, Category desc, Auction_ID
	else if (@orderby=11) select * from #Result order by rate desc, Category, Auction_ID
	else if (@orderby=12) select * from #Result order by rate desc, Title desc, Auction_ID
	else if (@orderby=13) select * from #Result order by rate desc, Title, Auction_ID	
	else if (@orderby=14) select * from #Result order by rate desc, Reserve desc, Auction_ID
	else if (@orderby=15) select * from #Result order by rate desc, Reserve, Auction_ID
	else if (@orderby=16) select * from #Result order by rate desc, Estimate desc, Auction_ID
	else if (@orderby=17) select * from #Result order by rate desc, Estimate, Auction_ID
	else if (@orderby=18) select * from #Result order by rate desc, Quantity desc, Auction_ID
	else if (@orderby=19) select * from #Result order by rate desc, Quantity, Auction_ID	
	else if (@orderby=20) select * from #Result order by rate desc, StartDate desc, Auction_ID
	else if (@orderby=21) select * from #Result order by rate desc, StartDate, Auction_ID
	else if (@orderby=22) select * from #Result order by rate desc, EndDate desc, Auction_ID
	else if (@orderby=23) select * from #Result order by rate desc, EndDate, Auction_ID
	else if (@orderby=24) select * from #Result order by rate desc, CommRate desc, Auction_ID
	else if (@orderby=25) select * from #Result order by rate desc, CommRate, Auction_ID
	else if (@orderby=26) select * from #Result order by rate desc, Seller desc, Auction_ID
	else if (@orderby=27) select * from #Result order by rate desc, Seller, Auction_ID
	else if (@orderby=28) select * from #Result order by rate desc, Shipping desc, Auction_ID
	else if (@orderby=29) select * from #Result order by rate desc, Shipping, Auction_ID
	else if (@orderby=30) select * from #Result order by rate desc, PriorityDescription desc, Auction_ID
	else if (@orderby=31) select * from #Result order by rate desc, PriorityDescription, Auction_ID
	else if (@orderby=32) select * from #Result order by rate desc, LOA desc, Auction_ID
	else if (@orderby=33) select * from #Result order by rate desc, LOA, Auction_ID
	else if (@orderby=34) select * from #Result order by rate desc, PulledOut desc, Auction_ID
	else if (@orderby=35) select * from #Result order by rate desc, PulledOut, Auction_ID
	else if (@orderby=36) select * from #Result order by rate desc, IsUnsold desc, Auction_ID
	else if (@orderby=37) select * from #Result order by rate desc, IsUnsold, Auction_ID
	else if (@orderby=38) select * from #Result order by rate desc, OldAuction_ID desc, Auction_ID
	else if (@orderby=39) select * from #Result order by rate desc, OldAuction_ID, Auction_ID
	else if (@orderby=40) select * from #Result order by rate desc, NotifiedOn desc, Auction_ID
	else if (@orderby=41) select * from #Result order by rate desc, NotifiedOn, Auction_ID
	else if (@orderby=42) select * from #Result order by rate desc, EnteredBy desc, Auction_ID
	else if (@orderby=43) select * from #Result order by rate desc, EnteredBy, Auction_ID
	else if (@orderby=44) select * from #Result order by rate desc, Specialist desc, Auction_ID
	else if (@orderby=45) select * from #Result order by rate desc, Specialist, Auction_ID	
	else if (@orderby=51) select * from #Result order by rate desc, Price, Auction_ID
	else select * from #Result order by rate desc, Price desc, Auction_ID	
	
	drop table #Result
end
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------