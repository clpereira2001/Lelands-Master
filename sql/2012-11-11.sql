USE [Lelands]
GO
----------------------------------------------------------------------------------------------------------------
If not exists(select * from Information_Schema.Columns where Table_Name='BidLog' and Column_Name='IsAutoBid')
begin
	alter table BidLog add IsAutoBid bit not null constraint DF_BIDLOG_ISAUTOBID default(0)
end
go
--------------------------------------------------------------------------------------------------
If not exists(select * from Information_Schema.Columns where Table_Name='BidLogCurrent' and Column_Name='IsAutoBid')
begin
	alter table BidLogCurrent add IsAutoBid bit not null constraint DF_BIDLOGCURRENT_ISAUTOBID default(0)
end
go
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BidWatchCurrent_AUCTION]') AND parent_object_id = OBJECT_ID(N'[dbo].[BidWatchCurrent]'))
ALTER TABLE [dbo].[BidWatchCurrent] DROP CONSTRAINT [FK_BidWatchCurrent_AUCTION]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BidWatchCurrent_USER]') AND parent_object_id = OBJECT_ID(N'[dbo].[BidWatchCurrent]'))
ALTER TABLE [dbo].[BidWatchCurrent] DROP CONSTRAINT [FK_BidWatchCurrent_USER]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BidWatchCurrent]') AND type in (N'U'))
DROP TABLE [dbo].[BidWatchCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BidWatchCurrent](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Auction_ID] [bigint] NOT NULL,
	[User_ID] [bigint] NOT NULL,
 CONSTRAINT [PK_BidWatchCurrent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [fgBidding_2]
) ON [fgBidding_2]

GO

ALTER TABLE [dbo].[BidWatchCurrent]  WITH CHECK ADD  CONSTRAINT [FK_BidWatchCurrent_AUCTION] FOREIGN KEY([Auction_ID])
REFERENCES [dbo].[Auction] ([ID])
GO

ALTER TABLE [dbo].[BidWatchCurrent] CHECK CONSTRAINT [FK_BidWatchCurrent_AUCTION]
GO

ALTER TABLE [dbo].[BidWatchCurrent]  WITH CHECK ADD  CONSTRAINT [FK_BidWatchCurrent_USER] FOREIGN KEY([User_ID])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[BidWatchCurrent] CHECK CONSTRAINT [FK_BidWatchCurrent_USER]
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AuctionResultsCurrent_AUCTION]') AND parent_object_id = OBJECT_ID(N'[dbo].[AuctionResultsCurrent]'))
ALTER TABLE [dbo].[AuctionResultsCurrent] DROP CONSTRAINT [FK_AuctionResultsCurrent_AUCTION]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_AuctionResultsCurrent_USER]') AND parent_object_id = OBJECT_ID(N'[dbo].[AuctionResultsCurrent]'))
ALTER TABLE [dbo].[AuctionResultsCurrent] DROP CONSTRAINT [FK_AuctionResultsCurrent_USER]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuctionResultsCurrent]') AND type in (N'U'))
DROP TABLE [dbo].[AuctionResultsCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AuctionResultsCurrent](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Auction_ID] [bigint] NOT NULL,
	[User_ID] [bigint] NULL,
	[CurrentBid] [money] NULL,
	[Bids] [int] NULL,
	[MaxBid] [money] NULL,
 CONSTRAINT [PK_AuctionResultsCurrent] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [fgAuctions_1]
) ON [fgAuctions_1]

GO

ALTER TABLE [dbo].[AuctionResultsCurrent]  WITH CHECK ADD  CONSTRAINT [FK_AuctionResultsCurrent_AUCTION] FOREIGN KEY([Auction_ID])
REFERENCES [dbo].[Auction] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AuctionResultsCurrent] CHECK CONSTRAINT [FK_AuctionResultsCurrent_AUCTION]
GO

