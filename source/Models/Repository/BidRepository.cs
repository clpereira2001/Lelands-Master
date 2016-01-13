using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Lib;

namespace Vauction.Models
{
  public class BidRepository : IBidRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public BidRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
    {
      this.dataContext = dataContext;
      this.CacheRepository = CacheRepository;
    }
    //SubmitChages
    private void SubmitChages()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }
    #endregion


    #region restruct 2012-11-12

    //GetTopBidForItem
    public BidCurrent GetTopBidForItem(long auction_id, bool fromcache = true)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETTOPBIDFORITEM", new object[] { auction_id }, CachingExpirationTime.Hours_01);
      BidCurrent result = CacheRepository.Get(dco) as BidCurrent;
      if (result != null && fromcache) return new BidCurrent(result);
      dataContext.CommandTimeout = 600000;
      result = dataContext.spBid_WinningBid(auction_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new BidCurrent(result) : null;
    }
      

    //BiddingForSingleAuction
    /// <summary>
    /// Add bid, resolve proxy bidding situation
    /// </summary>
    /// <returns> 0 - winner, 1 - outbidder, 2 - update bid, 3 - wrong bid</returns>
    public byte BiddingForSingleAuction(AuctionDetail auction, BidCurrent currentBid, out BidCurrent previousBid, out BidCurrent loserBid, out BidCurrent winnerBid)
    {
      long? bid_id = -1;
      previousBid = loserBid = winnerBid = null;

      List<BidCurrent> allbids = dataContext.spBid_LotBids(auction.LinkParams.ID).ToList();
      
      // no bids
      if (!allbids.Any())
      {
        currentBid.Amount = currentBid.IsProxy ? auction.Price : currentBid.MaxBid; 
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive, null, null, null,
                                                    null, null, null, null, null, null, null, null, currentBid.User_ID,
                                                    currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        winnerBid = new BidCurrent(currentBid);
        return 0;
      }

      previousBid = allbids.FirstOrDefault(q => q.User_ID == currentBid.User_ID);

      // the duplicate
      if (previousBid != null)
      {
        currentBid.ID = previousBid.ID;
        if (previousBid.IsProxy == currentBid.IsProxy && previousBid.MaxBid >= currentBid.Amount) return 3;
      }

      BidCurrent topBid = allbids.First();

      // current is highbidder
      if(topBid.User_ID == currentBid.User_ID)
      {
        currentBid.Amount = (!currentBid.IsProxy && currentBid.Amount > topBid.Amount)
                              ? currentBid.Amount
                              : topBid.Amount;
        currentBid.MaxBid = Math.Max(currentBid.MaxBid, topBid.MaxBid);
        currentBid.IsProxy = currentBid.Amount <= currentBid.MaxBid;
        bid_id = topBid.ID;
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, true, currentBid.IsActive, null, null, null,
                                                    null, null, null, null, null, null, null, null, currentBid.User_ID,
                                                    currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        previousBid = new BidCurrent(topBid);
        winnerBid = new BidCurrent(currentBid);
        return 2;
      }

      decimal amount;
      bool isautobid;
      // current max bid is bigger than the top bid
      if (topBid.MaxBid < currentBid.MaxBid)
      {
        amount = currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid;
        isautobid = topBid.Amount != topBid.MaxBid;
        topBid.Amount = topBid.MaxBid;
        currentBid.Amount = !isautobid ? amount : (currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid);
        bid_id = previousBid == null ? -1 : currentBid.ID;
        if (isautobid)
          dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount, currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                      currentBid.IsProxy, false, currentBid.IsActive, currentBid.Amount, currentBid.MaxBid, DateTime.Now,
                                                      topBid.ID, topBid.User_ID, topBid.Amount, topBid.MaxBid, topBid.DateMade, DateTime.Now, topBid.IP, topBid.IsProxy,
                                                      currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
        else
          dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                   currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                   currentBid.IsProxy, false, currentBid.IsActive, null, null, null, null, null, null, null, null, null, null, null,
                                                   currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        loserBid = new BidCurrent(topBid);
        winnerBid = new BidCurrent(currentBid);
        return 0;
      }

      amount = currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid;
      isautobid = amount < currentBid.MaxBid;
      currentBid.Amount = currentBid.MaxBid;
      bid_id = previousBid == null ? -1 : currentBid.ID;
      
      if (topBid.Amount == topBid.MaxBid || (topBid.Amount > currentBid.Amount && topBid.MaxBid > currentBid.MaxBid))
      {
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive,
                                                    isautobid ? (decimal?)currentBid.Amount : null,
                                                    isautobid ? (decimal?)currentBid.MaxBid : null,
                                                    isautobid ? (DateTime?)DateTime.Now : null,
                                                    null, null, null, null, null, null, null, null, null, null, null);
      }
      else
      {
        topBid.Amount = Math.Min(currentBid.MaxBid + Consts.GetIncrement(currentBid.MaxBid), topBid.MaxBid);
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive,
                                                    isautobid ? (decimal?) currentBid.Amount : null,
                                                    isautobid ? (decimal?) currentBid.MaxBid : null,
                                                    isautobid ? (DateTime?) DateTime.Now : null,
                                                    topBid.ID, topBid.User_ID, topBid.Amount, topBid.MaxBid,
                                                    topBid.DateMade, DateTime.Now,
                                                    topBid.IP, topBid.IsProxy, topBid.User_ID, topBid.Amount, topBid.MaxBid);
      }
      currentBid.ID = bid_id.GetValueOrDefault(-1);
      loserBid = new BidCurrent(currentBid);
      winnerBid = new BidCurrent(topBid);
      return 1;
    }

    //RemoveTopBidForItemCache
    public void RemoveTopBidForItemCache(long auction_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETTOPBIDFORITEM", new object[] { auction_id }));
    }

    //UpdateUsersTopBidCache
    public void UpdateUsersTopBidCache(long auction_id, long user_id, BidCurrent bid)
    {
      CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM", new object[] { auction_id, user_id }, CachingExpirationTime.Hours_01, bid));
    }

   
    #endregion

    //GetPastUsersWatchList
    public List<MyBid> GetPastUsersWatchList(long event_id, long user_id)
    {
      dataContext.CommandTimeout = 600000;
      return (from sp in dataContext.spBid_View_GetPastBidWatch(user_id, event_id)
              select new MyBid
              {
                PriceRealized = new PriceRealized { ID = sp.ID, Lot = sp.Lot.HasValue ? sp.Lot.Value : (short)0, Title = sp.Title, Price = sp.PriceRealized.GetValueOrDefault(0), LinkParams = new LinkParams { EventTitle = sp.EventTitle, MainCategoryTitle = sp.MainCategoryTitle, CategoryTitle = sp.CategoryTitle } },
                AuctionStatus = sp.AuctionStatus.GetValueOrDefault(1),
                Amount = sp.Amount,
                MaxBid = sp.MaxBid,
                DateMade = sp.DateMade.GetValueOrDefault(DateTime.MinValue),
                IsUnsold = sp.IsUnsold,
                IsWinner = sp.IsWinner.GetValueOrDefault(false),
                WinBid = sp.WinBid.GetValueOrDefault(0),
                BidsCount = sp.BidsCount.GetValueOrDefault(0),
                ThubnailImage = sp.ThumbnailPath
              }).ToList();
    }

    //GetPastEventBiddingHistory
    public List<Event> GetPastEventBiddingHistory(long user_id)
    {      
      return dataContext.spEvent_PastBiddingHistory(user_id).ToList();
    }

    //GetBidWatchForUser
    public List<UserBidWatch> GetBidWatchForUser(long user_id, long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "GETBIDWATCHFORUSER", new object[] { user_id, event_id }, CachingExpirationTime.Seconds_15);
      List<UserBidWatch> result = CacheRepository.Get(dco) as List<UserBidWatch>;
      if (result != null && result.Any()) return result;
      dataContext.CommandTimeout = 600000;
      result = (from p in dataContext.spBid_View_BidWatch(user_id, event_id)
                select new UserBidWatch
                {
                  AuctionStatus =  p.AuctionStatus.GetValueOrDefault(1),
                  Amount = p.Amount.GetValueOrDefault(0),
                  CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                  HighBidder = p.HighBidder,
                  Quantity = p.Quantity,
                  Bids = p.Bids.GetValueOrDefault(0),
                  MaxBid = p.MaxBid.GetValueOrDefault(0),
                  Option = p.IsWatch.GetValueOrDefault(0),
                  LinkParams = new LinkParams { ID = p.Auction_ID, Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle, MainCategoryTitle = p.MainCategoryTitle }
                }).ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    
    //GetUsersTopBidForItem
    public BidCurrent GetUserTopBidForItem(long auction_id, long user_id, bool iscaching)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM",
                                                new object[] { auction_id, user_id }, CachingExpirationTime.Minutes_05);
      BidCurrent bc = CacheRepository.Get(dco) as BidCurrent;
      if (bc != null && iscaching) return bc;
      dataContext.CommandTimeout = 600000;
      bc = dataContext.spBid_UserTopBid(user_id, auction_id).FirstOrDefault();
      if (bc != null)
      {
        dco.Data = bc;
        CacheRepository.Add(dco);
      }
      return bc;
    }

    //UpdateUsersTopBid
    public void UpdateUsersTopBid(long auction_id, long user_id, BidCurrent bid)
    {
      CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM",
                                                new object[] { auction_id, user_id }, CachingExpirationTime.Minutes_05, bid));
    }

    //AuctionBiddingResult
    public BiddingResult CurrentAuctionBiddingResult(long auction_id, long? user_id, decimal price)
    {
      BiddingResult br = new BiddingResult(price);
      br.WinningBid = GetTopBidForItem(auction_id);
      br.UsersTopBid = user_id.HasValue ? GetUserTopBidForItem(auction_id, user_id.Value, true) : null;
      return br;
    }

    //PlaceSingleBid
    public BiddingObject PlaceSingleBid(long auction_id, bool isproxy, decimal amount, long user_id, int quantity, bool isproxyraise, decimal aprice, BidCurrent prevUsersBid, BidCurrent lastTop)
    {
      BidCurrent newBid;
      bool isHighBidder = (lastTop != null) ? lastTop.User_ID == user_id : false;
      amount = (lastTop != null && ((long)amount == (long)lastTop.MaxBid || Math.Abs(lastTop.MaxBid - amount) < Consts.ErrorRangeAmount)) ? lastTop.MaxBid : amount;

      if (prevUsersBid != null)
        newBid = prevUsersBid;
      else
      {
        newBid = new BidCurrent();
        dataContext.BidCurrents.InsertOnSubmit(newBid);
      }
      if (!isproxy)
      {
        newBid.Amount = amount;
        if (newBid.MaxBid > amount)
          newBid.IsProxy = true;
        else
        {
          newBid.MaxBid = amount;
          newBid.IsProxy = false;
        }
      }
      else
      {
        decimal price = (lastTop == null) ? aprice : lastTop.Amount;
        newBid.MaxBid = (newBid.MaxBid < amount) ? amount : newBid.MaxBid;
        newBid.Amount = (price + Consts.GetIncrement(price) <= newBid.MaxBid) ? price : newBid.MaxBid;
        newBid.Amount += (lastTop != null && !isHighBidder && newBid.Amount + Consts.GetIncrement(price) <= newBid.MaxBid) ? Consts.GetIncrement(price) : 0;
        newBid.IsProxy = true;
      }
      newBid.Auction_ID = auction_id;
      newBid.User_ID = user_id;
      newBid.Quantity = quantity;
      newBid.DateMade = DateTime.Now;
      newBid.IP = Consts.UsersIPAddress;
      newBid.IsActive = true;

      BidLogCurrent log = new BidLogCurrent();
      dataContext.BidLogCurrents.InsertOnSubmit(log);
      log.Quantity = quantity;
      log.User_ID = newBid.User_ID;
      log.IsProxy = isproxy;
      log.MaxBid = newBid.MaxBid;
      log.Amount = newBid.Amount;
      log.IP = newBid.IP;
      log.Auction_ID = newBid.Auction_ID;
      log.DateMade = DateTime.Now;
      log.IsProxyRaise = isproxyraise;

      SubmitChages();

      return new BiddingObject { Bid = newBid, BidLog = log };
    }

    //ResolveProxyBiddingSituation
    public void ResolveProxyBiddingSituation(long auction_id, long user_id, bool isproxy, BiddingObject placedBid, BidCurrent lastTop, decimal aprice, List<BidLogCurrent> newbidlogs)
    {
      if (placedBid.Bid == null || placedBid.BidLog == null || lastTop == null) return;
      //List<BidCurrent> bids = dataContext.BidCurrents.Where(B => B.Auction_ID == auction_id && B.ID != placedBid.Bid.ID && B.User_ID != user_id).OrderBy(B3 => B3.DateMade).OrderByDescending(B2 => B2.MaxBid).OrderByDescending(B1 => B1.Amount).ToList();
      List<BidCurrent> bids = dataContext.spBid_BidsExceptCurrent(auction_id, placedBid.Bid.ID, user_id).ToList();
      if (bids.Count() == 0) return;

      decimal price = (lastTop == null) ? aprice : lastTop.Amount;
      decimal Increment = Consts.GetIncrement(price);

      BidCurrent queryBid = bids.FirstOrDefault();

      if (placedBid.Bid.IsProxy && placedBid.Bid.Amount <= (queryBid.MaxBid+ Consts.ErrorRangeAmount))
      {
        if (queryBid.MaxBid + Increment >= (placedBid.Bid.MaxBid+ Consts.ErrorRangeAmount))
        {
          //Can't overBid
          placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
        }
        else
        {
          Increment = Consts.GetIncrement(queryBid.MaxBid);
          if (Increment + queryBid.MaxBid + Consts.ErrorRangeAmount > placedBid.Bid.MaxBid)
            placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
          else if (placedBid.Bid.User_ID != lastTop.User_ID)
            placedBid.BidLog.Amount = placedBid.Bid.Amount = queryBid.MaxBid + Increment;
        }
      }
      foreach (BidCurrent runner in bids)
      {
        if (!runner.IsProxy) continue;
        if ((runner.MaxBid == placedBid.Bid.MaxBid) || (Math.Abs(runner.MaxBid - placedBid.Bid.MaxBid) < Consts.ErrorRangeAmount))
          runner.Amount = placedBid.Bid.MaxBid;
        else
        {
          Increment = Consts.GetIncrement(placedBid.Bid.MaxBid);
          if (runner.MaxBid >= placedBid.Bid.MaxBid + Increment + Consts.ErrorRangeAmount)
            runner.Amount = placedBid.Bid.MaxBid + Increment;
          else
          {
            if (runner.Amount != runner.MaxBid)
            {
              BidLogCurrent log = new BidLogCurrent();
              dataContext.BidLogCurrents.InsertOnSubmit(log);
              log.Quantity = runner.Quantity;
              log.User_ID = runner.User_ID;
              log.IsProxy = runner.IsProxy;
              log.MaxBid = runner.MaxBid;
              log.Amount = runner.MaxBid;
              log.IP = runner.IP;
              log.Auction_ID = runner.Auction_ID;
              log.DateMade = DateTime.Now;
              log.IsProxyRaise = false;
              newbidlogs.Add(log);
            }
            runner.Amount = runner.MaxBid;
          }
        }
      }
      SubmitChages();
    }

    //UpdateCurrentBid
    public bool UpdateCurrentBid(BidCurrent b)
    {
      try
      {
        //BidCurrent b = dataContext.BidCurrents.Where(B => B.ID == updBid.ID).SingleOrDefault();
        //if (b == null) return false;
        //b.Amount = updBid.Amount;
        //b.User_ID = updBid.User_ID;
        //b.DateMade = updBid.DateMade;
        //b.IP = updBid.IP;
        //b.IsProxy = updBid.IsProxy;
        //b.IsReserveBid = updBid.IsReserveBid;
        //b.MaxBid = updBid.MaxBid;
        //b.Quantity = updBid.Quantity;
        SubmitChages();
      }
      catch (Exception ex)
      {
        Logger.LogException("[bidlogcurrent_id=" + b.ID + "]", ex);
        return false;
      }
      return true;
    }

    //AddBidLogCurrent
    public BidLogCurrent AddBidLogCurrent(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP)
    {
      BidLogCurrent log = new BidLogCurrent();
      try
      {
        dataContext.BidLogCurrents.InsertOnSubmit(log);
        log.Quantity = quantity;
        log.User_ID = user_id;
        log.IsProxy = isproxy;
        log.MaxBid = maxamount;
        log.Amount = amount;
        log.IP = String.IsNullOrEmpty(IP) ? Consts.UsersIPAddress : IP;
        log.Auction_ID = auction_id;
        log.DateMade = DateTime.Now;
        log.IsProxyRaise = isproxyraise;
        SubmitChages();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return null;
      }
      return log;
    }

    //IsUserCanParticipateInBidding
    public bool IsUserCanParticipateInBidding(long auction_id, long user_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "ISUSERCANPARTICIPATEINBIDDING",
                                                new object[] { auction_id, user_id }, CachingExpirationTime.Days_01);
      bool? result = CacheRepository.Get(dco) as bool?;
      if (result.HasValue) return true;
      result = dataContext.spBid_IsUserHasRightsToBid(user_id, auction_id).ToList().Any();
      if (result.Value)
      {
        dco.Data = true;
        CacheRepository.Add(dco);
      }
      return result.Value;
    }
  }
}
