using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Relatives.Models.CustomBinders;
using Vauction.Models;
using Vauction.Models.CustomModels;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Helpers;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class HomeController : BaseController
  {
    #region init
    private IUserRepository UserRepository;
    private IEventRepository EventRepository;
    private IAuctionRepository AuctionRepository;
    private ICategoryRepository CategoryRepository;

    public HomeController()
    {
      UserRepository = dataProvider.UserRepository;
      EventRepository = dataProvider.EventRepository;
      AuctionRepository = dataProvider.AuctionRepository;
      CategoryRepository = dataProvider.CategoryRepository;
    }
    #endregion

    #region html pages
    //Index
    [HttpGet, Compress]
    public ActionResult Index(string trng, long? u)
    {
      if (!String.IsNullOrEmpty(trng) && trng.ToLower() == "spa" && u.HasValue)
      {
        dataProvider.DifferentRepository.TrackSPAbanner(Consts.UsersIPAddress, u.GetValueOrDefault(-1));
        return RedirectToAction("Index", "Home");
      }
      ViewData["BuzzArray"] = dataProvider.DifferentRepository.GetHomePageBuzzList(true);
      ViewData["BigImages"] = dataProvider.DifferentRepository.GetHomePageImages(Consts.HomepageImageType.Big);
      ViewData["StripeImages"] = dataProvider.DifferentRepository.GetHomePageImages(Consts.HomepageImageType.Stripe);
      return View();
    }

    //Buzz
    [HttpGet, Compress]
    public ActionResult Buzz()
    {
      return View(dataProvider.DifferentRepository.GetHomePageBuzzList(false));
    }

    //NationalConvention
    [HttpGet, Compress]
    public ActionResult NationalConvention()
    {
      return View();
    }

    //Top25BostonSportsArtifacts
    [HttpGet, Compress]
    public ActionResult Top25BostonSportsArtifacts()
    {
        return View();
    }
    
    //About
    [HttpGet, Compress]
    public ActionResult About()
    {
      return View();
    }

    //FAQs
    [HttpGet, Compress]
    public ActionResult FAQs()
    {
      InitCurrentEvent();
      return View();
    }

    //Consign
    [HttpGet, Compress]
    public ActionResult Consign()
    {
      return View(new ConsignForm());
    }

    //ContactUsMessage
    [HttpPost]
    //public ActionResult ContactUsMessage(string FirstName, string LastName, string Email, string Phone, string BestTime, string Description)
    public ActionResult ContactUsMessage(ConsignForm consignForm)
    {
      if (consignForm == null) return RedirectToAction("Consign");
      consignForm.Validate(ModelState);
      if (!ModelState.IsValid)
      {
        return View("Consign", consignForm);
      }
      Mail.SendMessageFromConsignor(consignForm.FirstName, consignForm.LastName, consignForm.Email, consignForm.Phone ?? String.Empty, consignForm.BestTime ?? String.Empty, consignForm.Description);
      return RedirectToAction("Consign");
    }

    //ContactUs
    [HttpGet, Compress]
    public ActionResult ContactUs()
    {
      return View();
    }

    //RelatedLinks
    [HttpGet, Compress]
    public ActionResult RelatedLinks()
    {
      return View();
    }

    //UpdateBrowser
    [HttpGet, Compress]
    public ActionResult UpdateBrowser()
    {
      return View();
    }

    //UpcomingEvents
    [HttpGet, Compress]
    public ActionResult UpcomingEvents()
    {
      return View();
    }

    //RaceBidAuction
    [HttpGet, Compress]
    public ActionResult RaceBidAuction()
    {
      return View();
    }

    //TrckHealth
    public ActionResult TrckHealth()
    {
      return View();
    }
    #endregion

    #region free alert registration
    //FreeEmailAlertsRegister
    [HttpGet, Compress]
    public ActionResult FreeEmailAlertsRegister()
    {      
      return View(new OuterSubscription());
    }

    //FreeEmailAlertsRegistrationSuccess
    [HttpPost, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult FreeEmailAlertsRegistrationSuccess()
    {
      OuterSubscription os = new OuterSubscription();
      if (!TryUpdateModel(os, new[] { "Country", "Email", "EmailConfirm", "FirstName", "LastName", "State", "IsRecievingWeeklySpecials", "IsRecievingUpdates" }))
        return View("FreeEmailAlertsRegister", os);
      os.Validate(ModelState);
      if (ModelState.IsValid)
      {
        UserRepository.AddOuterSubscription(os);        
        Mail.SendFreeEmailRegisterConfirmation(os.Email, os.FirstName, os.LastName, AppHelper.GetSiteUrl(Url.Action("FreeEmailAlertsRegisterConfirm", "Home", new { id = os.ID })));
        return View();
      }
      return View("FreeEmailAlertsRegister", os);
    }

    //FreeEmailAlertsRegisterConfirm
    [HttpGet, Compress]
    public ActionResult FreeEmailAlertsRegisterConfirm(long? id)
    {
      return View( (!id.HasValue || !UserRepository.ActivateOuterSubscription(id.Value)) ? "FreeEmailAlertsRegistrationFailed" :"FreeEmailAlertsRegisterConfirm");
    }

    [HttpGet, Compress]
    public ActionResult FreeEmailAlertsUnregister()
    {
      return View();
    }

    //FreeEmailAlertsUnsubscribeSuccess
    [HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)] //AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)
    public ActionResult FreeEmailAlertsUnsubscribeSuccess(string Email)
    {      
      bool res1, res2;
      res1 = res2 = false;
      if (string.IsNullOrEmpty(Email)) return RedirectToAction("FreeEmailAlertsUnregister");
      res1 = UserRepository.UnsubscribeFromEmail(Email);
      res2 = UserRepository.UnsubscribeRegisterUser(Email);
      if (!res1 && !res2) return RedirectToAction("FreeEmailAlertsUnregister");
      ViewData["Email"] = Email;      
      return View("EmailAlertsUnsubscribeSuccess");
    }

    //EmailAlertsUnsubscribeSuccess
    [HttpGet, Compress]
    public ActionResult EmailAlertsUnsubscribeSuccess(long? id, char? t)
    {
      SessionUser cuser = AppHelper.CurrentUser;
      if (!id.HasValue)
      {
        return RedirectToAction("Index", "Home");
      }
      if (t.HasValue)
      {
        if (t.Value == 'U')
        {
          User usr = UserRepository.GetUser(id.Value, true);
          if (usr != null)
          {
            UserRepository.UnsubscribeFromEmail(usr.Email);
            UserRepository.UnsubscribeRegisterUser(id.Value);
            if (cuser != null && cuser.ID == usr.ID)
            {
              cuser.IsRecieveWeeklySpecials = false;
              cuser.IsRecieveNewsUpdates = false;              
            }
            ViewData["Email"] = usr.Email;
          }
        }
        else if (t.Value == 'M')
        {
          OuterSubscription os = dataProvider.DifferentRepository.GetOuterSubscription(id.Value);
          if (os != null)
          {
            UserRepository.UnsubscribeRegisterUser(os.Email);
            UserRepository.UnsubscribeFromOuterSubscribtionByID(id.Value);
            ViewData["Email"] = os.Email;
          }
        }
      }
      else
      {
        IUser usr = UserRepository.GetUser(id.Value, true);
        if (usr != null)
        {
          UserRepository.UnsubscribeFromEmail(usr.Email);
          UserRepository.UnsubscribeRegisterUser(id.Value);
          if (cuser != null && cuser.ID == usr.ID)
          {
            cuser.IsRecieveWeeklySpecials = false;
            cuser.IsRecieveNewsUpdates = false;            
          }
          ViewData["Email"] = usr.Email;
        }
      }
      if (ViewData["Email"] == null) return RedirectToAction("Index", "Home");
      InitCurrentEvent();
      return View();
    }
    #endregion

    #region for sale
    //Unsold    
    [HttpGet, Compress]
    public ActionResult Unsold()
    {
      //ViewData["Items"] = AuctionRepository.GetForSaleUnsoldItems(911);
      return View("ForSale");
      //return RedirectToAction("Index", "Home");
    }

    //ForSale    
    [HttpGet, Compress]
    public ActionResult ForSale()
    {
      //ViewData["Items"] = AuctionRepository.GetForSaleUnsoldItems(911);
      return View();
      //return RedirectToAction("Index", "Home");
    }
    #endregion

    #region search

    //SearchResult
    [HttpGet, Compress]
    public ActionResult SearchResult([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
    {      
      if (String.IsNullOrEmpty(param.Description))
      {
        param.Type = "l";
        param.Lot = "1";
      }
      else
      {
        if (param.Type == "l") param.Lot = param.Description; else param.Title = param.Description;
        param.Description = null;
      }
      InitCurrentEvent();
      Event evnt = ViewData["CurrentEvent"] as Event;      
      param.Event_ID = param.Event_ID ?? evnt.ID;      
      SetFilterParams(param);
      ViewData["ShowAdvancedForm"] = param.SeachType.HasValue && param.SeachType!=0;
      return View();
    }
    
    //AdvancedSearch
    [HttpGet, Compress]
    public ActionResult AdvancedSearch()
    {
      InitCurrentEvent();
      return View();
    }
    
    //AdvancedSearchResult
    [HttpGet, Compress]
    public ActionResult AdvancedSearchResult([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
    {
      if (param.SeachType == 1 && !param.Event_ID.HasValue && String.IsNullOrEmpty(param.Description))
        return RedirectToAction("AdvancedSearch");
      switch (param.Type)
      {
        case "t": param.Title = param.Description; param.Description = null; break;
        case "l": param.Lot = param.Description; param.Description = null; break;
        case "td": param.Title = param.Description; break;
        default: break;
      }
      ViewData["ShowAdvancedForm"] = true;
      SetFilterParams(param);
      InitCurrentEvent();
      Event ent = ViewData["CurrentEvent"] as Event;
      if (param.SeachType == 2) param.Event_ID = ent.ID;
      return View("SearchResult");
    }

    //pAdvancedSearch
    [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_01day)
    public ActionResult pAdvancedSearch(long eventid, AuctionFilterParams param)
    {
      List<IEvent> evnts = new List<IEvent>(EventRepository.GetPastedEventsList());
      if (evnts.Count > 0 && evnts[0].ID != eventid) evnts.Insert(0, EventRepository.GetEventByID(eventid));
      
      List<SelectListItem> sl = new List<SelectListItem>();
      sl.Add(new SelectListItem() { Selected = !param.Event_ID.HasValue || -1 == param.Event_ID.Value, Value = "-1", Text = "Search All Auctions" });
      foreach (Event evnt in evnts)
        sl.Add(new SelectListItem() { Selected = param.Event_ID.HasValue && evnt.ID == param.Event_ID.Value, Text = evnt.Title, Value = evnt.ID.ToString() });
      ViewData["AllEvents"] = sl;
      
      List<SelectListItem> cl = new List<SelectListItem>();
      cl.Add(new SelectListItem() { Selected = !param.SelectedCategory.HasValue || -1 == param.SelectedCategory.Value, Value = "-1", Text = "All Categories" });
      foreach (IdTitle item in CategoryRepository.GetListForCategory(eventid))
        cl.Add(new SelectListItem() { Selected = param.SelectedCategory.HasValue && param.SelectedCategory.Value==item.ID, Text = item.Title, Value = item.ID.ToString() });
      ViewData["CategoriesList"] = cl;
      return View("pAdvancedSearch", param);
    }

    //pSearch
    private object pSearch(AuctionFilterParams param)
    {
      SetFilterParams(param);
      ViewData["PageActionPath"] = "SearchResult";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      return AuctionRepository.GetByCriterias(param);
    }        
    //pSearchResultCurrent
    [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_01min)
    public ActionResult pSearchResultCurrent(bool iscurrent, AuctionFilterParams param, int page, int viewmode, int imageviewmode)
    {      
      ViewData["IsPastGrid"] = false;      
      return View("pAuctionGrid", pSearch(param));
    }
    //pSearchResultPast
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pSearchResultPast(bool iscurrent, AuctionFilterParams param, int page, int viewmode, int imageviewmode)
    {
      ViewData["IsPastGrid"] = true;
      return View("pAuctionGrid", pSearch(param));
    }
    
    #endregion

    #region old/temp methods
    //[NonAction]
    //public ActionResult NSC()
    //{
    //  return View();
    //}

    //HonusWagner
    [HttpGet]
    public ActionResult HonusWagner()
    {
      return View();
    }

    //BringingRecordSettingBabeRuthJersey
    [HttpGet]
    public ActionResult BringingRecordSettingBabeRuthJersey()
    {
      return View();
    }

    [HttpGet]
    public ActionResult Developer()
    {
      return View();
    }
    #endregion
  }
}