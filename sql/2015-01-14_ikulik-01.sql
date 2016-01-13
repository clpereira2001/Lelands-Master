use Lelands
GO
------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_Event_Close]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_Event_Close]
GO
--
CREATE PROCEDURE [dbo].[sp_Event_Close]
	@EventID bigint, @IsFirstStep bit = 1
AS
BEGIN		
	set nocount on;
	
	-- [STEP 1]
	if (@IsFirstStep=1) begin		
		--set event's step==1 
		update dbo.[Event] set CloseStep=1 where ID=@EventID;		
	  
	  --close auctions where bids count < 2
	  update a
	  set a.[Status]=3, EndDate=GETDATE()
		from dbo.Auction a
			join dbo.[Event] e on e.ID=a.Event_ID
			left join (
				select Auction_Id, COUNT(*) as BidNumber
				from dbo.BidCurrent
				group by Auction_ID
			) b on b.Auction_ID=a.ID
		where e.IsCurrent=1 
			and e.ID=@EventID
			and a.[Status]=2
			and IsNull(b.BidNumber, 0) < 2
		
		waitfor delay '00:00:10'
		
		-- copying bids
		insert into Bid(Amount, Auction_ID, DateMade, IP, IsActive, IsProxy, MaxBid, Quantity, [User_ID])
			select bc.Amount, bc.Auction_ID, bc.DateMade, bc.IP, bc.IsActive, bc.IsProxy, bc.MaxBid, bc.Quantity, bc.[User_ID]
			from dbo.BidCurrent bc 
				join Auction auc on auc.ID=bc.Auction_ID 
			where auc.Event_ID=@EventID and auc.[Status]=3 and auc.DateSold is null
		
		-- copying bid log
		insert into BidLog(Amount, Auction_ID, DateMade, IP, IsProxy, IsProxyRaise, MaxBid, Quantity, [User_ID])
			select bl.Amount, bl.Auction_ID, bl.DateMade, bl.IP, bl.IsProxy, bl.IsProxyRaise, bl.MaxBid, bl.Quantity, bl.[User_ID] 
			from dbo.BidLogCurrent bl 
				join Auction auc on auc.ID=bl.Auction_ID 
			where auc.Event_ID=@EventID and auc.[Status] = 3 and auc.DateSold is null
		return;
	end
	
	-- [STEP 2]
	-- close auction
	update Auction set [Status]=3, EndDate=GETDATE() where [Status]=2 and Event_ID=@EventID;
	
	-- delay for all bids to be set correctly
	waitfor delay '00:00:10'
	
	-- copying bids
	insert into Bid(Amount, Auction_ID, DateMade, IP, IsActive, IsProxy, MaxBid, Quantity, [User_ID])
		select bc.Amount, bc.Auction_ID, bc.DateMade, bc.IP, bc.IsActive, bc.IsProxy, bc.MaxBid, bc.Quantity, bc.[User_ID]
		from dbo.BidCurrent bc 
			join Auction auc on auc.ID=bc.Auction_ID
			left join Bid b on b.Auction_ID=bc.Auction_ID and b.User_ID=bc.User_ID
		where auc.Event_ID=@EventID and auc.DateSold is null and b.ID is null
	
	-- copying bid log
	insert into BidLog(Amount, Auction_ID, DateMade, IP, IsProxy, IsProxyRaise, MaxBid, Quantity, [User_ID])
		select bl.Amount, bl.Auction_ID, bl.DateMade, bl.IP, bl.IsProxy, bl.IsProxyRaise, bl.MaxBid, bl.Quantity, bl.[User_ID] 
		from BidLogCurrent bl 
			join Auction auc on auc.ID=bl.Auction_ID
			left join BidLog b on b.Auction_ID=bl.Auction_ID and b.User_ID=bl.User_ID
		where auc.Event_ID=@EventID and auc.DateSold is null and b.ID is null
		
	-- copying bid watch current
	insert into BidWatch(Auction_ID, [User_ID])
		select bwc.Auction_ID, bwc.[User_ID] 
		from BidWatchCurrent bwc
			join Auction auc on auc.id=bwc.Auction_ID
			left join BidWatch bw on bw.Auction_ID=bwc.Auction_ID and bw.[User_ID]=bwc.[User_ID] 
		where auc.Event_ID=@EventID and bw.ID is null
	
	-- copying auction results	
	delete ar from AuctionResultsCurrent ar join Auction a on a.ID=ar.Auction_ID
	where a.Event_ID=@EventID 
		and ISNULL(ar.User_ID,-1)=-1		
		and ISNULL(ar.CurrentBid,-1)=-1
		and ISNULL(ar.MaxBid,-1)=-1		
		
	delete AR from dbo.AuctionResults AR join dbo.Auction a on a.ID=AR.Auction_ID where a.Event_ID=@EventID;	
	insert into dbo.AuctionResults(Auction_ID, CurrentBid, MaxBid, Bids, [User_ID])
		select Auction_ID, CurrentBid, MaxBid, Bids, [User_ID] 
		from dbo.AuctionResultsCurrent arc join dbo.Auction a on a.ID=arc.Auction_ID where a.Event_ID=@EventID	
	
	-- invoices
	declare @auction_id bigint;
	declare auction_cursor cursor for 
		select a.ID 
		from Auction a			
		where a.Event_ID=@EventID and a.[Status]=3 and a.DateSold is null
		
	open auction_cursor
	fetch next from auction_cursor into @auction_id
	while @@FETCH_STATUS = 0
	begin
		insert into Invoice(Auction_ID,[User_ID],DateCreated,IsPaid,Amount, Cost, Comments,Quantity,Shipping,Tax, IsSent, LateFee)
		select tbl.Auction_ID, tbl.[USER_ID], GETDATE(), 0, tbl.Amount, cast(Round(tbl.Amount*(1+e.BuyerFee/100),2) as money), case when tbl.Quantity = 1 then 'Regular auction won item' else 'Multiple auction won item' end, tbl.Quantity, CAST(Round(tbl.Quantity*IsNull(a.Shipping,0),2) as money), tbl.TaxAmount, 0, 0
		from 
		(select d.Auction_ID, d.[USER_ID], sum(d.AMOUNT) as Amount, COUNT(d.AMOUNT) as Quantity, sum(case when (ac.[State]='NY' or ac.[State_ID]=40) and IsNull(u.TaxpayerID,'')=''  then Round(v.Value*(Round(d.Amount*(1+e.BuyerFee/100)+a.Shipping,2)),2) else 0 end) as TaxAmount
			from dbo.[f_Bid_GetWinner_CheckEvent](@auction_id) d
				join [Auction] a on a.ID = d.Auction_ID					
				join [User] u on u.ID = d.[USER_ID]
				join [AddressCard] ac on ac.ID = u.Billing_AddressCard_ID				
				join [Event] e on e.ID = a.Event_ID
				join Variable v on v.Name = 'SalesTaxRate'
			where U.UserType in (4,5)
				--and d.Amount>=a.Reserve
				and d.Amount>=isnull(a.Estimate, 0)
			group by d.Auction_ID, d.[User_ID]
			) tbl
			join [Auction] a on a.ID = tbl.Auction_ID
			join [Event] e on e.ID = a.Event_ID
		order by tbl.Amount desc
		
		-- next auction
		fetch next from auction_cursor into @auction_id
	end
	close auction_cursor
	deallocate auction_cursor
	
	-- setting event not current
	update [Event] set IsCurrent=0, CloseStep=2 where ID=@EventID;
	
	-- add userinvoices
	insert into UserInvoices([User_ID], SaleDate, [Event_ID], IsPaid, IsSend)
		select inv.[User_ID], MAX(inv.DateCreated) as DT, auc.Event_ID, 0, 0
		from [Invoice] inv
			join [Auction] auc on auc.ID=inv.Auction_ID
			left join [UserInvoices] ui on ui.Event_ID=auc.Event_ID and ui.[User_ID]=inv.[User_ID]			
		where auc.Event_ID=@EventID and ui.Id is null
		group by inv.[User_ID], auc.Event_ID
	
	update i
	set i.UserInvoices_ID=ui.Id
	from UserInvoices ui
		join Auction a on a.Event_ID=ui.Event_ID
		join Invoice i on i.Auction_ID = a.ID and i.[User_ID]=ui.[User_ID] and i.Consignments_ID is null
	where ui.Event_ID = @EventID
	
	---- consignor statements	
	insert into Invoice(Auction_ID,[User_ID], DateCreated,IsPaid,Amount, Cost, Comments,Quantity,Shipping, Tax, Consignments_ID, IsSent, LateFee, Invoice_ID)
		select a.ID, c.[User_ID], GETDATE(), 0, sum(i.Amount), CAST(Round(sum(i.Amount-dbo.GetComissionForItem(i.Auction_ID, i.Amount)),2) as money), '', SUM(i.Quantity), 0, 0, c.Id, 0, 0, i.ID
		from Consignments c 
			join  Auction a on c.Event_ID = a.Event_ID and a.Owner_ID = c.[User_ID]		
			join Invoice i on i.Auction_ID=a.ID
		where c.Event_ID = @EventID and a.DateSold is null and a.[Status] = 3
		group by a.ID, c.[User_ID], c.Id, i.ID
		order by c.Id
	
	update iB set iB.Invoice_ID=iSe.ID
	from Invoice iB
		join Invoice iSe on iSe.Invoice_Id=iB.Id
		join  Auction a on a.ID=iSe.Auction_ID
	where a.DateSold is null and a.[Status] = 3 and a.Event_ID=@EventID
	
	update auc
		set auc.DateSold = GETDATE()
		from Invoice inv
			join Auction auc on auc.ID=inv.Auction_ID
		where auc.Event_ID=@EventID
		
	-- add bidding statistic
	exec sp_Event_AddBiddingStatistic @EventID;	
END
GO
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------
------------------------------------------------------------------