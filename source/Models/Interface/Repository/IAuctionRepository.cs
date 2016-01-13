using System.Collections.Generic;
using Vauction.Models.CustomModels;

namespace Vauction.Models
{
  public interface IAuctionRepository
  {
    IEnumerable<AuctionShort> GetByCriterias(AuctionFilterParams filter);
    IEnumerable<AuctionShort> GetPastListForEvent(CategoryFilterParams filterParam);
    IEnumerable<AuctionShort> GetPastListForCategory(CategoryFilterParams filterParam);
    IEnumerable<AuctionShort> GetListForCategory(CategoryFilterParams filterParam);
    List<AuctionUpdate> GetAuctionUpdates(long event_id);
    IEnumerable<AuctionShort> GetAuctionListForSeller(AuctionFilterParams filter, long user_id);
    AuctionDetail GetAuctionDetail(long auction_id, long currentevent_id, bool iscaching);
    AuctionDetail GetAuctionDetail(long auction_id, bool iscaching);
    AuctionShort GetAuctionDetailResult(long auction_id, bool iscaching);
    AuctionShort GetAuctionDetailResultPast(long auction_id);
    bool IsUserWatchItem(long user_id, long auction_id, bool iscache = true);
    void AddItemToWatchList(long user_id, long auction_id);
    bool RemoveItemFromWatchList(long user_id, long auction_id);
    void UpdateAuctionBiddingResult(long auction_id, long user_id, decimal currentbid, decimal maxbid);
    object UpdateCategoryViewResults(string prms);
    IEnumerable<AuctionShort> GetProductsListForTag(long eventID, bool isPast, TagFilterParams filter);
    void RemoveAuctionCache(long auctionID);
    void RemoveAuctionResultsCache(long auctionID);
    List<AuctionSales> GetProductsForSales(long eventID);
  }
}