ALTER TABLE [dbo].[AuctionResultsCurrent]  WITH CHECK ADD  CONSTRAINT [FK_AuctionResultsCurrent_USER] FOREIGN KEY([User_ID])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[AuctionResultsCurrent] CHECK CONSTRAINT [FK_AuctionResultsCurrent_USER]
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_BidWatch]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_BidWatch]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_View_BidWatch]
	@user_id bigint, @event_id bigint
as
begin
	set nocount on;	
	select a.ID as Auction_ID, 
		a.Lot, 
		a.Title, 
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
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSelect_BidWatchCurrent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spSelect_BidWatchCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spSelect_BidWatchCurrent]
	@Auction_ID bigint, @user_id bigint
as
begin
	set nocount on;
	SELECT *
	FROM [dbo].[BidWatchCurrent]
	where Auction_id=IsNUll(@Auction_ID,-1)
		and [USER_ID]=ISNULL(@user_id, -1)
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spInsert_BidWatchCurrent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spInsert_BidWatchCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spInsert_BidWatchCurrent]
	@ID [bigint] output, @Auction_ID [bigint], @User_ID [bigint]
as
begin
	set nocount on;
	INSERT INTO [dbo].[BidWatchCurrent]([Auction_ID],[User_ID]) VALUES (@Auction_ID, @User_ID)
	set @ID = CAST(SCOPE_IDENTITY() as bigint)
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spDelete_BidWatchCurrent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spDelete_BidWatchCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spDelete_BidWatchCurrent]
	@ID [bigint]
as
begin
	set nocount on;
	delete from dbo.BidWatchCurrent where ID=@ID;
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Event_SetCurrent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_Event_SetCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_Event_SetCurrent] 
	@event_id bigint
as
begin
	set nocount on;
	if exists (select * from dbo.[Event] where IsCurrent=1 and ID=@event_id) return;
	update dbo.[Event] set IsCurrent=1, IsViewable=1, IsClickable=1, CloseStep=0 where ID=@event_id;
	update dbo.[Auction] set StartDate=GETDATE(), [Status]=2, DateSold=null where Event_ID=@event_id and [Status]=1 and Lot is not null;
	update dbo.[Auction] set [Status]=5 where Event_ID=@event_id and [Status]=1 and Lot is null;	
	
	delete B from dbo.BidWatch b join dbo.Auction a on a.ID=b.Auction_ID where a.Event_ID=@event_id;	
	delete ar from dbo.AuctionResults ar join dbo.Auction a on a.ID=ar.Auction_ID where a.Event_ID=@event_id	
	
	delete ar from dbo.AuctionResultsCurrent ar join dbo.Auction a on a.ID=ar.Auction_ID where a.Event_ID=@event_id	
	insert into dbo.AuctionResultsCurrent(Auction_ID) select ID from dbo.Auction where Event_ID=@event_id and [Status]=2
	
	delete B from dbo.Bid b join dbo.Auction a on a.ID=b.Auction_ID where a.Event_ID=@event_id;
	delete B from dbo.BidLog b join dbo.Auction a on a.ID=b.Auction_ID where a.Event_ID=@event_id;
	
	delete B from dbo.BidCurrent b join dbo.Auction a on a.ID=b.Auction_ID where a.Event_ID=@event_id;
	delete B from dbo.BidLogCurrent b join dbo.Auction a on a.ID=b.Auction_ID where a.Event_ID=@event_id;
	
	delete I from dbo.Invoice i join dbo.UserInvoices ui on ui.Id=i.UserInvoices_ID where ui.Event_ID=@event_id;
	delete P from dbo.Payment p join dbo.UserInvoices ui on ui.Id=p.UserInvoices_ID where ui.Event_ID=@event_id;	
	delete I from dbo.Invoice i join dbo.Consignments c on c.Id = i.Consignments_ID where c.Event_ID=@event_id;
	delete from dbo.UserInvoices where Event_ID=@event_id;
	
	delete from dbo.PastEventsBiddingStatistic where Event_ID=@event_id
end;
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_WinningBid]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_WinningBid]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_WinningBid]
	@auction_id bigint
