use Lelands
go
-------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_AuctionBids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_AuctionBids]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_View_AuctionBids]
	@event_id bigint, @lot bigint, @title varchar(100), @currentbid money, @bid int,
	@sort varchar(100)='lot', @order bit=0,
	@pageindex int=0, @pagesize int=20, @totalamount int output
as
begin
	set nocount on;
	--select 
	--	CAST(0 as bigint) as Auction_ID,
	--	CAST(0 as bigint) as Lot,
	--	CAST(' ' as varchar(512)) as Title,		
	--	CAST(0 as money) as CurrentBid,
	--	CAST(0 as money) as MaxBid,
	--	CAST(0 as int) as Bids
	--set @totalamount = 0;
	
	create table #result(RN int not null identity(1,1), Auction_ID bigint, Lot bigint, Title varchar(512), CurrentBid money, MaxBid money,Bids int);
	
	if (@order=0) begin
		if (@sort='title') begin
			insert into #result(Auction_ID, Lot, Title, CurrentBid, MaxBid, Bids)
			select a.ID, a.Lot, a.Title, case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, case when e.IsCurrent=1 then arc.MaxBid else ar.MaxBid end, case when e.IsCurrent=1 then arc.Bids else ar.Bids end
			from dbo.Auction a
				join dbo.[Event] e on e.ID = a.Event_ID
				left join dbo.AuctionResults ar on ar.Auction_ID=a.ID
				left join dbo.AuctionResultsCurrent arc on arc.Auction_ID=a.ID
			where (ISNULL(@event_id,-1)=-1 or a.Event_ID=@event_id)
				and (ISNULL(@lot,-1)=-1 or a.Lot=@lot)
				and (ISNULL(@currentbid,-1)=-1 or ((IsNUll(arc.CurrentBid,-1)=@currentbid and e.IsCurrent=1) or (IsNUll(ar.CurrentBid,-1)=@currentbid and e.IsCurrent=0) )) 
				and (ISNULL(@bid,-1)=-1 or ((IsNUll(arc.Bids,-1)=@bid and e.IsCurrent=1) or (IsNUll(ar.Bids,-1)=@bid and e.IsCurrent=0) )) 
				and a.Title like '%'+ISNULL(@title,'')+'%'
				and a.AuctionType_ID<3
				and a.[Status] in (2,3)
			order by a.Title asc
		end else begin
			insert into #result(Auction_ID, Lot, Title, CurrentBid, MaxBid, Bids)
			select a.ID, a.Lot, a.Title, case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, case when e.IsCurrent=1 then arc.MaxBid else ar.MaxBid end, case when e.IsCurrent=1 then arc.Bids else ar.Bids end
			from dbo.Auction a
				join dbo.[Event] e on e.ID = a.Event_ID
				left join dbo.AuctionResults ar on ar.Auction_ID=a.ID
				left join dbo.AuctionResultsCurrent arc on arc.Auction_ID=a.ID
			where (ISNULL(@event_id,-1)=-1 or a.Event_ID=@event_id)
				and (ISNULL(@lot,-1)=-1 or a.Lot=@lot)
				and (ISNULL(@currentbid,-1)=-1 or ((IsNUll(arc.CurrentBid,-1)=@currentbid and e.IsCurrent=1) or (IsNUll(ar.CurrentBid,-1)=@currentbid and e.IsCurrent=0) )) 
				and (ISNULL(@bid,-1)=-1 or ((IsNUll(arc.Bids,-1)=@bid and e.IsCurrent=1) or (IsNUll(ar.Bids,-1)=@bid and e.IsCurrent=0) )) 
				and a.Title like '%'+ISNULL(@title,'')+'%'
				and a.AuctionType_ID<3
				and a.[Status] in (2,3)
			order by case @sort 
				when 'lot' then a.Lot
				when 'bidnumber' then case when e.IsCurrent=1 then arc.Bids else ar.Bids end
				else case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end end asc, a.Title asc
		end
	end else begin
		if (@sort='title') begin
			insert into #result(Auction_ID, Lot, Title, CurrentBid, MaxBid, Bids)
			select a.ID, a.Lot, a.Title, case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, case when e.IsCurrent=1 then arc.MaxBid else ar.MaxBid end, case when e.IsCurrent=1 then arc.Bids else ar.Bids end
			from dbo.Auction a
				join dbo.[Event] e on e.ID = a.Event_ID
				left join dbo.AuctionResults ar on ar.Auction_ID=a.ID
				left join dbo.AuctionResultsCurrent arc on arc.Auction_ID=a.ID
			where (ISNULL(@event_id,-1)=-1 or a.Event_ID=@event_id)
				and (ISNULL(@lot,-1)=-1 or a.Lot=@lot)
				and (ISNULL(@currentbid,-1)=-1 or ((IsNUll(arc.CurrentBid,-1)=@currentbid and e.IsCurrent=1) or (IsNUll(ar.CurrentBid,-1)=@currentbid and e.IsCurrent=0) )) 
				and (ISNULL(@bid,-1)=-1 or ((IsNUll(arc.Bids,-1)=@bid and e.IsCurrent=1) or (IsNUll(ar.Bids,-1)=@bid and e.IsCurrent=0) )) 
				and a.Title like '%'+ISNULL(@title,'')+'%'
				and a.AuctionType_ID<3
				and a.[Status] in (2,3)
			order by a.Title desc
		end else begin
			insert into #result(Auction_ID, Lot, Title, CurrentBid, MaxBid, Bids)
			select a.ID, a.Lot, a.Title, case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, case when e.IsCurrent=1 then arc.MaxBid else ar.MaxBid end, case when e.IsCurrent=1 then arc.Bids else ar.Bids end
			from dbo.Auction a
				join dbo.[Event] e on e.ID = a.Event_ID
				left join dbo.AuctionResults ar on ar.Auction_ID=a.ID
				left join dbo.AuctionResultsCurrent arc on arc.Auction_ID=a.ID
			where (ISNULL(@event_id,-1)=-1 or a.Event_ID=@event_id)
				and (ISNULL(@lot,-1)=-1 or a.Lot=@lot)
				and (ISNULL(@currentbid,-1)=-1 or ((IsNUll(arc.CurrentBid,-1)=@currentbid and e.IsCurrent=1) or (IsNUll(ar.CurrentBid,-1)=@currentbid and e.IsCurrent=0) )) 
				and (ISNULL(@bid,-1)=-1 or ((IsNUll(arc.Bids,-1)=@bid and e.IsCurrent=1) or (IsNUll(ar.Bids,-1)=@bid and e.IsCurrent=0) )) 
				and a.Title like '%'+ISNULL(@title,'')+'%'
				and a.AuctionType_ID<3
				and a.[Status] in (2,3)
			order by case @sort 
				when 'lot' then a.Lot
				when 'bidnumber' then case when e.IsCurrent=1 then arc.Bids else ar.Bids end
				else case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end end desc, a.Title asc
		end
	end
	
	select r.*	
	from #result r	
		--left join dbo.[User] u1 on u1.ID=r.User_ID_1
		--left join dbo.[User] u2 on u2.ID=r.User_ID_2
		--left join dbo.[UserTypes] ut1 on ut1.Id=u1.UserType
		--left join dbo.[UserTypes] ut2 on ut2.Id=u2.UserType		
	where r.RN between
			case when @pageindex=0 then @pageindex*@pagesize else @pageindex*@pagesize+1 end
			and @pageindex*@pagesize+@pagesize
	order by r.RN
	
	select @totalamount = COUNT(*) from #result;	
	drop table #result;
end
GO
----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------
