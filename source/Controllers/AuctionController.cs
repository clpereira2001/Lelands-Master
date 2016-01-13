using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils.Helpers;
using Vauction.Utils;
using System;
using Relatives.Models.CustomBinders;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class AuctionController : BaseController
  {
    #region init
    private IAuctionRepository AuctionRepository;
    private IBidRepository BidRepository;
    private ICategoryRepository CategoryRepository;
    private IEventRepository EventRepository;
    private IDifferentRepository DifferentRepository;
    private IInvoiceRepository InvoiceRepository;

    public AuctionController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      BidRepository = dataProvider.BidRepository;
      CategoryRepository = dataProvider.CategoryRepository;
      EventRepository = dataProvider.EventRepository;
      DifferentRepository = dataProvider.DifferentRepository;
      InvoiceRepository = dataProvider.InvoiceRepository;
    }
    #endregion

    //Index
    [HttpGet, Compress]
    public ActionResult Index()
    {
      return RedirectToAction("Category");
    }

    #region Category
    //Category
    [Compress]
    public ActionResult Category()
    {
      InitCurrentEvent();
      return View();
    }

    //pCategory
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pCategory(long event_id)
    {
      ViewData["HTMLPage"] = CategoryRepository.GetCategoriesForCategoriesPage(event_id, true);
      return View("pHTMLPage");
    }

    //SubCategory
    [HttpGet, Compress]
    public ActionResult SubCategory(long id)
    {
      MainCategory mc = CategoryRepository.GetMainCategoryById(id);
      if (mc == null) return RedirectToAction("Category", "Auction");
      InitCurrentEvent();
      Event evnt = (ViewData["CurrentEvent"] as Event);
      EventCategory evCat = CategoryRepository.GetEventCategoryByMainCategory(id, evnt.ID) as EventCategory;
      return (evCat != null) ? RedirectToAction("CategoryView", new { @id = evCat.ID, @evnt = evnt.UrlTitle, @maincat = mc.UrlTitle }) : RedirectToAction("Category");
    }

    #endregion

    #region Category view
    // CategoryView
    [Compress]
    public ActionResult CategoryView([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param, string cat)
    {
      EventCategoryDetail ecd = CategoryRepository.GetEventCategoryDetail(param.Id);
      if (ecd == null) return RedirectToAction("Category");
      ViewData["ECDetail"] = ecd;
      ViewData["IsMainCategory"] = String.IsNullOrEmpty(cat);
      ViewData["MainCategory"] = ecd.LinkParams.MainCategory_ID;

      if (!String.IsNullOrEmpty(cat))
      {
        ViewData["Title"] = String.Format("{0} < {1} < {2} - {3}", ecd.LinkParams.CategoryTitle, ecd.LinkParams.MainCategoryTitle, ecd.LinkParams.EventTitle, Consts.CompanyTitleName);
        SetFilterParams(param);
      }
      else
      {
        ViewData["Title"] = String.Format("{0} < {1} - {2}", ecd.LinkParams.MainCategoryTitle, ecd.LinkParams.EventTitle, Consts.CompanyTitleName);
      }
      return View();
    }
    //pCategoryViewMainCategory
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pCategoryViewMainCategory(long event_id, int maincategory_id)
    {
      ViewData["HTMLPage"] = CategoryRepository.GetCategoriesForCategoriesByMainCategory(event_id, maincategory_id);
      return View("pHTMLPage");
    }
    //pCategoryView
    [ChildActionOnly]//, ActionOutputCache(CachingExpirationTime.Hours_01)
    public ActionResult pCategoryView(long event_id, bool iscurrentevent, CategoryFilterParams param, int page, int viewmode, int imageviewmode, int eventstep)
    {
      SetFilterParams(param);
      ViewData["IsPastGrid"] = !iscurrentevent && eventstep == 2;
      ViewData["PageActionPath"] = "CategoryView";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      return View("pAuctionGrid", AuctionRepository.GetPastListForCategory(param));
    }
    //pCategoryViewCurrent
    [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_01min)
    public ActionResult pCategoryViewCurrent(long event_id, bool iscurrentevent, CategoryFilterParams param, int page, int viewmode, int imageviewmode, int eventstep)
    {
      SetFilterParams(param);
      ViewData["IsPastGrid"] = !iscurrentevent && eventstep == 2;
      ViewData["PageActionPath"] = "CategoryView";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      ViewData["PageParams"] = String.Join(",", (new object[] { param.Id, false, (int)param.Sortby - 1, (byte)param.Orderby == 2, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
      return View("pAuctionGrid", AuctionRepository.GetListForCategory(param));
    }
    //UpdateCategoryViewResult
    [HttpPost, Compress]
    public JsonResult UpdateCategoryViewResult(string prms)
    {
      return JSON(AuctionRepository.UpdateCategoryViewResults(prms));
    }
    #endregion

    //Tag
    public ActionResult Tcategory([ModelBinder(typeof(CustomItemBinder))]TagFilterParams param)
    {
      var tag = DifferentRepository.GetTag(param.ID);
      if (tag == null || !tag.IsViewable) return RedirectToAction("Index", "Home");
      ViewData["Tag"] = tag;
      var evnt = param.EventID.HasValue ? EventRepository.GetEventByID(param.EventID.Value) : EventRepository.GetCurrent();
      ViewData["Title"] = string.Format("{0} < {1}", tag.Title, evnt.Title);
      ViewData["Event"] = evnt;
      SetFilterParams(param);
      return View();
    }

    //TagProducts
    [ChildActionOnly]
    public ActionResult TagProducts(TagFilterParams param)
    {
      SetFilterParams(param);
      var evnt = param.EventID.HasValue ? EventRepository.GetEventByID(param.EventID.Value) : EventRepository.GetCurrent();
      bool isPast = !evnt.IsCurrent && evnt.CloseStep == 2;
      ViewData["IsPastGrid"] = isPast;
      ViewData["PageActionPath"] = "Tcategory";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      ViewData["PageParams"] = String.Join(",", (new object[] { param.ID, param.EventID, false, (int)param.Sortby - 1, (byte)param.Orderby == 2, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
      return View("pAuctionGrid", AuctionRepository.GetProductsListForTag(evnt.ID, isPast, param));
    }

    #region Dynaminc/HTML Menus & Tree
    //GetCategoriesTree
    [NonAction]
    [HttpGet, Compress]
    public ContentResult GetCategoriesTree()
    {
      return Content(CategoryRepository.GetCategoriesTree(null));
    }
    //GetCategoriesMenu
    [NonAction]
    [HttpGet, Compress]
    public ContentResult GetCategoriesMenu()
    {
      return Content(CategoryRepository.GetCategoriesMenu(null, true));
    }

    //pCategoryTree
    [ChildActionOnly]
    public ActionResult pCategoryTree(long event_id, int maincategory_id)
    {
      ViewData["CatTree"] = CategoryRepository.GetCategoriesTree(event_id);
      return View("pCategoryTree", maincategory_id);
    }

    //pCategoryTree
    [ChildActionOnly]
    public ActionResult pCategoryMenu()
    {
      ViewData["CatMenu"] = CategoryRepository.GetCategoriesMenu(null, true);
      return View("pCategoryMenu");
    }
    #endregion

    #region Past auctions

    #region past auction results
    //PastAuctionResults
    [HttpGet, Compress]
    public ActionResult PastAuctionResults()
    {
      return View();
    }
    //pPastAuctionResults
    [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_01day)
    public ActionResult pPastAuctionResults()
    {
      return View("pPastAuctionResults", EventRepository.GetPastedEventsList());
    }
    #endregion

    //pPastAuctionResults
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Days_01)]
    public ActionResult pPastAuctionMenu(long event_id, string event_title)
    {
      ViewData["EventTitleUrl"] = event_title;
      ViewData["EventID"] = event_id;
      IEnumerable<IdTitleCount> tags;
      var categories = CategoryRepository.GetCategoriesForEvent(event_id, true, out tags);
      ViewData["Tags"] = tags;
      return View("pPastAuctionMenu", categories);
    }

    #region auction results
    //AuctionResults
    [HttpGet, Compress]
    public ActionResult AuctionResults([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param)
    {
      ViewData["Event"] = EventRepository.GetEventByID(param.Id);
      if (ViewData["Event"] == null) return RedirectToAction("PastAuctionResults");
      SetFilterParams(param);
      return View();
    }
    //pAuctionResults
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pAuctionResults(long event_id, CategoryFilterParams param, int page, int viewmode, int imageviewmode)
    {
      SetFilterParams(param);
      ViewData["IsPastGrid"] = true;
      ViewData["PageActionPath"] = "AuctionResults";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      return View("pAuctionGrid", AuctionRepository.GetPastListForEvent(param));
    }
    #endregion

    #region past categories view
    //PastCategoriesView
    [HttpGet, Compress]
    public ActionResult PastCategoriesView([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param)
    {
      //EventCategory evCat = CategoryRepository.GetEventCategoryById(param.Id);
      EventCategoryDetail ecd = CategoryRepository.GetEventCategoryDetail(param.Id);
      if (ecd == null) return RedirectToAction("PastAuctionResults");
      ViewData["Event"] = EventRepository.GetEventByID(ecd.LinkParams.Event_ID);
      ViewData["EventCategory"] = ecd;
      ViewData["Title"] = String.Format("{0} < {1} < {2} - {3}", ecd.LinkParams.CategoryTitle, ecd.LinkParams.MainCategoryTitle, ecd.LinkParams.EventTitle, Consts.CompanyTitleName);
      SetFilterParams(param);
      return View();
    }
    //pPastCategoriesView
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pPastCategoriesView(long event_id, CategoryFilterParams param, int page, int viewmode, int imageviewmode)
    {
      SetFilterParams(param);
      ViewData["IsPastGrid"] = true;
      ViewData["PageActionPath"] = "PastCategoriesView";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      return View("pAuctionGrid", AuctionRepository.GetPastListForCategory(param));
    }
    #endregion

    #region price realized
    //PriceRealized
    [HttpGet, Compress]
    public ActionResult PriceRealized(long id)
    {
      ViewData["Event"] = EventRepository.GetEventByID(id);
      if (ViewData["Event"] == null) return RedirectToAction("PastAuctionResults");
      return View();
    }
    //pPriceRealized
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pPriceRealized(long event_id)
    {
      return View("pPriceRealized", InvoiceRepository.GetPriceRealizedForEvent(event_id));
    }
    #endregion

    #region past auction updates
    //PastAuctionUpdates
    [HttpGet, Compress]
    public ActionResult PastAuctionUpdates(long id)
    {
      ViewData["Event"] = EventRepository.GetEventByID(id);
      if (ViewData["Event"] == null) return RedirectToAction("PastAuctionResults");
      return View();
    }
    //pPastAuctionUpdates
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pPastAuctionUpdates(long event_id)
    {
      return View("pAuctionUpdates", AuctionRepository.GetAuctionUpdates(event_id));
    }
    #endregion

    #region my bids
    [HttpGet, VauctionAuthorize(Order = 1), Compress(Order = 2)]
    public ActionResult MyBids(long id)
    {
      ViewData["Event"] = EventRepository.GetEventByID(id);
      if (ViewData["Event"] == null) return RedirectToAction("PastAuctionResults");
      return View();
    }
    //pMyBids
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pMyBids(long event_id, long user_id)
    {
      return View("pMyBids", BidRepository.GetPastUsersWatchList(event_id, user_id));
    }
    #endregion

    #endregion

    #region Auction updates
    // AuctionUpdates
    [HttpGet, Compress]
    public ActionResult AuctionUpdates()
    {
      InitCurrentEvent();
      ViewData["Event"] = ViewData["CurrentEvent"];
      return View();
    }
    //pAuctionUpdates
    [ChildActionOnly]
    public ActionResult pAuctionUpdates(long event_id)
    {
      return View("pAuctionUpdates", AuctionRepository.GetAuctionUpdates(event_id));
    }
    #endregion

    #region Auction detail
    //AuctionDetail
    [HttpGet, Compress]
    public ActionResult AuctionDetail(long? id)
    {
      if (!id.HasValue) return RedirectToAction("Category");
      InitCurrentEvent();
      Event evnt = ViewData["CurrentEvent"] as Event;
      AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, evnt.ID, true);
      if (auction == null) return RedirectToAction("Category");
      SessionUser cuser = AppHelper.CurrentUser;

      if (auction.Status == Consts.AuctionStatus.Pending || auction.Status == Consts.AuctionStatus.Open)
      {
        AuctionUserInfo aui = new AuctionUserInfo();
        if (cuser != null)
        {
          aui.IsRegisterForEvent = EventRepository.IsUserRegisterForEvent(cuser.ID, auction.LinkParams.Event_ID);
          aui.IsInWatchList = AuctionRepository.IsUserWatchItem(cuser.ID, auction.LinkParams.ID);
        }
        else aui = new AuctionUserInfo { IsInWatchList = false, IsRegisterForEvent = false };
        ViewData["AuctionUserInfo"] = aui;

        AuctionShort ashort = AuctionRepository.GetAuctionDetailResult(auction.LinkParams.ID, true);
        ViewData["AuctionShort"] = ashort;
        ViewData["BiddingResult"] = BidRepository.CurrentAuctionBiddingResult(auction.LinkParams.ID, cuser == null ? null : (long?)cuser.ID, ashort.Price);
        if (ashort.Bids != 0)
        {
          if (auction.CloseStep == 1 && cuser != null)
            ViewData["IsUserHasRightsToBid"] = BidRepository.IsUserCanParticipateInBidding(auction.LinkParams.ID, cuser.ID);
        }
      }
      return View("AuctionDetailNew", auction);
    }



    //pAuctionDetailPast
    [ChildActionOnly]
    public ActionResult pAuctionDetailPast(long auction_id)
    {
      return View("pAuctionDetailPast", AuctionRepository.GetAuctionDetailResultPast(auction_id));
    }
    //pAuctionDetailImages
    [ChildActionOnly]
    public ActionResult pAuctionDetailImages(long auction_id)
    {
      return View("pAuctionDetailImages", DifferentRepository.GetAuctionImages(auction_id));
    }

    //UpdateAuctionResult
    [HttpPost, Compress]
    public JsonResult UpdateAuctionResult(long id)
    {
      AuctionShort ashort = AuctionRepository.GetAuctionDetailResult(id, true);
      BidCurrent bc = BidRepository.GetTopBidForItem(id);
      return JSON(new { currentbid = ashort.Bids > 0 ? ashort.CurrentBid.GetCurrency() : String.Empty, bids = ashort.Bids > 0 ? ashort.Bids.ToString() : String.Empty, minbid = ((bc == null ? ashort.Price : bc.Amount) + (bc == null ? 0 : Consts.GetIncrement(bc.Amount))).GetCurrency() });
    }

    #endregion

    #region Bidding
    //AuctionDetail
    [VauctionAuthorize, HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult AuctionDetail(long? id, bool? ProxyBidding, decimal? BidAmount, decimal? OutBidAmount)
    {
      if (!id.HasValue) return RedirectToAction("Category", "Auction");
      AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.GetValueOrDefault(-1), true);
      if (auction == null) return RedirectToAction("Category", "Auction");
      if (!auction.IsCurrentEvent || auction.Status == Consts.AuctionStatus.Closed || auction.DateEnd.CompareTo(DateTime.Now) < 0 || (!ProxyBidding.HasValue || !BidAmount.HasValue))
        return RedirectToAction("AuctionDetail", "Auction", new { @id = auction.LinkParams.ID });

      PreviewBid previewBid = new PreviewBid
                                {
                                  IsProxy = ProxyBidding.Value,
                                  Amount = BidAmount.Value,
                                  LinkParams = auction.LinkParams,
                                  Quantity = 1
                                };

      BidCurrent prevMaxBid = BidRepository.GetTopBidForItem(auction.LinkParams.ID);
      if (prevMaxBid != null)
      {
        previewBid.IsOutBid = previewBid.Amount <= prevMaxBid.Amount && previewBid.Amount <= prevMaxBid.MaxBid;
        if (prevMaxBid.User_ID == AppHelper.CurrentUser.ID)
        {
          previewBid.IsUpdate = true;
          previewBid.PreviousAmount = prevMaxBid.Amount;
          previewBid.PreviousMaxBid = prevMaxBid.MaxBid;
          previewBid.RealAmount = (previewBid.IsProxy) ? prevMaxBid.Amount : previewBid.Amount;
          previewBid.Amount = (previewBid.Amount < prevMaxBid.MaxBid) ? prevMaxBid.MaxBid : previewBid.Amount;
        }
        else
          previewBid.RealAmount = (!previewBid.IsProxy) ? previewBid.Amount : (((!OutBidAmount.HasValue) ? prevMaxBid.Amount : OutBidAmount.Value) + Consts.GetIncrement(prevMaxBid.Amount));
      }
      else
      {
        previewBid.RealAmount = (previewBid.IsProxy) ? auction.Price : previewBid.Amount;
      }
      return View("PreviewBid", previewBid);
    }

    [VauctionAuthorize, Compress, HttpPost, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)] // AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)
    public ActionResult PlaceBid(long? id, bool? ProxyBidding, decimal? BidAmount, decimal? RealBidAmount)
    {
      if (!id.HasValue) return RedirectToAction("Category", "Auction");
      AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.GetValueOrDefault(-1), true);
      if (auction == null) return RedirectToAction("Category", "Auction");
      if (!auction.IsCurrentEvent || auction.Status == Consts.AuctionStatus.Closed || auction.DateEnd.CompareTo(DateTime.Now) < 0 || (!ProxyBidding.HasValue || !BidAmount.HasValue))
        return RedirectToAction("AuctionDetail", "Auction", new { @id = auction.LinkParams.ID });

      SessionUser cuser = AppHelper.CurrentUser;

      if (!AuctionRepository.IsUserWatchItem(cuser.ID, auction.LinkParams.ID))
        AuctionRepository.AddItemToWatchList(cuser.ID, auction.LinkParams.ID);

      PreviewBid previewBid = new PreviewBid
                                {
                                  LinkParams = auction.LinkParams,
                                  IsProxy = ProxyBidding.Value,
                                  Amount = BidAmount.Value,
                                  Quantity = 1,
                                  RealAmount = RealBidAmount.GetValueOrDefault(0)
                                };

      byte result;
      BidCurrent currentBid = new BidCurrent
      {
        Amount = BidAmount.GetValueOrDefault(0),
        Auction_ID = id.Value,
        DateMade = DateTime.Now,
        IP = Consts.UsersIPAddress,
        IsActive = true,
        IsProxy = ProxyBidding.GetValueOrDefault(false),
        MaxBid = BidAmount.GetValueOrDefault(0),
        Quantity = 1,
        User_ID = cuser.ID
      };
      BidCurrent previousBid, loserBid, winnerBid;

      result = BidRepository.BiddingForSingleAuction(auction, currentBid, out previousBid, out loserBid, out winnerBid);

      if (result == 3) return RedirectToAction("AuctionDetail", new { id.Value });

      AuctionRepository.RemoveAuctionResultsCache(id.Value);

      BidRepository.RemoveTopBidForItemCache(id.Value);

      BidRepository.UpdateUsersTopBidCache(id.Value, cuser.ID, currentBid);

      if (result == 1)
      {
        previewBid.Amount = winnerBid.Amount;
        if (cuser.IsRecievingOutBidNotice && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
          Mail.SendOutBidLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, winnerBid.Amount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
        return View("OutBid", previewBid);
      }

      if (cuser.IsRecievingBidConfirmation && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
      {
        if (result == 2)
          Mail.SendSuccessfulBidUpdateLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, currentBid.Amount.GetCurrency(), currentBid.MaxBid.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl, currentBid.MaxBid > previousBid.MaxBid);
        else
          Mail.SendSuccessfulBidLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, currentBid.Amount.GetCurrency(), BidAmount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
      }

      if (loserBid != null && loserBid.User_ID != cuser.ID)
      {
        User usr = dataProvider.UserRepository.GetUser(loserBid.User_ID, true);
        if (usr.IsRecievingOutBidNotice && !String.IsNullOrEmpty(usr.Email) && !usr.IsHouseBidder)
        {
          AddressCard ac = dataProvider.UserRepository.GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
          Mail.SendOutBidLetter(ac.FirstName, ac.LastName, usr.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, winnerBid.Amount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
        }
      }
      /*

      BidCurrent prevBid = BidRepository.GetUserTopBidForItem(id.Value, cuser.ID, false);
      if (prevBid != null && (prevBid.IsProxy == ProxyBidding) && prevBid.MaxBid >= BidAmount) return RedirectToAction("AuctionDetail", "Auction", new { @id = id });
      
      BidCurrent lastTop = BidRepository.GetTopBidForItem(id.Value);
      decimal lastMaxBid = (lastTop == null) ? 0 : lastTop.MaxBid;
      decimal lastamount = (lastTop == null) ? 0 : lastTop.Amount;

      BiddingObject placedBid = BidRepository.PlaceSingleBid(id.Value, ProxyBidding.Value, BidAmount.Value, cuser.ID, 1, (lastTop != null && lastTop.User_ID == cuser.ID && ProxyBidding.Value), auction.Price, prevBid, lastTop);      

      List<BidLogCurrent> newblogs = new List<BidLogCurrent>();
      BidRepository.ResolveProxyBiddingSituation(id.Value, cuser.ID, ProxyBidding.Value, placedBid, lastTop, auction.Price, newblogs);

      BidCurrent currentTop = BidRepository.GetTopBidForItem(id.Value);      
      BidRepository.UpdateUsersTopBid(id.Value, AppSession.CurrentUser.ID, placedBid.Bid);
      bool IsOutBidden = (lastTop != null && currentTop.MaxBid <= lastTop.MaxBid && currentTop.User_ID != cuser.ID);
      if (IsOutBidden)
      {
        if (placedBid.Bid.Amount >= currentTop.Amount)
        {
          currentTop.Amount = placedBid.Bid.Amount;
          BidRepository.UpdateCurrentBid(currentTop);
        }        
        if (lastamount < currentTop.Amount && newblogs.Where(BL=>BL.User_ID ==currentTop.User_ID && BL.Amount ==currentTop.Amount && BL.MaxBid==currentTop.MaxBid && BL.IsProxy == currentTop.IsProxy).Count()==0)
          BidRepository.AddBidLogCurrent(id.Value, currentTop.Quantity, currentTop.User_ID, currentTop.IsProxy, currentTop.Amount, currentTop.MaxBid, false, currentTop.IP);
        previewBid.Amount = currentTop.Amount;
        try
        {
          if (cuser.IsRecievingOutBidNotice && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
            Mail.SendOutBidLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, currentTop.Amount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
        }
        catch (Exception ex)
        {
          Utils.Lib.Logger.LogException(ex);
        }
        AuctionRepository.UpdateAuctionBiddingResult(id.Value, currentTop.User_ID, currentTop.Amount, currentTop.MaxBid);
        return View("OutBid", previewBid);
      }
      AuctionRepository.UpdateAuctionBiddingResult(id.Value, currentTop.User_ID, currentTop.Amount, currentTop.MaxBid);
      if (lastTop != null && lastTop.User_ID!=cuser.ID)
      {
        User usr = dataProvider.UserRepository.GetUser(lastTop.User_ID, true);
        AddressCard ac = dataProvider.UserRepository.GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
        if (usr.IsRecievingOutBidNotice && !String.IsNullOrEmpty(usr.Email) && !usr.IsHouseBidder)
        {
          try
          {
            Mail.SendOutBidLetter(ac.FirstName, ac.LastName, usr.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, placedBid.Bid.Amount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
          }
          catch (Exception ex)
          {
            Utils.Lib.Logger.LogException(ex);
          }
        }
      }

      try
      {
        if (cuser.IsRecievingBidConfirmation && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
        {
          if (lastTop == null || lastTop.User_ID != cuser.ID)
          {
            Mail.SendSuccessfulBidLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, placedBid.Bid.Amount.GetCurrency(), BidAmount.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl);
          }
          else
          {
            Mail.SendSuccessfulBidUpdateLetter(cuser.FirstName, cuser.LastName, cuser.Email, auction.LinkParams.Lot.ToString(), auction.LinkParams.Title, currentTop.Amount.GetCurrency(), currentTop.MaxBid.GetCurrency(), auction.EventDateEnd > DateTime.Now ? auction.EventDateEnd.ToString() : DateTime.Now.Date.ToShortDateString(), auction.LinkParams.AuctionDetailUrl, (lastMaxBid < currentTop.MaxBid));
          }
        }
      }
      catch (Exception ex)
      {
        Utils.Lib.Logger.LogException(ex);
      }*/
      return View("SuccessfulBid", auction);
    }
    #endregion

    [NonAction, VauctionAuthorize, Compress, AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
    public ActionResult PlaceBidTest(long? id)
    {
      if (!id.HasValue) // return RedirectToAction("Category", "Auction");
        id = 65414;
      AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, true);
      if (auction == null) return RedirectToAction("Category", "Auction");
      BiddingResult br = BidRepository.CurrentAuctionBiddingResult(id.Value, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, auction.Price);
      decimal bid = br.MinBid == 0 ? auction.Price : (br.UsersTopBid == null || br.UsersTopBid.MaxBid < br.MinBid) ? br.MinBid : br.UsersTopBid.MaxBid;
      bid = bid + Consts.GetIncrement(bid);
      return PlaceBid(auction.LinkParams.ID, false, bid, bid);
    }
  }
}