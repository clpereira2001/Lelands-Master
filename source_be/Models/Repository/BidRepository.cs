using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
    public class BidRepository : IBidRepository
    {
        private ICacheDataProvider CacheRepository;
        private VauctionDataContext dataContext;

        public BidRepository(VauctionDataContext dataContext, ICacheDataProvider cacheRepository)
        {
            this.dataContext = dataContext;
            CacheRepository = cacheRepository;
        }

        //GetBidsForAuction
        public object GetBidsForAuction(long auction_id)
        {
            var auc = dataContext.Auctions.SingleOrDefault(A => A.ID == auction_id);
            var bids = (auc.Status == (byte) Consts.AuctionStatus.Open)
                ? (from B in dataContext.BidCurrents
                    where B.Auction_ID == auction_id
                    orderby B.Amount descending, B.MaxBid descending, B.DateMade ascending
                    select B).Cast<IBid>().ToList()
                : (from B in dataContext.Bids
                    where B.Auction_ID == auction_id
                    orderby B.Amount descending, B.MaxBid descending, B.DateMade ascending
                    select B).Cast<IBid>().ToList();

            var jsonData = new
            {
                total = bids.Count,
                page = 1,
                records = bids.Count,
                rows = (
                    from query in bids
                    select new
                    {
                        i = query.ID,
                        cell = new string[]
                        {
                            (auc.Status == (byte) Consts.AuctionStatus.Open)
                                ? (query as BidCurrent).User.Login
                                : (query as Bid).User.Login,
                            (auc.Status == (byte) Consts.AuctionStatus.Open)
                                ? (query as BidCurrent).User.UType.Title
                                : (query as Bid).User.UType.Title,
                            query.Amount.GetCurrency(),
                            query.MaxBid.GetCurrency(),
                            query.DateMade.Date.ToString("d") + " " +
                            query.DateMade.ToString("hh:mm:ss.fff", CultureInfo.InvariantCulture),
                            (((auc.Status == (byte) Consts.AuctionStatus.Open)
                                ? (query as BidCurrent).Auction.AuctionType_ID
                                : (query as Bid).Auction.AuctionType_ID) == (byte) Consts.AuctionType.Normal)
                                ? query.Quantity.ToString()
                                : String.Empty
                        }
                    }).ToArray()
            };

            bids = null;

            return jsonData;
        }

        //GetBiddingStatistic
        public object GetBiddingStatistic(long event_id)
        {
            //DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETBIDDINGSTATISTIC", new object[] { event_id }, CachingExpirationTime.Seconds_30);
            //var res = CacheRepository.Get(dco);
            //if (res != null) return res;
            dataContext.CommandTimeout = 600000;
            var bs = dataContext.sp_GetBiddingStatistic(event_id).ToList();
            return new
            {
                total = 1,
                page = 1,
                records = 1,
                rows = (
                    from query in bs
                    select new
                    {
                        i = 1,
                        cell =
                            new List<string>
                            {
                                query.WithoutBids.ToString(),
                                query.OneBid.ToString(),
                                query.MoreThanOneBid.ToString()
                            }
                    }).ToList()
            };
            //dco.Data = res;
            //CacheRepository.Add(dco);
            //return res;
        }

        public JsonExecuteResult PlaceBid(long auction_id, long UserID, decimal Amount, decimal MaxBid, int Quantity,
            DateTime DateMade, bool IsProxy, string Comments, string IP, long bid_id)
        {
            try
            {
                var auction = GetAuctionDetail(auction_id);
                if (auction == null)
                {
                    throw new Exception("The auction doesn't exist.");
                }
                if (!auction.IsCurrentEvent || auction.CloseStep == 2)
                {
                    throw new Exception("You can't place this bid. The auction is not open.");
                }
                //add to bidwatch
                if (bid_id == 0)
                {
                    var bw =
                        dataContext.BidWatchCurrents.SingleOrDefault(
                            bidWatch => bidWatch.Auction_ID == auction_id && bidWatch.User_ID == UserID);
                    if (bw == null)
                    {
                        dataContext.BidWatchCurrents.InsertOnSubmit(new BidWatchCurrent
                        {
                            Auction_ID = auction_id,
                            User_ID = UserID
                        });
                        GeneralRepository.SubmitChanges(dataContext);
                    }
                }

                byte result;
                var currentBid = new BidCurrent
                {
                    Amount = Amount,
                    Auction_ID = auction_id,
                    DateMade = DateMade,
                    IP = IP,
                    IsActive = true,
                    IsProxy = IsProxy || Amount < MaxBid,
                    MaxBid = MaxBid,
                    Quantity = Quantity,
                    User_ID = UserID
                };
                BidCurrent previousBid, loserBid, winnerBid;

                lock (auction)
                {
                    result = BiddingForSingleAuction(auction, currentBid, out previousBid, out loserBid, out winnerBid);
                    if (result == 3)
                    {
                        throw new Exception("The error had happen.");
                    }

                    CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS,
                        "GETAUCTIONRESULTCURRENT", new object[] {auction_id}));
                    CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS,
                        "GETTOPBIDFORITEM", new object[] {auction_id}));
                    CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS,
                        "GETUSERTOPBIDFORITEM", new object[] {auction_id, UserID}, CachingExpirationTime.Hours_01,
                        currentBid));
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);

            //try
            //{
            //  Auction auction = dataContext.Auctions.Where(A => A.ID == auction_id).SingleOrDefault();
            //  Event evnt = auction.Event;
            //  if (auction == null) throw new Exception("The auction doesn't exist.");
            //  if (!evnt.IsCurrent || evnt.CloseStep==2) throw new Exception("You can't place this bid. The auction is not open.");
            //  //if (auction.Quantity < Quantity) throw new Exception("The auction quantity is less than " + Quantity.ToString() + ". You can't place this bid.");

            //  if (bid_id == 0)
            //  {
            //    BidWatch bw = dataContext.BidWatches.Where(BW => BW.Auction_ID == auction_id && BW.User_ID == UserID).SingleOrDefault();
            //    if (bw == null)
            //    {
            //      bw = new BidWatch();
            //      bw.Auction_ID = auction_id;
            //      bw.User_ID = UserID;
            //      dataContext.BidWatches.InsertOnSubmit(bw);
            //    }
            //  }

            //  BidCurrent prevBid = GetUserTopBidForItem(auction_id, UserID);        
            //  BidCurrent LastTop = GetTopBidForItem(auction_id);
            //  bool isfirstbid = LastTop == null;
            //  decimal lastMaxBid = (LastTop == null) ? 0 : LastTop.MaxBid;
            //  decimal lastamount = (LastTop == null) ? 0 : LastTop.Amount;

            //  BiddingObject placedBid = PlaceSingleBid(auction_id, IsProxy, MaxBid, UserID, Quantity, (LastTop != null && LastTop.User_ID == UserID && IsProxy), auction.Price, prevBid, LastTop);

            //  List<BidLogCurrent> newblogs = new List<BidLogCurrent>();
            //  ResolveProxyBiddingSituation(auction_id, UserID, IsProxy, placedBid, LastTop, auction.Price, newblogs);

            //  BidCurrent CurrentTop = GetTopBidForItem(auction_id);        
            //  bool IsOutBidden = (LastTop != null && CurrentTop.MaxBid <= LastTop.MaxBid && CurrentTop.User_ID != UserID);
            //  if (IsOutBidden)
            //  {
            //    if (placedBid.Bid.Amount >= CurrentTop.Amount)
            //    {
            //      CurrentTop.Amount = placedBid.Bid.Amount;
            //      GeneralRepository.SubmitChanges(dataContext);
            //    }
            //    if (lastamount < CurrentTop.Amount && newblogs.Where(BL => BL.User_ID == CurrentTop.User_ID && BL.Amount == CurrentTop.Amount && BL.MaxBid == CurrentTop.MaxBid && BL.IsProxy == CurrentTop.IsProxy).Count() == 0)
            //      AddBidLogCurrent(auction_id, CurrentTop.Quantity, CurrentTop.User_ID, CurrentTop.IsProxy, CurrentTop.Amount, CurrentTop.MaxBid, false, CurrentTop.IP);          
            //    UpdateAuctionBiddingResult(auction_id, CurrentTop.User_ID, CurrentTop.Amount, CurrentTop.MaxBid);
            //  }
            //  else
            //  {
            //    UpdateAuctionBiddingResult(auction_id, CurrentTop.User_ID, CurrentTop.Amount, CurrentTop.MaxBid);
            //  }
            //}
            //catch (Exception ex)
            //{
            //  return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            //}
            //return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteBid
        public JsonExecuteResult DeleteBid(long bid_id, long auction_id)
        {
            var auction = dataContext.Auctions.SingleOrDefault(A => A.ID == auction_id);
            try
            {
                if (auction == null)
                {
                    throw new Exception("The auction doesn't exist.");
                }
                var evnt = auction.Event;
                if (!evnt.IsCurrent || evnt.CloseStep == 2)
                {
                    throw new Exception("You can't place this bid. The auction is not open.");
                }
                var bc = dataContext.BidCurrents.SingleOrDefault(B => B.ID == bid_id);
                if (bc == null)
                {
                    throw new Exception("The bid doesn't exist.");
                }
                var blc =
                    dataContext.BidLogCurrents.Where(B => B.Auction_ID == bc.Auction_ID && B.User_ID == bc.User_ID)
                        .ToList();
                var bw =
                    dataContext.BidWatchCurrents.SingleOrDefault(
                        B => B.Auction_ID == bc.Auction_ID && B.User_ID == bc.User_ID);
                if (blc.Any())
                {
                    dataContext.BidLogCurrents.DeleteAllOnSubmit(blc);
                }
                if (bw != null)
                {
                    dataContext.BidWatchCurrents.DeleteOnSubmit(bw);
                }
                dataContext.BidCurrents.DeleteOnSubmit(bc);
                GeneralRepository.SubmitChanges(dataContext);
                var CurrentTop = GetTopBidForItem(auction_id);
                if (CurrentTop != null)
                {
                    UpdateAuctionBiddingResult(auction_id, CurrentTop.User_ID, CurrentTop.Amount, CurrentTop.MaxBid);
                }
                else
                {
                    UpdateAuctionBiddingResult(auction_id, null, null, null);
                }
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //UpdateBid
        public JsonExecuteResult UpdateBid(long auction_id, long UserID, decimal Amount, decimal MaxBid, int Quantity,
            DateTime DateMade, bool IsProxy, string Comments, string IP, bool IsProxyRaise, long bid_id)
        {
            try
            {
                var auction = dataContext.Auctions.SingleOrDefault(A => A.ID == auction_id);
                if (auction == null)
                {
                    throw new Exception("The auction doesn't exist.");
                }
                if (!auction.Event.IsCurrent && auction.Event.CloseStep == 2)
                {
                    throw new Exception("You can't add/edit this bid. The auction is not open.");
                }
                var bd = dataContext.BidLogCurrents.FirstOrDefault(B => B.ID == bid_id);
                if (bd == null)
                {
                    bd = new BidLogCurrent();
                    dataContext.BidLogCurrents.InsertOnSubmit(bd);
                }
                bd.Auction_ID = auction_id;
                bd.Amount = Amount;
                bd.DateMade = DateMade;
                bd.IP = IP;
                bd.IsProxy = auction.AuctionType_ID == (byte) Consts.AuctionType.Normal ? IsProxy : false;
                bd.IsProxyRaise = auction.AuctionType_ID == (byte) Consts.AuctionType.Normal ? IsProxyRaise : false;
                bd.MaxBid = MaxBid;
                bd.Quantity = Quantity;
                bd.User_ID = UserID;
                bd.IsAutoBid = false;
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //DeleteBidLog
        public JsonExecuteResult DeleteBidLog(long bid_id, long auction_id)
        {
            var auction = dataContext.Auctions.SingleOrDefault(A => A.ID == auction_id);
            try
            {
                if (auction == null)
                {
                    throw new Exception("The auction doesn't exist.");
                }
                if (!auction.Event.IsCurrent && auction.Event.CloseStep == 2)
                {
                    throw new Exception("You can't place this bid. The auction is not open.");
                }

                var bc = dataContext.BidLogCurrents.Where(B => B.ID == bid_id).SingleOrDefault();
                if (bc == null)
                {
                    throw new Exception("The bid doesn't exist.");
                }
                dataContext.BidLogCurrents.DeleteOnSubmit(bc);
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch (Exception ex)
            {
                return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
            }
            return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        }

        //GetBidsAmountForAuction
        public string GetBidsAmountForAuction(long auction_id)
        {
            var sb = new StringBuilder();
            sb.Append("<select>");
            var auction = dataContext.Auctions.FirstOrDefault(a => a.ID == auction_id);
            var bidamount = auction.Price;
            for (var i = 0; i < 100; i++)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", bidamount.GetPrice(), bidamount.GetCurrency());
                bidamount += Consts.GetIncrement(bidamount);
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        //GetTopBidForItem
        private BidCurrent GetTopBidForItem(long auction_id)
        {
            dataContext.CommandTimeout = 600000;
            return dataContext.spBid_WinningBid(auction_id).FirstOrDefault();
        }

        //GetUserTopBidForItem
        private BidCurrent GetUserTopBidForItem(long auction_id, long user_id)
        {
            dataContext.CommandTimeout = 600000;
            return dataContext.spBid_UserTopBid(user_id, auction_id).FirstOrDefault();
        }

        //PlaceSingleBid
        private BiddingObject PlaceSingleBid(long auction_id, bool isproxy, decimal amount, long user_id, int quantity,
            bool isproxyraise, decimal aprice, BidCurrent prevUsersBid, BidCurrent lastTop)
        {
            BidCurrent newBid;
            var isHighBidder = (lastTop != null) ? lastTop.User_ID == user_id : false;
            amount = (lastTop != null &&
                      ((long) amount == (long) lastTop.MaxBid ||
                       Math.Abs(lastTop.MaxBid - amount) < Consts.ErrorRangeAmount))
                ? lastTop.MaxBid
                : amount;

            if (prevUsersBid != null)
            {
                newBid = prevUsersBid;
            }
            else
            {
                newBid = new BidCurrent();
                dataContext.BidCurrents.InsertOnSubmit(newBid);
            }
            if (!isproxy)
            {
                newBid.Amount = amount;
                if (newBid.MaxBid > amount)
                {
                    newBid.IsProxy = true;
                }
                else
                {
                    newBid.MaxBid = amount;
                    newBid.IsProxy = false;
                }
            }
            else
            {
                var price = (lastTop == null) ? aprice : lastTop.Amount;
                newBid.MaxBid = (newBid.MaxBid < amount) ? amount : newBid.MaxBid;
                newBid.Amount = (price + Consts.GetIncrement(price) <= newBid.MaxBid) ? price : newBid.MaxBid;
                newBid.Amount += (lastTop != null && !isHighBidder &&
                                  newBid.Amount + Consts.GetIncrement(price) <= newBid.MaxBid)
                    ? Consts.GetIncrement(price)
                    : 0;
                newBid.IsProxy = true;
            }
            newBid.Auction_ID = auction_id;
            newBid.User_ID = user_id;
            newBid.Quantity = quantity;
            newBid.DateMade = DateTime.Now;
            newBid.IP = Consts.UsersIPAddress;
            newBid.IsActive = true;

            var log = new BidLogCurrent();
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

            GeneralRepository.SubmitChanges(dataContext);

            return new BiddingObject {Bid = newBid, BidLog = log};
        }

        //ResolveProxyBiddingSituation
        private void ResolveProxyBiddingSituation(long auction_id, long user_id, bool isproxy, BiddingObject placedBid,
            BidCurrent lastTop, decimal aprice, List<BidLogCurrent> newbidlogs)
        {
            if (placedBid.Bid == null || placedBid.BidLog == null || lastTop == null)
            {
                return;
            }
            var bids = dataContext.spBid_BidsExceptCurrent(auction_id, placedBid.Bid.ID, user_id).ToList();
            if (bids.Count() == 0)
            {
                return;
            }

            var price = (lastTop == null) ? aprice : lastTop.Amount;
            var Increment = Consts.GetIncrement(price);

            var queryBid = bids.FirstOrDefault();

            if (placedBid.Bid.IsProxy && placedBid.Bid.Amount <= (queryBid.MaxBid + Consts.ErrorRangeAmount))
            {
                if (queryBid.MaxBid + Increment >= (placedBid.Bid.MaxBid + Consts.ErrorRangeAmount))
                {
                    //Can't overBid
                    placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
                }
                else
                {
                    Increment = Consts.GetIncrement(queryBid.MaxBid);
                    if (Increment + queryBid.MaxBid + Consts.ErrorRangeAmount > placedBid.Bid.MaxBid)
                    {
                        placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
                    }
                    else if (placedBid.Bid.User_ID != lastTop.User_ID)
                    {
                        placedBid.BidLog.Amount = placedBid.Bid.Amount = queryBid.MaxBid + Increment;
                    }
                }
            }
            foreach (var runner in bids)
            {
                if (!runner.IsProxy)
                {
                    continue;
                }
                if ((runner.MaxBid == placedBid.Bid.MaxBid) ||
                    (Math.Abs(runner.MaxBid - placedBid.Bid.MaxBid) < Consts.ErrorRangeAmount))
                {
                    runner.Amount = placedBid.Bid.MaxBid;
                }
                else
                {
                    Increment = Consts.GetIncrement(placedBid.Bid.MaxBid);
                    if (runner.MaxBid >= placedBid.Bid.MaxBid + Increment + Consts.ErrorRangeAmount)
                    {
                        runner.Amount = placedBid.Bid.MaxBid + Increment;
                    }
                    else
                    {
                        if (runner.Amount != runner.MaxBid)
                        {
                            var log = new BidLogCurrent();
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
            GeneralRepository.SubmitChanges(dataContext);
        }

        //AddBidLogCurrent
        private BidLogCurrent AddBidLogCurrent(long auction_id, int quantity, long user_id, bool isproxy, decimal amount,
            decimal maxamount, bool isproxyraise, string IP)
        {
            var log = new BidLogCurrent();
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
                GeneralRepository.SubmitChanges(dataContext);
            }
            catch
            {
                return null;
            }
            return log;
        }

        //RemoveAuctionResultsCache
        public void RemoveAuctionResultsCache(long auction_id)
        {
            CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS,
                "GETAUCTIONDETAILRESULT", new object[] {auction_id}));
        }

        //UpdateAuctionBiddingResult
        private void UpdateAuctionBiddingResult(long auction_id, long? user_id, decimal? currentbid, decimal? maxbid)
        {
            try
            {
                var ar = dataContext.AuctionResultsCurrents.Where(AR => AR.Auction_ID == auction_id).FirstOrDefault();
                if (ar == null)
                {
                    ar = new AuctionResultsCurrent();
                    dataContext.AuctionResultsCurrents.InsertOnSubmit(ar);
                }
                ar.Auction_ID = auction_id;
                ar.User_ID = user_id;
                ar.CurrentBid = currentbid;
                ar.MaxBid = maxbid;
                ar.Bids =
                    (dataContext.spBid_LogCount(auction_id).FirstOrDefault() ?? new spBid_LogCountResult()).LogCount
                        .GetValueOrDefault(0);
                //dataContext.spUpdate_AuctionResults(ar.ID, ar.Auction_ID, ar.User_ID, ar.CurrentBid, ar.Bids, ar.MaxBid);
                dataContext.spUpdate_AuctionResultsCurrent(ar.ID, ar.Auction_ID, ar.User_ID, ar.CurrentBid, ar.Bids,
                    ar.MaxBid);
                //RemoveAuctionResultsCache(auction_id);
                try
                {
                    var client = new WebClient();
                    client.Headers.Add("user-agent",
                        "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearARPMethod + "/" +
                                    auction_id.ToString());
                }
                catch (Exception ex)
                {
                    Logger.LogException(
                        "[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearARPMethod + "/" + auction_id.ToString() +
                        "]", ex);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(
                    String.Format("[auction_id={0}; user={1}; cb={2}; maxbid={3}", auction_id, user_id, currentbid,
                        maxbid), ex);
            }
        }

        //GetAuctionDetail
        private AuctionDetail GetAuctionDetail(long auction_id, bool iscaching = true)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL",
                new object[] {auction_id}, CachingExpirationTime.Minutes_30);
            var ad = CacheRepository.Get(dco) as AuctionDetail;
            if (ad != null && iscaching)
            {
                return ad;
            }
            dataContext.CommandTimeout = 600000;
            ad = (from p in dataContext.spAuction_View_Detail(auction_id)
                select new AuctionDetail
                {
                    Lot = p.Lot,
                    Title = p.Title,
                    IsPulledOut = p.IsPulledOut,
                    Description = p.Description,
                    Addendum = p.Addendum,
                    Status = (Consts.AuctionStatus) p.Status.GetValueOrDefault(0),
                    DefaultImage = p.PicturePath,
                    IsCurrentEvent = p.IsCurrent,
                    DateStart = p.StartDate,
                    DateEnd = p.EndDate,
                    EventDateStart = p.EventStartDate,
                    EventDateEnd = p.EventEndDate,
                    CloseStep = p.CloseStep,
                    Owner_ID = p.Owner_ID,
                    Price = p.Price,
                    LinkParams =
                        new LinkParams
                        {
                            ID = p.Auction_ID,
                            Lot = p.Lot,
                            Title = p.Title,
                            EventTitle = p.EventTitle,
                            MainCategoryTitle = p.MainCategoryTitle,
                            CategoryTitle = p.CategoryTitle,
                            Event_ID = p.Event_ID,
                            EventCategory_ID = p.EventCategory_ID,
                            MainCategory_ID = p.MainCategory_ID,
                            Category_ID = p.Category_ID
                        },
                    PrevAuction =
                        (p.PrevAuction_ID.HasValue)
                            ? new LinkParams
                            {
                                ID = p.PrevAuction_ID.Value,
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.PrevMainCategoryTitle,
                                CategoryTitle = p.PrevCategoryTitle,
                                Lot = p.PrevLot.GetValueOrDefault(0),
                                Title = p.PrevTitle
                            }
                            : null,
                    NextAuction =
                        (p.NextAuction_ID.HasValue)
                            ? new LinkParams
                            {
                                ID = p.NextAuction_ID.Value,
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.NextMainCategoryTitle,
                                CategoryTitle = p.NextCategoryTitle,
                                Lot = p.NextLot.GetValueOrDefault(0),
                                Title = p.NextTitle
                            }
                            : null
                }).FirstOrDefault();
            if (ad != null)
            {
                ad.Collections = (from ac in dataContext.AuctionCollections
                    join c in dataContext.Collections on ac.CollectionID equals c.ID
                    where ac.AuctionID == auction_id
                    select new IdTitleDesc {ID = c.ID, Title = c.Title, Description = c.Description}).ToList();
                dco.Data = ad;
                CacheRepository.Add(dco);
            }
            return ad;
        }

        private byte BiddingForSingleAuction(AuctionDetail auction, BidCurrent currentBid, out BidCurrent previousBid,
            out BidCurrent loserBid, out BidCurrent winnerBid)
        {
            long? bid_id = -1;
            previousBid = loserBid = winnerBid = null;

            var allbids = dataContext.spBid_LotBids(auction.LinkParams.ID).ToList();

            // no bids
            if (!allbids.Any())
            {
                currentBid.Amount = currentBid.IsProxy ? auction.Price : currentBid.MaxBid;
                dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                    currentBid.Amount,
                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                    currentBid.IsProxy, false, currentBid.IsActive, null, null, null,
                    null, null, null, null, null, null, null, null, currentBid.User_ID,
                    currentBid.Amount, currentBid.MaxBid);
                currentBid.ID = bid_id.GetValueOrDefault(-1);
                winnerBid = new BidCurrent(currentBid);
                return 0;
            }

            previousBid = allbids.FirstOrDefault(q => q.User_ID == currentBid.User_ID);

            var topBid = allbids.First();

            // the duplicate
            if (previousBid != null)
            {
                currentBid.ID = previousBid.ID;
                if (previousBid.IsProxy == currentBid.IsProxy && previousBid.MaxBid >= currentBid.MaxBid &&
                    topBid.User_ID != currentBid.User_ID)
                {
                    return 3;
                }
            }

            // current is highbidder
            if (topBid.User_ID == currentBid.User_ID)
            {
                //currentBid.Amount = (!currentBid.IsProxy && currentBid.Amount > topBid.Amount) ? currentBid.Amount : topBid.Amount;
                //currentBid.MaxBid = Math.Max(currentBid.MaxBid, currentBid.Amount); for backend and for front end -> //currentBid.MaxBid = Math.Max(currentBid.MaxBid, topBid.MaxBid);
                currentBid.IsProxy = currentBid.Amount <= currentBid.MaxBid;
                bid_id = topBid.ID;
                dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                    currentBid.Amount,
                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                    currentBid.IsProxy, currentBid.MaxBid > topBid.MaxBid, currentBid.IsActive, null, null, null,
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
                amount = currentBid.IsProxy
                    ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid)
                    : currentBid.MaxBid;
                isautobid = topBid.Amount != topBid.MaxBid;
                topBid.Amount = topBid.MaxBid;
                currentBid.Amount = !isautobid
                    ? amount
                    : (currentBid.IsProxy
                        ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid)
                        : currentBid.MaxBid);
                bid_id = previousBid == null ? -1 : currentBid.ID;
                if (isautobid)
                {
                    dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                        amount, currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                        currentBid.IsProxy, false, currentBid.IsActive, currentBid.Amount, currentBid.MaxBid,
                        DateTime.Now,
                        topBid.ID, topBid.User_ID, topBid.Amount, topBid.MaxBid, topBid.DateMade, DateTime.Now,
                        topBid.IP, topBid.IsProxy,
                        currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
                }
                else
                {
                    dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                        currentBid.Amount,
                        currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                        currentBid.IsProxy, false, currentBid.IsActive, null, null, null, null, null, null, null, null,
                        null, null, null,
                        currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
                }
                currentBid.ID = bid_id.GetValueOrDefault(-1);
                loserBid = new BidCurrent(topBid);
                winnerBid = new BidCurrent(currentBid);
                return 0;
            }

            amount = currentBid.IsProxy
                ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid)
                : currentBid.MaxBid;
            isautobid = amount < currentBid.MaxBid;
            currentBid.Amount = currentBid.MaxBid;
            bid_id = previousBid == null ? -1 : currentBid.ID;

            if (topBid.Amount == topBid.MaxBid ||
                (topBid.Amount > currentBid.Amount && topBid.MaxBid > currentBid.MaxBid))
            {
                dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                    amount,
                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                    currentBid.IsProxy, false, currentBid.IsActive,
                    isautobid ? (decimal?) currentBid.Amount : null,
                    isautobid ? (decimal?) currentBid.MaxBid : null,
                    isautobid ? (DateTime?) DateTime.Now : null,
                    null, null, null, null, null, null, null, null, null, null, null);
            }
            else
            {
                topBid.Amount = Math.Min(currentBid.MaxBid + Consts.GetIncrement(currentBid.MaxBid), topBid.MaxBid);
                dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID,
                    amount,
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

        //PlaceBid

        //public IEnumerable<Bid> GetBidsForItem(long id)
        //{
        //  var query = from a in dataContext.Bids
        //              where a.Auction_ID == id
        //              select a;
        //  return query.Cast<Bid>();
        //}

        //public IEnumerable<BidLog> GetBidsHistoryForItem(long id)
        //{
        //  var query = from a in dataContext.BidLogs
        //              where a.Auction_ID == id
        //              orderby a.Amount descending, a.MaxBid descending, a.DateMade ascending
        //              select a;
        //  return (query.Count()>0)?query.Cast<BidLog>():null;
        //}

        //public User GetHighBidderForNormalItem(long id)
        //{
        //  var query = from u in dataContext.Users
        //              join b in dataContext.Bids on u.ID equals b.User_ID
        //              where b.Auction_ID == id
        //              orderby b.Amount descending
        //              select u;
        //  return (query.Count() > 0)? query.First<User>() : null;
        //}

        //public void PlaceDOWOrder(long id, int Quantity, long userid)
        //{
        //  //var query = (from p in dataContext.Auctions where p.ID == id select p);
        //  //Auction selectedAuction = query.FirstOrDefault();
        //  //if (selectedAuction == null || selectedAuction.Quantity<Quantity) return;
        //  //selectedAuction.Quantity -= Quantity;
        //  //dataContext.SubmitChanges();
        //  //    //dataContext.Auctions
        //  //    IInvoice newInvoice = new Invoice();
        //  //    newInvoice.Amount = selectedAuction.Price;
        //  //    newInvoice.Auction_ID = id;
        //  //    newInvoice.User_ID = userid;
        //  //    newInvoice.IsPaid = false;
        //  //    newInvoice.DateCreated = DateTime.Now;
        //  //    newInvoice.Quantity = Quantity;
        //  //    if (selectedAuction.Shipping != null)
        //  //    {
        //  //      newInvoice.Shipping = Convert.ToDecimal(selectedAuction.Shipping);
        //  //    }
        //  //    else
        //  //    {
        //  //      newInvoice.Shipping = 0;
        //  //    }
        //  //    newInvoice.Insurance = GetCoefficient("InsuranceCoefficient") * selectedAuction.Price / 100;
        //  //    var quearyUser = from p in dataContext.Users where p.ID == userid select p;
        //  //    if ((quearyUser.First<User>().AddressCard != null) && (quearyUser.First<User>().AddressCard.State == "FL"))
        //  //    {
        //  //      newInvoice.Tax = GetCoefficient("SalesTaxRate");
        //  //    }
        //  //    else
        //  //    {
        //  //      newInvoice.Tax = 0;
        //  //    }
        //  //    dataContext.Invoices.InsertOnSubmit(newInvoice as Invoice);
        //  //    dataContext.SubmitChanges();
        //  //  }      
        //}

        //public decimal GetCoefficient(string name)
        //{
        //  var q = from a in dataContext.Variables
        //          where a.Name == name
        //          select a;
        //  return q.First().Value;
        //}

        //public void UpdateDOWAfterFail(long invoiceID)
        //{
        //  //var q = from a in dataContext.Invoices
        //  //        where a.ID == invoiceID
        //  //        select a;
        //  //IInvoice failedInvoice = q.Single();

        //  //var b = from a in dataContext.Auctions
        //  //        where a.ID == failedInvoice.Auction_ID
        //  //        select a;
        //  //b.Single().Quantity += failedInvoice.Quantity;

        //  //q.Single().Comments = "20 minutes time out";
        //  //dataContext.SubmitChanges();
        //}

        //public IEnumerable<IInvoice> GetDOWFailed()
        //{
        //  //var q = from a in dataContext.Invoices
        //  //        join b in dataContext.Auctions on a.Auction_ID equals b.ID
        //  //        where b.AuctionType_ID == (long)Consts.AuctionType.DealOfTheWeek
        //  //        && a.IsPaid == false && a.DateCreated < DateTime.Now.AddMinutes(-20)
        //  //        select a;
        //  return null;// q.Cast<IInvoice>();
        //}

        //public BidRepository(VauctionDataContext dataContext)
        //{
        //  this.dataContext = dataContext;
        //}

        //public IEnumerable<Bid> GetDutchWinnerTableBidForItem(long id)
        //{
        //  return dataContext.GetDutchAuctionWinnerTable(id).ToList<Bid>();
        //}

        //public decimal GetMinimumBidForDutchItem(long id)
        //{
        //  ////TODO ERROR!!! need understand why...
        //  //IEnumerable<Bid> table = dataContext.GetDutchAuctionWinnerTable(id);
        //  //var q = from a in dataContext.Auctions
        //  //        where a.ID == id
        //  //        select a;
        //  //decimal Min = q.First().Price;
        //  //foreach (Bid i in table)
        //  //{
        //  //  Min = i.Amount;
        //  //}
        //  return 0;//Min;
        //}

        //public decimal GetMinimumBidForNormalItem(long id, long UserId)
        //{
        //  var z = from a in dataContext.Auctions
        //          where a.ID == id
        //          select a;
        //  Auction currentAuction = z.FirstOrDefault();

        //  var q = from a in dataContext.Bids
        //          where a.Auction_ID == id
        //          orderby a.Amount descending, a.MaxBid descending, a.DateMade ascending
        //          select a;
        //  Bid bid = q.FirstOrDefault();
        //  decimal MinBid = 0;
        //  if (bid==null)
        //  { 
        //    decimal Increment = 0;
        //    Increment = (currentAuction.Increment == null || currentAuction.Increment == 0)?Consts.GetIncrement(currentAuction.Price):Convert.ToDecimal(z.First().Increment);

        //    MinBid = (currentAuction.Bids != null && currentAuction.Bids.Count > 0)? currentAuction.Price - Increment:currentAuction.Price;
        //  }
        //  else
        //    MinBid = bid.Amount;

        //  if (UserId != -1)
        //  {
        //    var q2 = from a in dataContext.Bids
        //             where a.Auction_ID == id && a.User_ID == UserId
        //             orderby a.Amount descending, a.MaxBid descending, a.DateMade ascending
        //             select a;
        //    Bid bd = q2.FirstOrDefault();
        //    if (bd!=null)
        //    {
        //      if ((MinBid < bd.MaxBid) && (bd.IsProxy.HasValue && !bd.IsProxy.Value))
        //      {
        //        MinBid = bd.MaxBid;
        //      }
        //    }
        //  }
        //  return MinBid;
        //}

        //public bool IsBidWatchedByUser(long userId, long auctionId)
        //{
        //  var q = from a in dataContext.BidWatches
        //          where a.User_ID == userId && a.Auction_ID == auctionId
        //          select a;
        //  return (q.Count() > 0);
        //}

        //public IBid GetTopBidForItem(long id)
        //{
        //  var q = (from p in dataContext.Bids
        //           where p.Auction_ID == id
        //           orderby p.MaxBid descending, p.DateMade ascending
        //           select p);
        //  return q.FirstOrDefault();
        //}

        //public Int64 AddNew(BidLog newBid)
        //{
        //  dataContext.BidLogs.InsertOnSubmit(newBid);
        //  try
        //  {
        //    dataContext.SubmitChanges();
        //  }
        //  catch (ChangeConflictException e)
        //  {
        //    Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        //    foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        //    {
        //      occ.Resolve(RefreshMode.KeepCurrentValues);
        //    }
        //  }
        //  return newBid.ID;
        //}

        //public BidLog AddNew(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP )
        //{
        //  BidLog log = new BidLog();
        //  dataContext.BidLogs.InsertOnSubmit(log as BidLog);
        //  log.Quantity = quantity;
        //  log.User_ID = user_id;
        //  log.IsProxy = isproxy;
        //  log.MaxBid = maxamount;
        //  log.Amount = amount;
        //  log.IP = String.IsNullOrEmpty(IP)?Consts.UsersIPAddress:IP;
        //  log.Auction_ID = auction_id;
        //  log.DateMade = DateTime.Now;
        //  log.IsProxyRaise = isproxyraise;

        //  try
        //  {
        //    dataContext.SubmitChanges();
        //  }
        //  catch (ChangeConflictException e)
        //  {
        //    Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        //    foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        //    {
        //      occ.Resolve(RefreshMode.KeepCurrentValues);
        //    }
        //  }
        //  return log; 
        //}

        //public IBid GetBid(Int64 id)
        //{
        //  var query = from p in dataContext.Bids where p.ID == id select p;
        //  return (query.Count() > 0)? query.First() : null;
        //}

        //public BidLog GetBidLog(Int64 id)
        //{
        //  var query = from p in dataContext.BidLogs where p.ID == id select p;
        //  return (query.Count() > 0) ? query.First() : null;
        //}

        //public bool Update(Bid updBid)
        //{
        //  var query = from p in dataContext.Bids where p.ID == updBid.ID select p;
        //  if (query.Count() == 0) return false;      
        //  Bid bid = query.First();
        //  bid.Amount = updBid.Amount;
        //  bid.User_ID = updBid.User_ID;
        //  bid.DateMade = updBid.DateMade;
        //  bid.IP = updBid.IP;
        //  bid.IsProxy = updBid.IsProxy;
        //  bid.IsReserveBid = updBid.IsReserveBid;
        //  //bid.IsRetracted = updBid.IsRetracted;
        //  bid.MaxBid = updBid.MaxBid;
        //  bid.Quantity = updBid.Quantity;
        //  bool result = true;
        //  try
        //  {
        //    dataContext.SubmitChanges();
        //  }
        //  catch (ChangeConflictException e)
        //  {
        //    Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        //    foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        //    {          
        //      occ.Resolve(RefreshMode.KeepCurrentValues);
        //    }
        //    result = false;
        //  }
        //  return result;      
        //}

        //public bool Update(BidLog updBid)
        //{
        //  var query = from p in dataContext.BidLogs where p.ID == updBid.ID select p;
        //  if (query.Count() == 0) return false;
        //  BidLog bid = query.First();
        //  if (bid.IsRetracted) return false;
        //  bid.Amount = updBid.Amount;
        //  bid.User_ID = updBid.User_ID;
        //  bid.DateMade = updBid.DateMade;
        //  bid.IP = updBid.IP;
        //  bid.IsProxy = updBid.IsProxy;
        //  bid.MaxBid = updBid.MaxBid;
        //  bid.IsProxyRaise = updBid.IsProxyRaise;
        //  bid.Quantity = updBid.Quantity;
        //  if (updBid.IsRetracted)
        //  {
        //    bid.IsRetracted = updBid.IsRetracted;
        //    bid.RetDate = updBid.RetDate;
        //    bid.RetractIP = updBid.RetractIP;
        //  }      
        //  bool result = true;
        //  try
        //  {
        //    dataContext.SubmitChanges();
        //  }
        //  catch (ChangeConflictException e)
        //  {
        //    Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        //    foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        //    {
        //      occ.Resolve(RefreshMode.KeepCurrentValues);
        //    }
        //    result = false;
        //  }
        //  return result;
        //}

        //public IBid PlaceSingleBid(long AuctionId, bool ProxyBid, decimal BidToPlace, long UserId, int Quantity, bool IsRaiseProxy)
        //{
        //  var query = from p in dataContext.Auctions
        //              where p.ID == AuctionId
        //              select p;
        //  Auction currentAuction = query.FirstOrDefault();
        //  if (currentAuction == null) return null;
        //  var secquery = from b in dataContext.Bids
        //                 where b.User_ID == UserId && b.Auction_ID == AuctionId
        //                 select b;
        //  Bid secQuery = secquery.FirstOrDefault();

        //  var user = from b in dataContext.Users
        //             where b.ID == UserId
        //             select b;

        //  IBid newBid;
        //  bool isHighBidder = (user.Count() > 0 && String.Compare(user.First().Login, currentAuction.HighBid) == 0);

        //  if (currentAuction.AuctionType_ID == (long)Consts.AuctionType.Dutch)
        //  { ProxyBid = true; }

        //  if ((currentAuction.AuctionType_ID == (long)Consts.AuctionType.Dutch && secQuery!=null) || (secQuery!=null))
        //  {
        //    newBid = secQuery;
        //  }
        //  else
        //  {
        //    newBid = new Bid();
        //    dataContext.Bids.InsertOnSubmit(newBid as Bid);
        //  }

        //  if (!ProxyBid)
        //  {
        //    newBid.Amount = BidToPlace;
        //    if (newBid.MaxBid > BidToPlace)
        //    {
        //      newBid.IsProxy = true;
        //    }
        //    else
        //    {
        //      newBid.MaxBid = BidToPlace;
        //      newBid.IsProxy = false;
        //    }
        //  }
        //  else
        //  {
        //    decimal price = (currentAuction.Bids.Count==0) ? currentAuction.Price : Convert.ToDecimal(currentAuction.CurBid);
        //    newBid.MaxBid = (newBid.MaxBid < BidToPlace) ? BidToPlace : newBid.MaxBid;
        //    newBid.Amount = (price + Consts.GetIncrement(price) <= newBid.MaxBid) ? price : newBid.MaxBid;        
        //    newBid.Amount += (currentAuction.Bids.Count>0 && !isHighBidder && newBid.Amount + Consts.GetIncrement(price) <= newBid.MaxBid) ? Consts.GetIncrement(price) : 0;
        //    newBid.IsProxy = true;
        //  }
        //  newBid.Auction_ID = AuctionId;
        //  newBid.User_ID = UserId;
        //  newBid.Quantity = Quantity;
        //  newBid.DateMade = DateTime.Now;
        //  newBid.IP = System.Web.HttpContext.Current.Request.UserHostAddress;

        //  BidLog log = new BidLog();
        //  dataContext.BidLogs.InsertOnSubmit(log as BidLog);
        //  log.Quantity = Quantity;
        //  log.User_ID = newBid.User_ID;
        //  log.IsProxy = ProxyBid;
        //  log.MaxBid = newBid.MaxBid;
        //  log.Amount = newBid.Amount;
        //  log.IP = newBid.IP;
        //  log.Auction_ID = newBid.Auction_ID;
        //  log.DateMade = DateTime.Now;
        //  log.IsProxyRaise = IsRaiseProxy;

        //  try
        //  {
        //    dataContext.SubmitChanges();
        //  }
        //  catch (ChangeConflictException e)
        //  {
        //    Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        //    foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        //    {
        //      occ.Resolve(RefreshMode.KeepCurrentValues);
        //    }
        //  }
        //  return newBid;     
        //}

        //public IEnumerable<sp_GetWatchBidForUserResult> GetBidWatchForUser(long p)
        //{
        //  return dataContext.sp_GetWatchBidForUser(p).ToList();
        //}

        //public decimal GetDepositNeededForUser(decimal BidToPlace, long User_ID)
        //{
        //  //var queryDone = (from a in dataContext.WinningBids
        //  //                 where a.User_ID == User_ID
        //  //                 select a.Amount).Sum(); //Bid sum
        //  //return (queryDone + BidToPlace) / 10;
        //  return 0;
        //}

        //public IBid GetUsersBidForAuction(long user_id, long auction_id)
        //{
        //  return (from a in dataContext.Bids
        //          where a.User_ID == user_id && a.Auction_ID == auction_id
        //          select a).FirstOrDefault();
        //}

        //public bool DepositNeeded(decimal BidAmount, long User_ID, long Auction_ID)
        //{
        //  //var NotNullQuery = (from a in dataContext.Payments
        //  //                    where a.User_ID == User_ID && a.PaidDate != null
        //  //                    && a.Amount != null
        //  //                    select a.Amount).Count();
        //  //decimal SumQuDone = 0;
        //  //var queryDone = (from a in dataContext.WinningBids
        //  //                 where a.User_ID == User_ID && a.Amount != null
        //  //                 select a.Amount); //Bid sum
        //  //if (queryDone.Count() > 0)
        //  //{
        //  //  SumQuDone = queryDone.Sum();
        //  //}
        //  //if (NotNullQuery != 0)
        //  //{
        //  //  var quary = (from a in dataContext.Payments
        //  //               where a.User_ID == User_ID && a.PaidDate != null
        //  //               && a.Amount != 0
        //  //               select a.Amount).Sum(); // PaidSum
        //  //  if ((quary > 2500) && (SumQuDone + BidAmount) < Consts.TopDepositLimit)
        //  //    return false;
        //  //}
        //  //if (SumQuDone + BidAmount < Consts.BottomDepositLimit)
        //  //{
        //  //  return false;
        //  //}
        //  //if (SumQuDone + BidAmount > Consts.TopDepositLimit)
        //  //{
        //  //  return true;
        //  //}
        //  return false;//true;
        //}
    }
}