as
begin
	set nocount on;
	--select * from [dbo].[f_Bid_GetWinner](@auction_id)
	select top(1) * from BidCurrent 
	where Auction_id=@auction_id and IsActive=1
		order by Amount desc, MaxBid desc, DateMade asc
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_LotBids]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_LotBids]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_LotBids]
	@auction_id bigint
as
begin
	set nocount on;
	select *
	from dbo.BidCurrent
	where Auction_ID=IsNUll(@Auction_ID,-1)
	order by Amount desc, MaxBid desc, DateMade asc, ID asc
end
GO
--------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_PlaceBidsForSingleBidding]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_PlaceBidsForSingleBidding]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_PlaceBidsForSingleBidding]
	@auction_id bigint,
	@current_bid_id bigint output,
	@current_user_id bigint,
	@current_amount money,
	@current_maxbid money,
	@current_datemade datetime,
	@current_ip nvarchar(15),
	@current_isproxy bit,
	@current_isproxyrase bit,
	@current_isactive bit,	
	@update_amount money,
	@update_maxbid money,
	@update_datemade datetime,
	@prev_bid_id bigint,
	@prev_user_id bigint,
	@prev_amount money,
	@prev_maxbid money,	
	@prev_datemade datetime,
	@prev_datemadeforlot datetime,
	@prev_ip nvarchar(15),
	@prev_isproxy bit,
	@winner_user_id bigint,
	@winner_amount money,
	@winner_maxbid money
as
begin
	set nocount on;
	declare @bidamount bigint = 0;
	-- place current user's bid - bid
	if (ISNULL(@current_bid_id,-1)=-1) begin
		insert into dbo.BidCurrent(Auction_ID, [User_ID], Amount, MaxBid, DateMade, IsProxy, IsActive, IP, Quantity)
			values(@auction_id, @current_user_id, IsNUll(@update_amount, @current_amount), IsNUll(@update_maxbid, @current_maxbid), IsNull(@update_datemade, @current_datemade), @current_isproxy, @current_isactive, @current_ip, 1)
		set @current_bid_id = CAST(SCOPE_IDENTITY() as bigint);		
	end
	else begin
	  update dbo.BidCurrent
	  set Amount=IsNUll(@update_amount, @current_amount), MaxBid=IsNUll(@update_maxbid, @current_maxbid), IsProxy=@current_isproxy, IsActive=@current_isactive, IP=@current_ip, DateMade=IsNull(@update_datemade, @current_datemade)
		where ID=@current_bid_id
	end
		
	-- place current user's bid - bid log
	insert into dbo.BidLogCurrent(Auction_ID, [User_ID], Amount, MaxBid, DateMade, IsProxy, IsProxyRaise, IP, Quantity, IsAutoBid)
		values(@auction_id, @current_user_id, @current_amount, @current_maxbid, @current_datemade, @current_isproxy, @current_isproxyrase, @current_ip, 1, 0)
	
	if (@current_isproxyrase=0) begin set @bidamount = @bidamount + 1; end
	
	--place updated user's bid - bid log
	if (ISNULL(@update_amount,-1) <> -1	) begin
		insert into dbo.BidLogCurrent(Auction_ID, [User_ID], Amount, MaxBid, DateMade, IsProxy, IsProxyRaise, IP, Quantity, IsAutoBid)
			values(@auction_id, @current_user_id, @update_amount, @update_maxbid, @update_datemade, @current_isproxy, 0, @current_ip, 1, 1)
		set @bidamount = @bidamount + 1;
	end
	
	--update previous user bids
	if (ISNULL(@prev_bid_id,-1)<>-1) begin
		update dbo.BidCurrent set Amount=@prev_amount, DateMade=@prev_datemade where ID=@prev_bid_id
		insert into dbo.BidLogCurrent(Auction_ID, [User_ID], Amount, MaxBid, DateMade, IsProxy, IsProxyRaise, IP, Quantity, IsAutoBid)
			values(@auction_id, @prev_user_id, @prev_amount, @prev_maxbid, @prev_datemadeforlot, @prev_isproxy, 0, @prev_ip, 1, 1)
		set @bidamount = @bidamount + 1;
	end
	
	--update lot bidding result
	if (ISNULL(@winner_user_id, 0)>0)
		update dbo.AuctionResultsCurrent set [User_ID]=@winner_user_id, CurrentBid=@winner_amount, MaxBid=@winner_maxbid, Bids = Isnull(Bids,0) + @bidamount where Auction_ID=@auction_id		
