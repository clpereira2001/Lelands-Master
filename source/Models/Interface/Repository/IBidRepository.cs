using System.Collections.Generic;

namespace Vauction.Models
{
  public interface IBidRepository
  {
    BidCurrent GetTopBidForItem(long auction_id, bool fromcache = true);
    byte BiddingForSingleAuction(AuctionDetail auction, BidCurrent currentBid, out BidCurrent previousBid,
                                 out BidCurrent loserBid, out BidCurrent winnerBid);

    void RemoveTopBidForItemCache(long auction_id);
    void UpdateUsersTopBidCache(long auction_id, long user_id, BidCurrent bid);
    



    List<MyBid> GetPastUsersWatchList(long event_id, long user_id);
    List<Event> GetPastEventBiddingHistory(long user_id);
    List<UserBidWatch> GetBidWatchForUser(long user_id, long event_id);
    
    BidCurrent GetUserTopBidForItem(long auction_id, long user_id, bool iscaching);
    BiddingResult CurrentAuctionBiddingResult(long auction_id, long? user_id, decimal price);
    BiddingObject PlaceSingleBid(long auction_id, bool isproxy, decimal amount, long user_id, int quantity, bool isproxyraise, decimal aprice, BidCurrent prevUsersBid, BidCurrent lastTop);
    void ResolveProxyBiddingSituation(long auction_id, long user_id, bool isproxy, BiddingObject placedBid, BidCurrent lastTop, decimal aprice, List<BidLogCurrent> newbidlogs);
    bool UpdateCurrentBid(BidCurrent b);
    BidLogCurrent AddBidLogCurrent(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP);
    void UpdateUsersTopBid(long auction_id, long user_id, BidCurrent bid);
    bool IsUserCanParticipateInBidding(long auction_id, long user_id);    
  }
}
