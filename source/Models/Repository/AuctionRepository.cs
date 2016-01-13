using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Vauction.Models.CustomModels;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Paging;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
    public class AuctionRepository : IAuctionRepository
    {
        #region init

        private ICacheDataProvider CacheRepository;
        private VauctionDataContext dataContext;

        public AuctionRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
        {
            this.dataContext = dataContext;
            this.CacheRepository = CacheRepository;
        }

        //SubmitChanges
        private void SubmitChanges()
        {
            try
            {
                dataContext.SubmitChanges();
            }
            catch (ChangeConflictException e)
            {
                Logger.LogWarning(e.Message);
                foreach (var occ in dataContext.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                }
            }
        }

        #endregion

        #region search

        //GetByCriterias
        public IEnumerable<AuctionShort> GetByCriterias(AuctionFilterParams filter)
        {
            int? totalrecords = 0;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            var ash = (!filter.SeachType.HasValue)
                ? GetByCriterias(filter.Lot, filter.Title, (int) filter.Sortby,
                    filter.Orderby == Consts.OrderByValues.descending, filter.Event_ID.Value, pageindex, filter.PageSize,
                    out totalrecords)
                : GetByCriterias(filter, (int) filter.Sortby, filter.Orderby == Consts.OrderByValues.descending,
                    pageindex, filter.PageSize, out totalrecords);
            return new PagedList<AuctionShort>(ash, pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        //GetByCriterias
        private List<AuctionShort> GetByCriterias(string lot, string title, int sortby, bool ordrby, long event_id,
            int pageindex, int pagesize, out int? totalrecord)
        {
            title = String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%");
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETBYCRITERIAS",
                new object[] {lot, title, sortby, ordrby, event_id, pageindex, pagesize},
                CachingExpirationTime.Seconds_30);
            var result = CacheRepository.Get(dco) as TableViewResult;
            if (result != null && result.TotalRecords > 0)
            {
                totalrecord = result.TotalRecords;
                return result.Records;
            }
            result = new TableViewResult();
            totalrecord = 0;
            dataContext.CommandTimeout = 600000;
            result.Records =
                (from p in
                    dataContext.spAuction_View_Search(event_id, lot, title, String.Empty, -1, -1, sortby - 1, ordrby,
                        pageindex, pagesize, ref totalrecord)
                    select new AuctionShort
                    {
                        Bids = p.Bids.GetValueOrDefault(0),
                        CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.GetValueOrDefault(false),
                        IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                        IsUnsoldOrPulledOut =
                            p.IsUnsold.GetValueOrDefault(false) || p.IsPulledOut.GetValueOrDefault(false),
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.GetValueOrDefault(0),
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.GetValueOrDefault(0),
                        PriceRealized = p.PriceRealized.GetValueOrDefault(0),
                        PulledOut = p.IsPulledOut.GetValueOrDefault(false),
                        Status = p.AuctionStatus.GetValueOrDefault(0),
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.GetValueOrDefault(false) ? "UNSOLD" : "WITHDRAWN"
                    }).ToList();
            result.TotalRecords = totalrecord.GetValueOrDefault(0);
            if (result.TotalRecords > 0)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result.Records;
        }

        //GetByCriterias (advanced)
        private List<AuctionShort> GetByCriterias(AuctionFilterParams filter, int sortby, bool ordrby, int pageindex,
            int pagesize, out int? totalrecord)
        {
            filter.Title = String.IsNullOrEmpty(filter.Title) ? String.Empty : filter.Title.Replace(" ", "%");
            filter.Description = String.IsNullOrEmpty(filter.Description)
                ? String.Empty
                : filter.Description.Replace(" ", "%");
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETBYCRITERIAS",
                new object[]
                {
                    filter.Event_ID, filter.Type == "l" ? filter.Lot : String.Empty,
                    filter.Type == "t" || filter.Type == "td" ? filter.Title : String.Empty,
                    filter.Type == "d" || filter.Type == "td" ? filter.Description : String.Empty,
                    filter.SelectedCategory, -1, sortby - 1, ordrby,
                    pageindex, pagesize
                }, CachingExpirationTime.Seconds_30);

            var result = CacheRepository.Get(dco) as TableViewResult;
            if (result != null && result.TotalRecords > 0)
            {
                totalrecord = result.TotalRecords;
                return result.Records;
            }
            result = new TableViewResult();
            totalrecord = 0;
            dataContext.CommandTimeout = 600000;
            result.Records =
                (from p in
                    dataContext.spAuction_View_Search(filter.Event_ID, filter.Type == "l" ? filter.Lot : String.Empty,
                        filter.Type == "t" || filter.Type == "td" ? filter.Title : String.Empty,
                        filter.Type == "d" || filter.Type == "td" ? filter.Description : String.Empty,
                        filter.SelectedCategory, -1, sortby - 1, ordrby, pageindex, pagesize, ref totalrecord)
                    select new AuctionShort
                    {
                        Bids = p.Bids.GetValueOrDefault(0),
                        CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.GetValueOrDefault(false),
                        IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                        IsUnsoldOrPulledOut =
                            p.IsUnsold.GetValueOrDefault(false) || p.IsPulledOut.GetValueOrDefault(false),
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.GetValueOrDefault(0),
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.GetValueOrDefault(0),
                        PriceRealized = p.PriceRealized.GetValueOrDefault(0),
                        PulledOut = p.IsPulledOut.GetValueOrDefault(false),
                        Status = p.AuctionStatus.GetValueOrDefault(0),
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.GetValueOrDefault(false) ? "UNSOLD" : "WITHDRAWN"
                    }).ToList();
            result.TotalRecords = totalrecord.GetValueOrDefault(0);
            if (result.TotalRecords > 0)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result.Records;
        }

        #endregion

        #region past auctions for event

        //GetPastListForEvent
        public IEnumerable<AuctionShort> GetPastListForEvent(CategoryFilterParams filter)
        {
            int? totalrecords = 0;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            var ash =
                (from p in
                    dataContext.spAuction_View_List((long) filter.Id, -1, 2, (int) filter.Sortby - 1,
                        (byte) filter.Orderby == 2, pageindex, filter.PageSize, ref totalrecords)
                    select new AuctionShort
                    {
                        Bids = p.Bids.Value,
                        CurrentBid = p.CurrentBid.Value,
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.Value,
                        IsFeatured = p.IsFeatured.Value,
                        IsUnsoldOrPulledOut = p.IsUnsold.Value || p.IsPulledOut.Value,
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.Value,
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.Value,
                        PriceRealized = p.PriceRealized.Value,
                        PulledOut = p.IsPulledOut.Value,
                        Status = p.AuctionStatus.Value,
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.Value ? "UNSOLD" : "WITHDRAWN"
                    });
            return new PagedList<AuctionShort>(ash.ToList(), pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        #endregion

        #region auctions for category (+past)

        //GetPastListForCategory
        public IEnumerable<AuctionShort> GetPastListForCategory(CategoryFilterParams filter)
        {
            int? totalrecords = 0;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            var ash = GetListForCategory(filter.Id, true, (int) filter.Sortby - 1, (byte) filter.Orderby == 2, pageindex,
                filter.PageSize, out totalrecords);
            return new PagedList<AuctionShort>(ash, pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        //GetListForCategory
        public IEnumerable<AuctionShort> GetListForCategory(CategoryFilterParams filter)
        {
            int? totalrecords = 0;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            var ash = GetListForCategory(filter.Id, false, (int) filter.Sortby - 1, (byte) filter.Orderby == 2,
                pageindex, filter.PageSize, out totalrecords);
            return new PagedList<AuctionShort>(ash, pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        //GetListForCategory

        //UpdateCategoryViewResults
        public object UpdateCategoryViewResults(string prms)
        {
            if (String.IsNullOrEmpty(prms)) return "null";
            var res = new List<object>();
            try
            {
                var prm = prms.Split(',');
                int? totalrecords = 0;
                var reslt = GetListForCategory(Convert.ToInt64(prm[0]), Convert.ToBoolean(prm[1]),
                    Convert.ToInt32(prm[2]), Convert.ToBoolean(prm[3]), Convert.ToInt32(prm[4]), Convert.ToInt32(prm[5]),
                    out totalrecords);
                var sb = new StringBuilder();
                foreach (var result in reslt)
                {
                    if (result == null) continue;
                    sb = new StringBuilder();
                    if (!result.PulledOut)
                        sb.Append((result.Status == (byte) Consts.AuctionStatus.Open
                            ? (result.Bids > 0 ? "Current Bids: " : "Opening Bids: ")
                            : "Price Realized: ") +
                                  (result.Bids == 0
                                      ? (result.Price.GetCurrency() +
                                         ((String.IsNullOrEmpty(result.Estimate)) ? "" : " *"))
                                      : (!result.IsUnsoldOrPulledOut ? result.CurrentBid.GetCurrency() : "UNSOLD")) +
                                  "<br class=\"br_clear\" />" +
                                  (result.Bids > 0
                                      ? "Bids: <span id='cv_b_" + result.LinkParams.ID.ToString() + "'>" +
                                        result.Bids.ToString()
                                      : String.Empty) + "</span>");
                    else sb.Append(result.UnsoldOrPulledOut);
                    res.Add(
                        new
                        {
                            id = result.LinkParams.ID,
                            col1 =
                                !result.IsUnsoldOrPulledOut
                                    ? ((result.Bids == 0)
                                        ? (result.Price.GetCurrency() +
                                           ((String.IsNullOrEmpty(result.Estimate)) ? "" : " *"))
                                        : Convert.ToDecimal(result.CurrentBid).GetCurrency())
                                    : result.UnsoldOrPulledOut,
                            col2 = result.Bids.ToString(),
                            col3 = sb.ToString()
                        });
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(prms, ex);
            }
            return res;
        }

        private List<AuctionShort> GetListForCategory(long category_id, bool ispast, int sort, bool ordrby,
            int pageindex, int pagesize, out int? totalrecords)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETLISTFORCATEGORY",
                new object[] {category_id, ispast, sort, ordrby, pageindex, pagesize}, CachingExpirationTime.Seconds_30);
            var result = CacheRepository.Get(dco) as TableViewResult;
            if (result != null && result.TotalRecords > 0)
            {
                totalrecords = result.TotalRecords;
                return result.Records;
            }
            result = new TableViewResult();
            totalrecords = 0;
            dataContext.CommandTimeout = 600000;
            result.Records =
                (from p in
                    dataContext.spAuction_View_List(-1, category_id, ispast ? 2 : 1, sort, ordrby, pageindex, pagesize,
                        ref totalrecords)
                    select new AuctionShort
                    {
                        Bids = p.Bids.GetValueOrDefault(0),
                        CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.GetValueOrDefault(false),
                        IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                        IsUnsoldOrPulledOut =
                            p.IsUnsold.GetValueOrDefault(false) || p.IsPulledOut.GetValueOrDefault(false),
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.GetValueOrDefault(0),
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.GetValueOrDefault(0),
                        PriceRealized = p.PriceRealized.GetValueOrDefault(0),
                        PulledOut = p.IsPulledOut.GetValueOrDefault(false),
                        Status = p.AuctionStatus.GetValueOrDefault(0),
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.GetValueOrDefault(false) ? "UNSOLD" : "WITHDRAWN"
                    }).ToList();
            result.TotalRecords = totalrecords.GetValueOrDefault(0);
            if (result.TotalRecords > 0)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result.Records;
        }

        #endregion

        //GetProductsListForTag
        public IEnumerable<AuctionShort> GetProductsListForTag(long eventID, bool isPast, TagFilterParams filter)
        {
            int? totalrecords;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            var ash = GetProductsForTag(eventID, filter.ID, isPast, (int) filter.Sortby - 1, (byte) filter.Orderby == 2,
                pageindex, filter.PageSize, out totalrecords);
            return new PagedList<AuctionShort>(ash, pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        //GetProductsForTag

        //GetProductsForTag
        public List<AuctionSales> GetProductsForSales(long eventID)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETPRODUCTSFORSALES",
                new object[] {eventID}, CachingExpirationTime.Seconds_30);
            var result = CacheRepository.Get(dco) as List<AuctionSales>;
            if (result != null && result.Any())
            {
                return result;
            }
            dataContext.CommandTimeout = 600000;
            result = (from p in dataContext.spAuction_View_Sales(eventID)
                select new AuctionSales
                {
                    LinkParams =
                        new LinkParams
                        {
                            ID = p.Auction_ID,
                            EventTitle = p.EventTitle,
                            MainCategoryTitle = p.MainCategoryTitle,
                            CategoryTitle = p.CategoryTitle
                        },
                    Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                    Price = p.Price,
                    ThumbnailPath = p.ThumbnailPath,
                    Title = p.Title,
                    Estimate = p.Estimate
                }).ToList();
            if (result.Any())
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetAuctionUpdates
        public List<AuctionUpdate> GetAuctionUpdates(long event_id)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONUPDATES",
                new object[] {event_id}, CachingExpirationTime.Hours_01);
            var result = CacheRepository.Get(dco) as List<AuctionUpdate>;
            if (result != null && result.Count() > 0) return result;
            result = (from p in dataContext.spAuction_Updates(event_id)
                select new AuctionUpdate
                {
                    Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                    Title = p.Title,
                    Addendum = p.Addendum,
                    IsPulledOut = p.IsPulledOut,
                    LinkParams =
                        new LinkParams
                        {
                            ID = p.Auction_ID,
                            EventTitle = p.EventTitle,
                            MainCategoryTitle = p.MainCategoryTitle,
                            CategoryTitle = p.CategoryTitle
                        }
                }).ToList();
            if (result.Count() > 0)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetAuctionListForSeller
        public IEnumerable<AuctionShort> GetAuctionListForSeller(AuctionFilterParams filter, long user_id)
        {
            int? totalrecords = 0;
            var pageindex = (filter.page > 0) ? filter.page - 1 : 0;
            dataContext.CommandTimeout = 600000;
            var ash =
                (from p in
                    dataContext.spAuction_View_Search(filter.Event_ID, filter.Type == "l" ? filter.Lot : String.Empty,
                        filter.Type == "t" || filter.Type == "td" ? filter.Title : String.Empty,
                        filter.Type == "d" || filter.Type == "td" ? filter.Description : String.Empty,
                        filter.SelectedCategory, user_id, (int) filter.Sortby - 1, (byte) filter.Orderby == 2, pageindex,
                        filter.PageSize, ref totalrecords)
                    select new AuctionShort
                    {
                        Bids = p.Bids.GetValueOrDefault(0),
                        CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.GetValueOrDefault(false),
                        IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                        IsUnsoldOrPulledOut =
                            p.IsUnsold.GetValueOrDefault(false) || p.IsPulledOut.GetValueOrDefault(false),
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.GetValueOrDefault(0),
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.GetValueOrDefault(0),
                        PriceRealized = p.PriceRealized.GetValueOrDefault(0),
                        PulledOut = p.IsPulledOut.GetValueOrDefault(false),
                        Status = p.AuctionStatus.GetValueOrDefault(0),
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.GetValueOrDefault(false) ? "UNSOLD" : "WITHDRAWN"
                    });
            return new PagedList<AuctionShort>(ash.ToList(), pageindex, filter.PageSize,
                totalrecords.HasValue ? totalrecords.Value : 0);
        }

        #region auction detail page

        //GetAuctionDetail
        public AuctionDetail GetAuctionDetail(long auction_id, long currentevent_id, bool iscaching)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL",
                new object[] {auction_id, currentevent_id}, CachingExpirationTime.Minutes_30);
            var ad = CacheRepository.Get(dco) as AuctionDetail;
            if (ad != null && iscaching) return ad;
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
                    IsCurrentEvent = p.IsCurrent || currentevent_id == p.Event_ID,
                    DateStart = p.StartDate,
                    DateEnd = p.EndDate,
                    EventDateStart = p.EventStartDate,
                    EventDateEnd = p.EventEndDate,
                    CloseStep = p.CloseStep,
                    Owner_ID = p.Owner_ID,
                    Price = p.Price,
                    Estimate = p.Estimate,
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
                    select new IdTitleDesc {ID = c.ID, Title = c.WebTitle, Description = c.Description}).ToList();
                dco.Data = ad;
                CacheRepository.Add(dco);
            }
            return ad;
        }

        //GetAuctionDetail
        public AuctionDetail GetAuctionDetail(long auction_id, bool iscaching)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL",
                new object[] {auction_id}, CachingExpirationTime.Minutes_30);
            var ad = CacheRepository.Get(dco) as AuctionDetail;
            if (ad != null && iscaching) return ad;
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
                dco.Data = ad;
                CacheRepository.Add(dco);
            }
            return ad;
        }

        //GetAuctionDetailResult
        public AuctionShort GetAuctionDetailResult(long auction_id, bool iscaching)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAILRESULT",
                new object[] {auction_id}, CachingExpirationTime.Minutes_01);
            var result = CacheRepository.Get(dco) as AuctionShort;
            if (result != null && iscaching) return result;
            dataContext.CommandTimeout = 600000;
            result = (from a in dataContext.spAuction_View_DetailResult(auction_id)
                select new AuctionShort
                {
                    Bids = a.Bids,
                    CurrentBid = a.CurrentBid,
                    Estimate = a.Estimate,
                    IsUnsoldOrPulledOut = a.IsUnsold,
                    Price = a.Price,
                    PriceRealized = a.PriceRealized,
                    EndDate = a.EventDateEnd
                }).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetAuctionDetailResultPast
        public AuctionShort GetAuctionDetailResultPast(long auction_id)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS,
                "GETAUCTIONDETAILRESULTPAST",
                new object[] {auction_id}, CachingExpirationTime.Days_01);
            var result = CacheRepository.Get(dco) as AuctionShort;
            if (result != null) return result;
            result = GetAuctionDetailResult(auction_id, false);
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        //GetAuctionResult

        //UpdateAuctionBiddingResult
        public void UpdateAuctionBiddingResult(long auction_id, long user_id, decimal currentbid, decimal maxbid)
        {
            try
            {
                var ar = GetAuctionResult(auction_id);
                if (ar == null)
                {
                    ar = new AuctionResult();
                    dataContext.AuctionResults.InsertOnSubmit(ar);
                    ar.Auction_ID = auction_id;
                    ar.User_ID = user_id;
                    ar.CurrentBid = 0;
                    ar.MaxBid = 0;
                    ar.Bids = 0;
                    SubmitChanges();
                    CacheRepository.Put(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS,
                        "GETAUCTIONRESULT", new object[] {auction_id}, CachingExpirationTime.Minutes_01, ar));
                }
                ar.Auction_ID = auction_id;
                ar.User_ID = user_id;
                ar.CurrentBid = currentbid;
                ar.MaxBid = maxbid;
                ar.Bids = dataContext.spBid_LogCount(auction_id).FirstOrDefault().LogCount.GetValueOrDefault(0);
                dataContext.spUpdate_AuctionResults(ar.ID, ar.Auction_ID, ar.User_ID, ar.CurrentBid, ar.Bids, ar.MaxBid);
                var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONRESULT",
                    new object[] {auction_id}, CachingExpirationTime.Minutes_01, ar);
                CacheRepository.Put(dco);
                dco.Method = "GETAUCTIONDETAILRESULT";
                var result = CacheRepository.Get(dco) as AuctionShort;
                if (result != null)
                {
                    result.Bids = ar.Bids;
                    result.CurrentBid = ar.CurrentBid;
                    dco.Data = result;
                    CacheRepository.Put(dco);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(
                    String.Format("[auction_id={0}; user={1}; cb={2}; maxbid={3}", auction_id, user_id, currentbid,
                        maxbid), ex);
            }
        }

        //RemoveAuctionCash
        public void RemoveAuctionCache(long auctionID)
        {
            var auction = dataContext.Auctions.SingleOrDefault(a => a.ID == auctionID);
            if (auction == null) return;
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL",
                new object[] {auction.ID});
            CacheRepository.Remove(dco);

            dco.Method = "GETAUCTIONDETAILRESULTPAST";
            CacheRepository.Remove(dco);

            dco.Method = "GETAUCTIONDETAILRESULT";
            CacheRepository.Remove(dco);

            dco.Data = new object[] {auction.Event_ID};
            dco.Method = "GETAUCTIONUPDATES";
            CacheRepository.Remove(dco);

            dco.Region = DataCacheRegions.IMAGES;
            dco.Method = "GETAUCTIONIMAGES";
            CacheRepository.Remove(dco);

            CacheRepository.Remove(new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES,
                "GETCATEGORIESMENU", new object[] {0}));

            CacheRepository.Remove(new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES,
                "GETCATEGORIESMENU", new object[] {auction.Event_ID}));

            dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL",
                new object[] {auction.ID, auction.Event_ID});
            CacheRepository.Remove(dco);
        }

        //RemoveAuctionResultsCache
        public void RemoveAuctionResultsCache(long auctionID)
        {
            CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS,
                "GETAUCTIONDETAILRESULT", new object[] {auctionID}));
        }

        private AuctionResult GetAuctionResult(long auction_id)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONRESULT",
                new object[] {auction_id}, CachingExpirationTime.Minutes_01);
            var result = CacheRepository.Get(dco) as AuctionResult;
            if (result != null) return result;
            result = dataContext.spSelect_AuctionResults(auction_id).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        #endregion

        #region watch list

        //IsUserWatchItem

        //IsUserWatchItem
        public bool IsUserWatchItem(long user_id, long auction_id, bool iscache = true)
        {
            return GetBidWatch(user_id, auction_id, iscache) != null;
        }

        //AddItemToWatchList
        public void AddItemToWatchList(long user_id, long auction_id)
        {
            try
            {
                //BidWatch bw = GetBidWatch(user_id, auction_id, false);
                //if (bw != null) return;
                var bw = new BidWatchCurrent();
                dataContext.BidWatchCurrents.InsertOnSubmit(bw);
                bw.User_ID = user_id;
                bw.Auction_ID = auction_id;
                SubmitChanges();
                CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS,
                    "GETBIDWATCH", new object[] {user_id, auction_id}, CachingExpirationTime.Days_01, bw));
            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[user_id={0}, auction_id={1}]", user_id, auction_id), ex);
            }
        }

        //RemoveItemFromWatchList
        public bool RemoveItemFromWatchList(long user_id, long auction_id)
        {
            try
            {
                var bw = GetBidWatch(user_id, auction_id, true);
                if (bw == null) return false;
                dataContext.spDelete_BidWatchCurrent(bw.ID);
                CacheRepository.Remove(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS,
                    "GETBIDWATCH", new object[] {user_id, auction_id}, CachingExpirationTime.Days_01));
            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[user_id={0}, auction_id={1}]", user_id, auction_id), ex);
                return false;
            }
            return true;
        }

        private BidWatchCurrent GetBidWatch(long user_id, long auction_id, bool iscache)
        {
            var dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "GETBIDWATCH",
                new object[] {user_id, auction_id}, CachingExpirationTime.Days_01);
            var result = CacheRepository.Get(dco) as BidWatchCurrent;
            if (result != null && iscache) return result;
            result = dataContext.spSelect_BidWatchCurrent(auction_id, user_id).FirstOrDefault();
            if (result != null)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result;
        }

        #endregion

        private List<AuctionShort> GetProductsForTag(long eventID, long tagID, bool ispast, int sort, bool ordrby,
            int pageindex, int pagesize, out int? totalrecords)
        {
            var dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETPRODUCTSFORTAG",
                new object[] {eventID, tagID, ispast, sort, ordrby, pageindex, pagesize},
                CachingExpirationTime.Seconds_30);
            var result = CacheRepository.Get(dco) as TableViewResult;
            if (result != null && result.TotalRecords > 0)
            {
                totalrecords = result.TotalRecords;
                return result.Records;
            }
            result = new TableViewResult();
            totalrecords = 0;
            dataContext.CommandTimeout = 600000;
            result.Records =
                (from p in
                    dataContext.spAuction_View_Tag(eventID, tagID, ispast ? 2 : 1, sort, ordrby, pageindex, pagesize,
                        ref totalrecords)
                    select new AuctionShort
                    {
                        Bids = p.Bids.GetValueOrDefault(0),
                        CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                        Estimate = p.Estimate,
                        IsBold = p.IsBold.GetValueOrDefault(false),
                        IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                        IsUnsoldOrPulledOut =
                            p.IsUnsold.GetValueOrDefault(false) || p.IsPulledOut.GetValueOrDefault(false),
                        LinkParams =
                            new LinkParams
                            {
                                ID = p.Auction_ID.GetValueOrDefault(0),
                                EventTitle = p.EventTitle,
                                MainCategoryTitle = p.MainCategoryTitle,
                                CategoryTitle = p.CategoryTitle
                            },
                        Lot = p.Lot.HasValue ? p.Lot.Value : (short) 0,
                        Price = p.Price.GetValueOrDefault(0),
                        PriceRealized = p.PriceRealized.GetValueOrDefault(0),
                        PulledOut = p.IsPulledOut.GetValueOrDefault(false),
                        Status = p.AuctionStatus.GetValueOrDefault(0),
                        ThumbnailPath = p.ThumbnailPath,
                        Title = p.Title,
                        UnsoldOrPulledOut = p.IsUnsold.GetValueOrDefault(false) ? "UNSOLD" : "WITHDRAWN"
                    }).ToList();
            result.TotalRecords = totalrecords.GetValueOrDefault(0);
            if (result.TotalRecords > 0)
            {
                dco.Data = result;
                CacheRepository.Add(dco);
            }
            return result.Records;
        }
    }
}