end

GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_List]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spAuction_View_List]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spAuction_View_List]
	@event_id bigint=-1, @category_id bigint=-1,
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
					join dbo.[Event] e on e.ID = a.Event_ID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID					
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))					
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
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
					join dbo.[Event] e on e.ID = a.Event_ID			
				where a.AuctionType_ID<>3					
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))					
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
					and ISNULL(a.Lot,-1)<>-1
					and ((@statusmode=-1 and a.[Status] in (1,2,3,5)) or (@statusmode=1 and ((e.CloseStep=0 and a.[Status] in (1,2,5)) or (e.CloseStep=1 and a.[status] in (1,2,3,5))))or (@statusmode=2 and a.[Status]=3))
				order by a.Title asc
		end
	end else begin
		if (@sort<>1) begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID					
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
				where a.AuctionType_ID<>3					
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))					
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
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
					join dbo.[Event] e on e.ID = a.Event_ID			
				where a.AuctionType_ID<>3					
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))					
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
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
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Auctions_GetAuctionListForPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_Auctions_GetAuctionListForPage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_Auctions_GetAuctionListForPage]
	 @auction_id bigint=null, @evnt bigint=null, @lot varchar(10)=null, @status int=null, @maincatory int=null, @category varchar(50)=null, @title varchar(50)=null, @price decimal=null, @reserve decimal=null, @commrate int=null, @seller varchar(50)=null, @shipping decimal=null, @prioritydescription int=null, @oldauction_id bigint=null, @enteredby varchar(50)=null, @specialist varchar(50)=null, @highbidder varchar(50)=null, @currentbid varchar(50)=null , @orderby tinyint
