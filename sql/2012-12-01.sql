USE [Lelands]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUpdate_AuctionResultsCurrent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[spUpdate_AuctionResultsCurrent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[spUpdate_AuctionResultsCurrent]
	@ID [bigint], @Auction_ID [bigint], @User_ID [bigint], @CurrentBid [money], @Bids [int], @MaxBid [money]
as
begin
	update dbo.AuctionResultsCurrent 
	set Auction_ID=@Auction_ID, [User_ID]=@User_ID, CurrentBid=@CurrentBid, Bids=@Bids, MaxBid=@MaxBid
	where ID=@ID	
end
GO