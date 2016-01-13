using System;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Collections.Generic;
using Relatives.Models.CustomBinders;
using System.Web.Routing;
using Vauction.Utils.Graphics;
using Vauction.Utils.Helpers;
using Vauction.Utils.Paging;
using Vauction.Models.CustomModels;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Reports;
using Vauction.Utils.Validation;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class AccountController : BaseController
  {
    #region initialization
    public IFormsAuthenticationService FormsService { get; set; }
    protected override void Initialize(RequestContext requestContext)
    {
      if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
      base.Initialize(requestContext);
    }

    private IUserRepository UserRepository;
    private IEventRepository EventRepository;
    private IAuctionRepository AuctionRepository;
    private IInvoiceRepository InvoiceRepository;
    private IBidRepository BidRepository;
    public AccountController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      UserRepository = dataProvider.UserRepository;
      EventRepository = dataProvider.EventRepository;
      InvoiceRepository = dataProvider.InvoiceRepository;
      BidRepository = dataProvider.BidRepository;
    }
    #endregion

    [HttpGet, Compress] //, VCache(CachingExpirationTime.Hours_01)
    public ActionResult Index()
    {
      return RedirectToAction("Index", "Home");
    }

    #region log in & out
    //LogOn - get
    [HttpGet, Compress] //, VCache(CachingExpirationTime.Hours_01)
    public ActionResult LogOn()
    {
      return View();
    }

    //LogOn - post
    [HttpPost, Compress]
    public ActionResult LogOn(string login, string password, bool? rememberMe, string returnUrl)
    {
      if (ValidationCheck.IsEmpty(login))
      {
        ModelState.AddModelError("login", ErrorMessages.GetRequired("UserID"));
        return View();
      }
      if (String.IsNullOrEmpty(password))
      {
        ModelState.AddModelError("password", ErrorMessages.GetRequired("Password"));
        return View();
      }
      User user = UserRepository.ValidateUser(login, password);
      if (user == null)
      {
        ModelState.AddModelError("login", "Incorrect UserID or password.");
        ViewData["rememberMe"] = rememberMe.HasValue && rememberMe.Value;
        ViewData["wrongPass"] = true;
        return View();
      }

      UserRepository.TryToUpdateNormalAttempts(user);
      FormsService.SignIn(login, rememberMe.HasValue && rememberMe.Value, user);

      if (!user.IsModifyed)
      {
        Session.Remove("redirectUrl");
        Session.Add("redirectUrl", returnUrl);
        return RedirectToAction("Profile", "Account");
      }

      //if (!String.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
      if (HttpContext.Request.IsUrlLocalToHost(returnUrl)) return Redirect(returnUrl);
      return RedirectToAction("Index", "Home");
    }

    //LogOff
    [HttpGet, Compress]
    public ActionResult LogOff()
    {
      SessionUser cuser = AppHelper.CurrentUser;
      if (cuser != null) UserRepository.RemoveUserFromCache(cuser.ID, cuser.Email);
      FormsService.SignOut();
      return RedirectToAction("Index", "Home");
    }
    #endregion

    [VauctionAuthorize, Compress]
    public ActionResult MyAccount()
    {
      return View();
    }

    #region profile & registration
    // LoadLinkedUserData
    public void LoadLinkedUserData(long? country, string zip)
    {
      if (country.HasValue)
        ViewData["Countries"] = new SelectList(dataProvider.DifferentRepository.GetCountries(), "ID", "Title", country);
      else
        ViewData["Countries"] = new SelectList(dataProvider.DifferentRepository.GetCountries(), "ID", "Title");
      if (!string.IsNullOrEmpty(zip))
        ViewData["States"] = new SelectList(dataProvider.DifferentRepository.GetStates(null), "Code", "Code", zip);
      else
        ViewData["States"] = new SelectList(dataProvider.DifferentRepository.GetStates(null), "Code", "Code");
    }

    //Profile
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult Profile()
    {
      RegisterInfo user = UserRepository.GetRegisterInfo(AppHelper.CurrentUser.ID);
      LoadLinkedUserData(user.BillingCountry, user.BillingState);
      return View(user);
    }
    //Profile (confrim)
    [VauctionAuthorize, HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult Profile(string ConfirmCode)
    {
      SessionUser cuser = AppHelper.CurrentUser;
      RegisterInfo user = UserRepository.GetRegisterInfo(cuser.ID);
      UpdateModel(user, new[] { "Login", "Email", "ConfirmEmail", "Password", "ConfirmPassword", "RecieveWeeklySpecials", "RecieveNewsUpdates", "Fax", "Reference", "BillingLikeShipping", "BillingFirstName", "BillingMIName", "BillingLastName", "BillingAddress1", "BillingAddress2", "BillingCity", "BillingState", "BillingZip", "BillingPhone", "BillingCountry", "ShippingFirstName", "ShippingMIName", "ShippingLastName", "ShippingAddress1", "ShippingAddress2", "ShippingCity", "ShippingState", "ShippingZip", "ShippingPhone", "ShippingWorkPhone", "ShippingCountry", "EveningPhone", "DayPhone", "MobilePhone", "TaxpayerID", "BillingCompany", "BillingInternationalState", "ShippingInternationalState", "Reference1AuctionHouse", "Reference1LastBidPlaced", "Reference1PhoneNumber", "Reference2AuctionHouse", "Reference2LastBidPlaced", "Reference2PhoneNumber", "EbayID", "EbayFeedback" });

      user.Validate(ModelState);

      if (ModelState.IsValid)
      {
        IUser usr = UserRepository.UpdateUser(user);
        if (usr != null && usr.ID == cuser.ID) AppHelper.CurrentUser = SessionUser.Create(usr);

        if (Session["redirectUrl"] != null) return Redirect(Session["redirectUrl"] as string);
        return RedirectToAction("ProfileSaveMessage", "Account");
      }
      ViewData.Model = user;
      LoadLinkedUserData(user.BillingCountry, user.BillingState);
      return View();
    }

    //ProfileSaveMessage
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult ProfileSaveMessage()
    {
      return View();
    }

    //Register
    [HttpGet, Compress]
    public ActionResult Register()
    {
      LoadLinkedUserData(null, null);
      return View(new RegisterInfo());
    }
    //Register (confirm)
    [HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult Register(string ConfirmCode)
    {
      RegisterInfo user = new RegisterInfo();
      string[] updateFields = new[] { "Login", "Email", "ConfirmEmail",  "Password", "ConfirmPassword","RecieveWeeklySpecials", "RecieveNewsUpdates",  
                "Fax", "Reference", "BillingLikeShipping","BillingFirstName", "BillingMIName", "BillingLastName", "BillingAddress1", "BillingAddress2", "BillingCity", "BillingState", "BillingZip", "BillingPhone", "BillingCountry", "ShippingFirstName", "ShippingMIName", "ShippingLastName", "ShippingAddress1", "ShippingAddress2", "ShippingCity", "ShippingState", "ShippingZip", "ShippingPhone", "ShippingWorkPhone", "ShippingCountry", "EveningPhone", "DayPhone", "BidderID", "MobilePhone", "TaxpayerID", "BillingCompany", "BillingInternationalState","ShippingInternationalState", "Reference1AuctionHouse", "Reference1LastBidPlaced", "Reference1PhoneNumber", "Reference2AuctionHouse", "Reference2LastBidPlaced", "Reference2PhoneNumber", "EbayID", "EbayFeedback", "RecievingOutBidNotice", "NoReferencesAvailable"};

      if (TryUpdateModel(user, updateFields))
      {
        user.Validate(ModelState);
        if (ModelState.IsValid)
        {
          User newUser = UserRepository.AddUser(user);
          if (newUser != null)
          {
            string confirmationUrl = AppHelper.GetSiteUrl(Url.Action("RegisterFinish", "Account", new { id = newUser.ConfirmationCode }));
            Mail.SendRegisterConfirmation(user.Email, user.Login, confirmationUrl, user.BillingFirstName, user.BillingLastName);
            return RedirectToAction("RegisterConfirm", "Account");
          }
        }
      }

      ViewData.Model = user;
      LoadLinkedUserData(user.BillingCountry, user.BillingState);
      return View();
    }

    //RegisterConfirmFail
    [HttpGet, Compress]
    public ActionResult RegisterConfirmFail()
    {
      return View();
    }

    //RegisterConfirmSuccess
    [HttpGet, Compress]
    public ActionResult RegisterConfirmSuccess()
    {
      return View();
    }

    //RegisterFinish
    [HttpGet, Compress]
    public ActionResult RegisterFinish(string id)
    {
      if (String.IsNullOrEmpty(id)) return RedirectToAction("RegisterConfirmFail", "Account");
      long userId = UserRepository.ConfirmUser(id);
      return (userId > 0) ? RedirectToAction("RegisterConfirmSuccess", "Account") : RedirectToAction("RegisterConfirmFail", "Account");
    }

    //RegisterConfirm
    [HttpGet, Compress]
    public ActionResult RegisterConfirm()
    {
      return View();
    }

    #endregion

    #region email settings
    //EditMailSettings (get)
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult EditMailSettings()
    {
      SessionUser cuser = AppHelper.CurrentUser;
      ViewData["IsRecievingBidConfirmation"] = cuser.IsRecievingBidConfirmation;
      ViewData["IsRecievingLotClosedNotice"] = cuser.IsRecievingLotClosedNotice;
      ViewData["IsRecievingLotSoldNotice"] = cuser.IsRecievingLotSoldNotice;
      ViewData["IsRecievingOutBidNotice"] = cuser.IsRecievingOutBidNotice;
      ViewData["IsRecievingWeeklySpecials"] = cuser.IsRecieveWeeklySpecials;
      ViewData["IsRecievingNewsUpdates"] = cuser.IsRecieveNewsUpdates;
      return View();
    }
    //EditMailSettings (post)
    [VauctionAuthorize, HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult EditMailSettings(string IsRecievingWeeklySpecials, string IsRecievingNewsUpdates, string IsRecievingBidConfirmation, string IsRecievingOutBidNotice, string IsRecievingLotSoldNotice, string IsRecievingLotClosedNotice)
    {
      bool _IsRecievingWeeklySpecials = Convert.ToBoolean(IsRecievingWeeklySpecials);
      bool _IsRecievingNewsUpdates = Convert.ToBoolean(IsRecievingNewsUpdates);
      bool _IsRecievingBidConfirmation = Convert.ToBoolean(IsRecievingBidConfirmation);
      bool _IsRecievingOutBidNotice = Convert.ToBoolean(IsRecievingOutBidNotice);
      bool _IsRecievingLotSoldNotice = Convert.ToBoolean(IsRecievingLotSoldNotice);
      bool _IsRecievingLotClosedNotice = Convert.ToBoolean(IsRecievingLotClosedNotice);

      SessionUser cuser = AppHelper.CurrentUser;
      if (UserRepository.UpdateEmailSettings(cuser.ID, _IsRecievingWeeklySpecials, _IsRecievingNewsUpdates, _IsRecievingBidConfirmation, _IsRecievingOutBidNotice, _IsRecievingLotSoldNotice, _IsRecievingLotClosedNotice))
      {
        ViewData["IsRecievingBidConfirmation"] = _IsRecievingBidConfirmation;
        ViewData["IsRecievingLotClosedNotice"] = _IsRecievingLotClosedNotice;
        ViewData["IsRecievingLotSoldNotice"] = _IsRecievingLotSoldNotice;
        ViewData["IsRecievingOutBidNotice"] = _IsRecievingOutBidNotice;
        ViewData["IsRecievingWeeklySpecials"] = _IsRecievingWeeklySpecials;
        ViewData["IsRecievingNewsUpdates"] = _IsRecievingNewsUpdates;
        ViewData["DataSaved"] = true;

        cuser.IsRecievingBidConfirmation = _IsRecievingBidConfirmation;
        cuser.IsRecievingLotClosedNotice = _IsRecievingLotClosedNotice;
        cuser.IsRecievingLotSoldNotice = _IsRecievingLotSoldNotice;
        cuser.IsRecievingOutBidNotice = _IsRecievingOutBidNotice;
        cuser.IsRecieveWeeklySpecials = _IsRecievingWeeklySpecials;
        cuser.IsRecieveNewsUpdates = _IsRecievingNewsUpdates;
      }
      return View();
    }
    #endregion

    #region forgot password
    //ForgotPassword (get)
    [HttpGet, Compress] //, VCache(CachingExpirationTime.Hours_01)
    public ActionResult ForgotPassword()
    {
      return View(new ForgotPassword());
    }
    //ForgotPassword (post)
    [HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult ForgotPassword(string email)
    {
      ForgotPassword data = new ForgotPassword();

      UpdateModel(data, new[] { "Email" });

      data.Validate(ModelState);

      if (ModelState.IsValid)
      {
        IUser user = UserRepository.SetNewUserPassword(data.Email);
        if (user != null)
        {
          UserRepository.RemoveUserFromCache(user.ID, user.Email);
          string confirmationUrl = AppHelper.GetSiteUrl(Url.Action("SetNewPassword", "Account", new { id = user.ConfirmationCode }));
          Mail.ForgotPassword(user.Email, user.Login, user.Password, confirmationUrl);
          return RedirectToAction("ForgotPasswordUpdate");
        }
      }

      ViewData.Model = data;
      return View();

    }
    //ForgotPasswordUpdate
    [HttpGet, Compress]
    public ActionResult ForgotPasswordUpdate()
    {
      return View();
    }
    //SetNewPassword (get)
    [HttpGet, Compress]
    public ActionResult SetNewPassword(string id)
    {
      if (String.IsNullOrEmpty(id)) return RedirectToAction("LogOn");
      IUser usr = UserRepository.GetUserByConfirmationCode(id);
      if (usr == null) return RedirectToAction("LogOn");
      RegisterInfo user = UserRepository.GetRegisterInfo(usr.ID);
      LoadLinkedUserData(user.BillingCountry, user.BillingState);
      user.Password = "";
      user.ConfirmPassword = "";
      return View(user);
    }
    // SetNewPassword (post)
    [HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult SetNewPassword(long? user_id, string password, string confirmpassword)
    {
      if (!user_id.HasValue) return RedirectToAction("LogOn");
      RegisterInfo user = UserRepository.GetRegisterInfo(user_id.Value);
      if (user == null) return RedirectToAction("LogOn");
      user.Password = password;
      user.ConfirmPassword = confirmpassword;
      UpdateModel(user, new[] { "Password", "ConfirmPassword" });
      user.Validate(ModelState);
      if (!ModelState.IsValid) return View(user);
      bool res = UserRepository.UpdatePassword(user_id.Value, password);
      if (!res) return RedirectToAction("LogOn");
      User usr = UserRepository.GetUser(user_id.Value, false);
      if (usr == null)
      {
        AppHelper.CurrentUser = null;
        return RedirectToAction("LogOn");
      }
      usr = UserRepository.ValidateUser(usr.Login, usr.Password);
      if (usr == null)
      {
        AppHelper.CurrentUser = null;
        return RedirectToAction("LogOn");
      }
      UserRepository.TryToUpdateNormalAttempts(usr);
      FormsService.SignIn(usr.Login, false, usr);
      return usr.IsModifyed ? RedirectToAction("Index", "Home") : RedirectToAction("Profile", "Account");
    }
    #endregion

    #region resent confirmation code
    //ResendConfirmationCode (get)
    [HttpGet, Compress] //, VCache(CachingExpirationTime.Hours_01)
    public ActionResult ResendConfirmationCode()
    {
      return View(new ResendConfirmationCode());
    }

    //ResendConfirmationCode (post)
    [HttpPost, Compress, ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public ActionResult ResendConfirmationCode(string Email)
    {
      ResendConfirmationCode data = new ResendConfirmationCode();
      UpdateModel(data, new[] { "Email" });
      data.Validate(ModelState);
      if (ModelState.IsValid)
      {
        IUser user = UserRepository.GenerateNewConfirmationCode(data.Email);
        if (user != null)
        {
          string confirmationUrl = AppHelper.GetSiteUrl(Url.Action("RegisterFinish", "Account", new { id = user.ConfirmationCode }));
          Mail.ResendConfirmationCode(user.Email, user.Login, confirmationUrl);
        }
        return RedirectToAction("ResendConfirmationCodeUpdate");
      }
      return View(data);
    }

    //ResendConfirmationCodeUpdate
    [HttpGet, Compress]
    public ActionResult ResendConfirmationCodeUpdate()
    {
      return View();
    }
    #endregion

    #region past auction bidding history
    //PastAuction
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult PastAuction()
    {
      ViewData["Event_ID"] = EventRepository.GetCurrent().ID;
      return View();
    }
    //pPastAuction
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Days_01)]
    public ActionResult pPastAuction(long event_id, long user_id)
    {
      return View("pPastAuction", BidRepository.GetPastEventBiddingHistory(user_id));
    }

    //AuctionsParticipated
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult AuctionsParticipated([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParamsEx param)
    {
      if (!param.ID.HasValue) return RedirectToAction("PastAuction");
      IEvent evnt = EventRepository.GetEventByID(param.ID.Value);
      if (evnt == null || (evnt.CloseStep != 2 && !evnt.IsClickable && !evnt.IsViewable)) return RedirectToAction("PastAuction");
      ViewData["Event"] = evnt;
      SetFilterParams(param);
      return View();
    }
    //pAuctionsParticipated
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pAuctionsParticipated(long event_id, long user_id, AuctionFilterParamsEx param, int page, int viewmode, int imageviewmode)
    {
      SetFilterParams(param);
      ViewData["PageActionPath"] = "AuctionsParticipated";
      return View("pAuctionsParticipated", BidRepository.GetPastUsersWatchList(event_id, user_id).ToPagedList(param.page, Config.GetIntProperty("PageSize")));
    }
    #endregion

    #region past auction invoices
    //AuctionInvoices
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult AuctionInvoices()
    {
      ViewData["Event_ID"] = EventRepository.GetCurrent().ID;
      return View();
    }
    //pAuctionInvoices
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Days_01)]
    public ActionResult pAuctionInvoices(long event_id, long user_id)
    {
      return View("pAuctionInvoices", InvoiceRepository.GetUserInvoicesForPage(user_id));
    }

    //InvoiceDetailed
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult InvoiceDetailed(long id)
    {
      UserInvoice ui = InvoiceRepository.GetUserInvoice(id);
      if (ui == null || ui.User_ID != AppHelper.CurrentUser.ID) return RedirectToAction("AuctionInvoices");
      ViewData["Invoice_ID"] = ui.Id;
      ViewData["Event_Title"] = EventRepository.GetEventByID(ui.Event_ID).Title;
      return View();
    }
    //pInvoiceDetailed
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pInvoiceDetailed(long userinvoice_id)
    {
      ViewData["AmountPaid"] = InvoiceRepository.GetUserInvoicePaidAmount(userinvoice_id);
      return View("pInvoiceDetailed", InvoiceRepository.GetInvoiceDetailsByUserInvoice(userinvoice_id));
    }
    #endregion

    #region consignor statments
    //ConisgnorStatements
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult ConisgnorStatements()
    {
      ViewData["Event_ID"] = EventRepository.GetCurrent().ID;
      return View(InvoiceRepository.GetConsignorStatementsForPage(AppHelper.CurrentUser.ID));
    }
    //ConsignmentDetail
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult ConsignmentDetail(long id)
    {
      Consignment consignment = InvoiceRepository.GetConsignment(id);
      if (consignment == null || consignment.User_ID != AppHelper.CurrentUser.ID) return RedirectToAction("ConisgnorStatements");
      ConsignmentContract consignmentContract = InvoiceRepository.GetConsignmentContract(consignment.ID);
      ViewData["ConsignmentContract"] = consignmentContract;
      ViewData["ConsignmentStatement"] = consignment;
      ViewData["Event_Title"] = EventRepository.GetEventByID(consignment.Event_ID).Title;
      ViewData["ConsignmentTotals"] = InvoiceRepository.GetConsignmentTotals(id);
      return View(InvoiceRepository.GetConsignmentDetailsByConsignmentID(id));
    }

    [VauctionAuthorize, HttpPost]
    public JsonResult SignConsignmentContract(long id, string sellerSignature)
    {
      try
      {
        Consignment consignment = InvoiceRepository.GetConsignment(id);
        ConsignmentContract consignmentContract = InvoiceRepository.GetConsignmentContract(id);
        if (consignment == null || consignmentContract == null || consignmentContract.StatusID != (int)Consts.ConsignmentContractStatus.Unsigned || consignment.User_ID != AppHelper.CurrentUser.ID)
        {
          throw new Exception("Error occurred during the process of signing the document. Please reload page and try again");
        }
        if (string.IsNullOrWhiteSpace(sellerSignature))
        {
          throw new Exception("Draw signature as first.");
        }
        Specialist specialist = InvoiceRepository.GetSpecialist(consignment.Specialist_ID.GetValueOrDefault(-1));
        if (specialist == null)
        {
          throw new Exception("Error occurred during the process of signing the document. Please contact us.");
        }
        string lelandsSignaturePath = AppHelper.SignatureImagesOnDisk(string.Format("userSignature{0}.png", specialist.User_ID));
        FileInfo fileInfo = new FileInfo(lelandsSignaturePath);
        if (!fileInfo.Exists)
        {
          throw new Exception("Error occurred during the process of signing the document. Please contact us.");
        }
        SignatureToImage signature = new SignatureToImage();
        Bitmap signatureImage = signature.SigJsonToImage(sellerSignature);
        string sellerSignaturePath = AppHelper.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName.ToLower().Replace(".pdf", ".png"));
        fileInfo = new FileInfo(sellerSignaturePath);
        if (fileInfo.Exists) fileInfo.Delete();
        signatureImage.Save(sellerSignaturePath);
        Address lelandsAddress = new Address
        {
          FirstName = "Lelands.com",
          LastName = Consts.SiteEmail,
          Address_1 = Consts.CompanyAddress,
          City = Consts.CompanyCity,
          State = Consts.CompanyState,
          Zip = Consts.CompanyZip,
          HomePhone = Consts.CompanyPhone,
          WorkPhone = Consts.CompanyFax
        };
        User consignor = UserRepository.GetUser(consignment.User_ID, true);
        Address consignorAddress = Address.GetAddress(UserRepository.GetAddressCard(consignor.Billing_AddressCard_ID.GetValueOrDefault(0), true));
        PdfReports.ConsignmentContract(AppHelper.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName), AppHelper.PublicImagesOnDisk("logo.png"), string.Empty, Consts.CompanyTitleName, string.Empty, lelandsAddress, lelandsSignaturePath, consignorAddress, consignor.Email, sellerSignaturePath, DateTime.Now, InvoiceRepository.GetConsignmentDetailsByConsignmentID(id), consignmentContract.ContractText);
        consignmentContract.StatusID = (int)Consts.ConsignmentContractStatus.Signed;
        InvoiceRepository.UpdateConsignmentContract(consignmentContract);
      }
      catch (Exception exc)
      {
        return JSON(new { success = false, exc.Message });
      }
      return JSON(new { success = true });
    }

    //ShowConsignmentContract
    public FileStreamResult ShowConsignmentContract(long id)
    {
      Consignment consignment = InvoiceRepository.GetConsignment(id);
      ConsignmentContract consignmentContract = InvoiceRepository.GetConsignmentContract(id);
      if (consignment == null || consignment.User_ID != AppHelper.CurrentUser.ID ||
consignmentContract == null || consignmentContract.StatusID == (int)Consts.ConsignmentContractStatus.NotGenerated || string.IsNullOrWhiteSpace(consignmentContract.FileName)) return null;
      string path = AppHelper.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName);
      FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
      return File(fs, "application/pdf");
    }

    //DownloadConsignmentContract
    [VauctionAuthorize]
    public FilePathResult DownloadConsignmentContract(long id)
    {
      try
      {
        Consignment consignment = InvoiceRepository.GetConsignment(id);
        ConsignmentContract consignmentContract = InvoiceRepository.GetConsignmentContract(id);
        if (consignment == null || consignment.User_ID != AppHelper.CurrentUser.ID ||
consignmentContract == null || consignmentContract.StatusID == (int)Consts.ConsignmentContractStatus.NotGenerated || string.IsNullOrWhiteSpace(consignmentContract.FileName)) return null;
        string path = AppHelper.ConsignmentContractOnDisk(consignment.ID, consignmentContract.FileName);
        return File(path, "application/pdf", consignmentContract.FileName);
      }
      catch
      {
        return null;
      }
    }
    #endregion

    #region seller tool
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult ConsignedItems([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
    {
      if (!AppHelper.CurrentUser.IsSellerType) return RedirectToAction("MyAccount");
      InitCurrentEvent();
      if (!param.Event_ID.HasValue) param.Event_ID = (ViewData["CurrentEvent"] as Event).ID;
      switch (param.Type)
      {
        case "t": param.Title = param.Description; param.Description = null; break;
        case "l": param.Lot = param.Description; param.Description = null; break;
        case "td": param.Title = param.Description; break;
        default: break;
      }
      SetFilterParams(param);
      return View();
    }

    //pConsignedItemsSearch
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Days_01)]
    public ActionResult pConsignedItemsSearch(long user_id, AuctionFilterParams param)
    {
      List<Event> evnts = EventRepository.GetConsingedEvents(AppHelper.CurrentUser.ID);
      List<SelectListItem> sl = new List<SelectListItem>();
      sl.Add(new SelectListItem() { Selected = !param.Event_ID.HasValue || -1 == param.Event_ID.Value, Value = "-1", Text = "Search All Auctions" });
      foreach (Event evnt in evnts)
        sl.Add(new SelectListItem() { Selected = param.Event_ID.HasValue && evnt.ID == param.Event_ID.Value, Text = evnt.Title, Value = evnt.ID.ToString() });
      ViewData["AllEvents"] = sl;
      return View("pConsignedItemsSearch", param);
    }
    //pConsignedItems
    private object pConsignedItems(AuctionFilterParams param, long user_id)
    {
      SetFilterParams(param);
      ViewData["PageActionPath"] = "ConsignedItems";
      ViewData["IsUserRegisteredForEvent"] = Consts.IsAllUsersCanSeeBids;
      ViewData["IsShownOpenBidOne"] = Consts.IsShownOpenBidOne;
      return AuctionRepository.GetAuctionListForSeller(param, user_id);
    }
    //pConsignedItemsCurrent
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Minutes_01)]
    public ActionResult pConsignedItemsCurrent(long user_id, AuctionFilterParams param, int page, int viewmode, int imageviewmode)
    {
      ViewData["IsPastGrid"] = false;
      return View("pAuctionGrid", pConsignedItems(param, user_id));
    }
    //pConsignedItemsPast
    [ChildActionOnly, ActionOutputCache(CachingExpirationTime.Hours_01)]
    public ActionResult pConsignedItemsPast(long user_id, AuctionFilterParams param, int page, int viewmode, int imageviewmode)
    {
      ViewData["IsPastGrid"] = true;
      return View("pAuctionGrid", pConsignedItems(param, user_id));
    }
    #endregion

    #region watch & remove bid
    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult WatchBid()
    {
      InitCurrentEvent();
      Event evnt = ViewData["CurrentEvent"] as Event ?? new Event();
      if (evnt.ID == 0 && !evnt.IsCurrent && evnt.CloseStep == 2)
        return RedirectToAction("AuctionsParticipated", new { id = evnt.ID });
      return View(BidRepository.GetBidWatchForUser(AppHelper.CurrentUser.ID, evnt.ID));
    }

    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult AddItemToWatchList(long? id)
    {
      if (id.HasValue)
        AuctionRepository.AddItemToWatchList(AppHelper.CurrentUser.ID, id.Value);
      return RedirectToAction("AuctionDetail", "Auction", new { @id = id });
    }

    [VauctionAuthorize]
    public ActionResult RemoveBid(long id)
    {
      return JSON(AuctionRepository.RemoveItemFromWatchList(AppHelper.CurrentUser.ID, id) ? 1 : 0);
    }
    #endregion
  }
}