as
begin
	select
		auc.ID as Auction_ID, ev.Title as [Event], auc.Lot, ast.Title as [Status], mc.Name as MainCategory,
		cat.Title as Category, auc.Title, auc.Price, auc.Reserve,	auc.Estimate, auc.Quantity, auc.StartDate,
		auc.EndDate, cr.[Description] as CommRate, uO.[Login] as Seller, auc.Shipping, auc.PriorityDescription,
		auc.LOA, auc.PulledOut, auc.IsUnsold, 
		case when auc.OldAuction_ID is null then old.InventoryID else auc.OldAuction_ID end as OldAuction_ID,
		auc.NotifiedOn, uS.[Login] as EnteredBy, sp.FirstName + ' ' +sp.LastName as Specialist, case when ev.IsCurrent=1 then  uHc.[Login] else uH.[Login] end as HighBidder, CAST(case when ev.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end as varchar(100)) as CurrentBid,
		--case when auc.ID = @auction_id then 10 else 0 end +
		--case when auc.id like '' + CAST(@auction_id as varchar) + '%' then 1 else 0 end +
		--case when ev.ID = @evnt then 10 else 0 end +
		--case when ev.id like '' + CAST(@evnt as varchar) + '%' then 1 else 0 end +
		--case when cat.Title = @category and isnull(@category,'') <> '' then 10 else 0 end +
		--case when cat.Title like '' + @category + '%' and isnull(@category,'') <> '' then 1 else 0 end +
		--case when auc.Title = @title and isnull(@title,'') <> '' then 10 else 0 end +
		--case when auc.Title like '' + @title + '%' then 1 else 0 end +
		--case when uO.[Login] = @seller then 10 else 0 end +
		--case when uO.[Login] like '' + @seller+'%' then 1 else 0 end +
		--case when uS.[Login] = @enteredby then 10 else 0 end +
		--case when uS.[Login] like '' + @enteredby+'%' then 1 else 0 end +
		--case when ISNULL(sp.FirstName + ' ' +sp.LastName,'') = @specialist and ISNULL(sp.FirstName + ' ' +sp.LastName,'') <> '' and ISNULL(@specialist,'') <> '' then 10 else 0 end +
		--case when ISNULL(sp.FirstName + ' ' +sp.LastName,'') like '%'+ISNULL(@specialist,'')+'%' then 1 else 0 end +
		--case when auc.HighBidder = @highbidder and ISNULL(auc.HighBidder,'') <> '' and ISNULL(@highbidder,'') <> '' then 10 else 0 end +
		--case when (ISNULL(auc.HighBidder,'') like ''+ISNULL(@highbidder,'')+'%') then 1 else 0 end +
		--case when ar.CurrentBid = @currentbid and ISNULL(ar.CurrentBid,'') <> '' and ISNULL(@currentbid,'') <> '' then 10 else 0 end +
		--case when ISNULL(auc.CurrentBid,'') like ''+ISNULL(@currentbid,'')+'%' then 1 else 0 end as rate
		0 as Rate	
	into #Result
	from Auction auc
		left join [AuctionResults] ar on ar.Auction_ID=auc.id		
		left join [AuctionResultsCurrent] arc on arc.Auction_ID=auc.id		
		left join [User] uH on uH.ID=ar.[User_ID]
		left join [User] uHc on uHc.ID=arc.[User_ID]
		join [Event] ev on ev.ID=auc.Event_ID
		join AuctionStatuses ast on ast.Id=auc.[Status]
		join [EventCategory] ec on ec.ID=auc.Category_ID
		join MainCategories mc on mc.ID=ec.MainCategory_ID
		join Category cat on cat.ID=ec.Category_ID
		join CommissionRate cr on cr.RateID=auc.CommissionRate_ID
		join [User] uO on uO.ID=auc.Owner_ID
		left join [User] uS on uS.ID=auc.EnteredBy
		join [Consignments] con on con.Event_ID=auc.Event_ID and con.[User_ID]=auc.Owner_ID		
		left join [Specialists] sp on sp.ID =con.Specialist_ID		
		left join [Old_New_AuctionIDs] old on old.AuctionId = auc.Event_ID and old.ID_New = auc.ID
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
	--else if (@orderby=46) select * from #Result order by rate desc, HighBidder desc, Auction_ID
	--else if (@orderby=47) select * from #Result order by rate desc, HighBidder, Auction_ID	
	--else if (@orderby=48) select * from #Result order by rate desc, CurrentBid desc, Auction_ID
	--else if (@orderby=49) select * from #Result order by rate desc, CurrentBid, Auction_ID	
	else if (@orderby=51) select * from #Result order by rate desc, Price, Auction_ID
	else select * from #Result order by rate desc, Price desc, Auction_ID	
	
	drop table #Result
end

GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_Search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spAuction_View_Search]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spAuction_View_Search]
	@event_id bigint=-1, @lot varchar(4)='', @title varchar(100)='', @description varchar(100)='', @category_id bigint=-1, @owner_id bigint=-1, @sort int=0, @order bit=0, @pageindex int=0, @pagesize int =20, @totalamount int output
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
					join dbo.[Event] e on e.ID = a.Event_ID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID
				where a.AuctionType_ID<>3
					and a.[Status] in (1,2,3,5)
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))
					and CAST(a.Lot as varchar(4)) like '%'+ISNULL(@lot,'')+'%'
					and a.Title like '%'+ISNULL(@title,'')+'%'
					and a.[Description] like '%'+ISNULL(@description,'')+'%'
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
					and (ISNULL(@owner_id,-1)=-1 or a.Owner_ID=@owner_id)
				order by case @sort 					
					when 2 then IsNUll(case when e.iscurrent=1 then arc.CurrentBid else ar.CurrentBid end,a.Price)
					when 3 then IsNull(case when e.iscurrent=1 then arc.Bids else ar.Bids end, 0)
					else IsNull(a.Lot,0) end asc, a.Event_ID desc
		end else begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID			
				where a.AuctionType_ID<>3
					and a.[Status] in (1,2,3,5)
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))
					and CAST(a.Lot as varchar(4)) like '%'+ISNULL(@lot,'')+'%'
					and a.Title like '%'+ISNULL(@title,'')+'%'
					and a.[Description] like '%'+ISNULL(@description,'')+'%'
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
					and (ISNULL(@owner_id,-1)=-1 or a.Owner_ID=@owner_id)
				order by a.Title asc
		end
	end else begin
		if (@sort<>1) begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID
					left join dbo.[AuctionResults] ar on ar.Auction_ID=a.ID
					left join dbo.[AuctionResultsCurrent] arc on arc.Auction_ID=a.ID		
				where a.AuctionType_ID<>3
					and a.[Status] in (1,2,3,5)
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))
					and CAST(a.Lot as varchar(4)) like '%'+ISNULL(@lot,'')+'%'
					and a.Title like '%'+ISNULL(@title,'')+'%'
					and a.[Description] like '%'+ISNULL(@description,'')+'%'
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
					and (ISNULL(@owner_id,-1)=-1 or a.Owner_ID=@owner_id)
				order by case @sort 					
					when 2 then IsNUll(case when e.iscurrent=1 then arc.CurrentBid else ar.CurrentBid end,a.Price)
					when 3 then IsNull(case when e.iscurrent=1 then arc.Bids else ar.Bids end, 0)
					else IsNull(a.Lot,0) end desc, a.Event_ID desc
		end else begin
			insert into #result(Auction_ID)
				select a.ID
				from dbo.Auction a
					join dbo.[Event] e on e.ID = a.Event_ID			
				where a.AuctionType_ID<>3
					and a.[Status] in (1,2,3,5)
					and e.IsViewable=1
					and (IsNUll(@event_id,-1)=-1 or a.Event_ID=IsNUll(@event_id,-1))
					and CAST(a.Lot as varchar(4)) like '%'+ISNULL(@lot,'')+'%'
					and a.Title like '%'+ISNULL(@title,'')+'%'
					and a.[Description] like '%'+ISNULL(@description,'')+'%'
					and (ISNULL(@category_id,-1)=-1 or a.Category_ID=@category_id)
					and (ISNULL(@owner_id,-1)=-1 or a.Owner_ID=@owner_id)
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
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAuction_View_DetailResult]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spAuction_View_DetailResult]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spAuction_View_DetailResult]
	@auction_id bigint = -1
as
begin
	set nocount on;
	select
		a.ID as Auction_ID,
		a.Price,
		a.Estimate,
		ISNULL(case when e.IsCurrent=1 then arc.Bids else ar.Bids end, 0) as Bids,
		IsNull(case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end, IsNUll(i.Amount, 0)) as CurrentBid,
		IsNUll(i.Cost,0) as PriceRealized,
		a.IsUnsold,
		e.DateEnd as EventDateEnd
	from dbo.Auction a
		left join dbo.AuctionResults ar on ar.Auction_ID=a.ID
		left join dbo.AuctionResultsCurrent arc on arc.Auction_ID=a.ID
		left join dbo.Invoice i on i.Auction_ID=a.ID and i.UserInvoices_ID is not null		
		join dbo.[Event] e on e.ID = a.Event_ID
	where a.ID = @auction_id
	order by a.ID, IsNull(case when e.IsCurrent=1 then arc.CurrentBid else ar.CurrentBid end,0) desc
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_BidLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_BidLog]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_View_BidLog]
	@event_id bigint, @pageindex int=0, @pagesize int=20, @totalamount int output
