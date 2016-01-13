using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IBidRepository
  {
    object GetBidsForAuction(long auction_id);
    object GetBiddingStatistic(long event_id);
    JsonExecuteResult PlaceBid(long auction_id, long UserID, decimal Amount, decimal MaxBid, int Quantity, DateTime DateMade, bool IsProxy, string Comments, string IP, long bid_id);
    JsonExecuteResult DeleteBid(long bid_id, long auction_id);
    JsonExecuteResult UpdateBid(long auction_id, long UserID, decimal Amount, decimal MaxBid, int Quantity, DateTime DateMade, bool IsProxy, string Comments, string IP, bool IsProxyRaise, long bid_id);
    JsonExecuteResult DeleteBidLog(long bid_id, long auction_id);
    string GetBidsAmountForAuction(long auction_id);

    //decimal GetCoefficient(string name);
    //IEnumerable<sp_GetWatchBidForUserResult> GetBidWatchForUser(long p);
    //IBid GetTopBidForItem(long id);
    //void PlaceDOWOrder(long id, int Quantity, long userid);
    //IBid PlaceSingleBid(long AuctionId, bool ProxyBid, decimal BidToPlace, long UserId, int Quantity, bool IsRaiseProxy);
    //IEnumerable<BidLog> GetBidsHistoryForItem(long id);
    //Int64 AddNew(BidLog newBid);
    //BidLog AddNew(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP);
    //bool Update(Bid updBid);
    //bool Update(BidLog updBid);
    //IBid GetBid(Int64 id);
    //BidLog GetBidLog(Int64 id);
    //IBid GetUsersBidForAuction(long user_id, long auction_id);
    //IEnumerable<Bid> GetDutchWinnerTableBidForItem(long id);
    //bool IsBidWatchedByUser(long userId, long auctionId);
    //decimal GetMinimumBidForDutchItem(long id);
    //decimal GetMinimumBidForNormalItem(long id, long UserId);
    //IEnumerable<IInvoice> GetDOWFailed();
    //void UpdateDOWAfterFail(long invoiceID);
    //User GetHighBidderForNormalItem(long id);
    //IEnumerable<Bid> GetBidsForItem(long id);
    //bool DepositNeeded(decimal BidAmount, long User_ID, long Auction_ID);
    //decimal GetDepositNeededForUser(decimal BidToPlace, long User_ID);
  }
}