as
begin
	set nocount on;
	--select 
	--	CAST(0 as bigint) as Bid_ID,
	--	CAST(0 as bigint) as Lot,
	--	CAST(' ' as varchar(512)) as Title,		
	--	CAST(' ' as varchar(100)) as Bidder,
	--	CAST(0 as money) as Amount,
	--	CAST(0 as money) as MaxBid,
	--	CAST('2001-01-01 00:00:00.000' as datetime) as DateMade,		
	--	CAST(0 as bit) as IsProxy,
	--	CAST(0 as bit) as IsProxyRaise		
	--	set @totalamount = 0;
	
	declare @iscurrent bit = 0
	--select @iscurrent=case when IsCurrent=1 or (DATEDIFF(second, DateEnd, getdate())>0 and DATEDIFF(second, DateEnd, getdate())<=300 and IsCurrent=0 and IsViewable=1 and IsClickable=1) then 1 else 0 end	from [Event] where ID=@event_id
	select @iscurrent = IsCurrent from [Event] where ID=@event_id
	
	create table #result(RN int not null identity(1,1), Bid_ID bigint, Lot bigint, Title varchar(512),Amount money, MaxBid money, DateMade datetime, IsProxy bit, IsProxyRaise bit, [User_Id] bigint);
	
	if (@iscurrent=1) begin
		insert into #result(Bid_ID, Lot, Title, Amount, MaxBid, DateMade, IsProxy, IsProxyRaise, [User_Id])
		select 
			b.ID,a.Lot,a.Title, b.Amount, b.MaxBid, b.DateMade, b.IsProxy, b.IsProxyRaise, b.[User_ID]
		from dbo.Auction a			
			join dbo.BidLogCurrent b on b.Auction_ID=a.ID			
		where a.Event_ID=@event_id
		order by b.DateMade desc
	end else begin
		insert into #result(Bid_ID, Lot, Title, Amount, MaxBid, DateMade, IsProxy, IsProxyRaise, [User_Id])
		select 
			b.ID,a.Lot,a.Title, b.Amount, b.MaxBid, b.DateMade, b.IsProxy, b.IsProxyRaise, b.[User_ID]
		from dbo.Auction a			
			join dbo.BidLog b on b.Auction_ID=a.ID			
		where a.Event_ID=@event_id
		order by b.DateMade desc
	end
	
	select r.*, u.[Login] as Bidder
	from #result r	
		join [User] u on u.ID=r.[User_Id]		
	where r.RN between
			case when @pageindex=0 then @pageindex*@pagesize else @pageindex*@pagesize+1 end
			and @pageindex*@pagesize+@pagesize
	order by r.RN
	
	select @totalamount = COUNT(*) from #result;	
	drop table #result;
end
GO
----------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spBid_View_BidLogStatistics]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spBid_View_BidLogStatistics]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spBid_View_BidLogStatistics]
	@event_id bigint
as
begin
	set nocount on;
	--select 
	--	CAST(0 as int) as BidsAmount,
	--	CAST(0 as money) as BidAmount,
	--	CAST(0 as money) as BuyersPremium
	
	declare @iscurrent bit = 0;
	declare @buyersfee money = 0;
	declare @bidsamount int =0;
	declare @bidamount money = 0;	
	
	select @iscurrent = IsCurrent, @buyersfee = BuyerFee from [Event] where ID=@event_id
	
	if (@iscurrent=1) begin
		select @bidsamount = COUNT(*) 
			from dbo.BidLogCurrent blc
				join dbo.Auction a on a.ID=blc.Auction_ID			
			where a.Event_ID=@event_id and DATEADD(MINUTE, -10, GETDATE()) < blc.DateMade;		
		select @bidamount = SUM(IsNUll(CurrentBid,0)) 
			from dbo.AuctionResultsCurrent arc
				join dbo.Auction a on a.ID=arc.Auction_ID			
			where a.Event_ID=@event_id and ISNULL(CurrentBid,0)>0;		
	end else begin		
		select @bidamount = SUM(IsNUll(CurrentBid,0)) 
			from dbo.AuctionResults arc
				join dbo.Auction a on a.ID=arc.Auction_ID			
			where a.Event_ID=@event_id and ISNULL(CurrentBid,0)>0;		
	end 	
	select @bidsamount as BidsAmount, @bidamount as BidAmount, cast(round(case when @bidamount =0 then 0 else @bidamount * @buyersfee * 0.01 end,2) as money) as BuyersPremium
end
GO
----------------------------------------------------------------------------------------------------------------
