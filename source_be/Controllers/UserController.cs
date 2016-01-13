using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Linq;
using System.Linq.Dynamic;
using Vauction.Models;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [Compress]
  public class UserController : BaseController
  {
    #region init
    private IUserRepository UserRepository;
    private IGeneralRepository GeneralRepository;
    public UserController()
    {
      UserRepository = dataProvider.UserRepository;
      GeneralRepository = dataProvider.GeneralRepository;
    }
    #endregion

    #region user dialog methods
    //GetUsersListSearchBox
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
    public JsonResult GetUsersListSearchBox(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? User_ID, string Login, string FN, string Email, string Phone)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) : JSON(UserRepository.GetUsersListSearchBox(sidx, sord, page, rows, 0, User_ID, Login, FN, Email, Phone));
    }
    
    //GetSellersList
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
    public JsonResult GetSellersList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? User_ID, string Login, string FN, string Email, string Phone)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) : JSON(UserRepository.GetUsersListSearchBox(sidx, sord, page, rows, 1, User_ID, Login, FN, Email, Phone));
    }

    //GetBuyersList
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public JsonResult GetBuyersList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? User_ID, string Login, string FN, string Email, string Phone)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) : JSON(UserRepository.GetUsersListSearchBox(sidx, sord, page, rows, 2, User_ID, Login, FN, Email, Phone));
    }
    #endregion

    #region changing user data 
    //ChangeUserTypeToSellerBuyer
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
    public JsonResult ChangeUserTypeToSellerBuyer(long user_id)
    {
      return JSON(UserRepository.ChangeUserTypeToSellerBuyer(user_id));
    }
    //ActivateUserWithoutEmail
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
    public JsonResult ActivateUserWithoutEmail(long user_id)
    {
      return JSON(UserRepository.ActivatingUserWithoutEmail(user_id));
    }

    #endregion


    //NOT DONE





    //Users
    [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
    public ActionResult Users()
    {
      ViewData["UserTypes"] = GeneralRepository.GetUserTypesForSearchSelect();
      ViewData["UserStatuses"] = GeneralRepository.GetUserStatusesForSearchSelect();
      ViewData["CommissionRates"] = GeneralRepository.GetCommissionRatesForSearchSelect();
      return View();
    }

    //GetUsersList
    [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
    public JsonResult GetUsersList(string sidx, string sord, int page, int rows, bool _search, long? User_Id, string Login, byte? Status, byte? UserType, int? CommRate_ID, string Name, string Email, DateTime? DateIN, string DayPhone, string EveningPhone, bool? isfirstload)
    {
      return JSON((!isfirstload.HasValue || isfirstload.Value)?false:UserRepository.GetUsersList(sidx, sord, page, rows, _search, User_Id, Login, Status, UserType, CommRate_ID, Name, Email, DateIN, DayPhone, EveningPhone));
    }

    //GetBuyersAndHBList
    public JsonResult GetBuyersAndHBList(string sidx, string sord, int page, int rows, bool? _firstload, bool _search, long? User_ID, string Login, string FN, string Email, string Phone)
    {
      return (_firstload.HasValue && _firstload.Value) ? JSON(false) : JSON(UserRepository.GetBuyersAndHBList(sidx, sord, page, rows, _search, User_ID, Login, FN, Email, Phone));
    }

    //GetInvoiceBuyer
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetInvoiceBuyer(long? userinvoice_id)
    {
      return (!userinvoice_id.HasValue) ? JSON(false) : JSON(UserRepository.GetInvoiceUser(userinvoice_id.Value, true));
    }

    //GetInvoiceSellers
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetInvoiceSellers(string sidx, string sord, int page, int rows, long? userinvoice_id)
    {
      return (!userinvoice_id.HasValue) ? JSON(false) : JSON(UserRepository.GetInvoiceOwners(sidx, sord, page, rows, userinvoice_id.Value));
    }

    //GetConsignSeller
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetConsignSeller(long? cons_id)
    {
      return (!cons_id.HasValue) ? JSON(false) : JSON(UserRepository.GetInvoiceUser(cons_id.Value, false));
    }

    //GetConsignBuyers
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetConsignBuyers(string sidx, string sord, int page, int rows, long? cons_id)
    {
      return (!cons_id.HasValue) ? JSON(false) : JSON(UserRepository.GetInvoiceBuyers(sidx, sord, page, rows, cons_id.Value));
    }

    //GetUserDataJSON
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult GetUserDataJSON(long? user_id)
    {
      return JSON((!user_id.HasValue) ? false : UserRepository.GetUserDataJSON(user_id.Value));
    }

    //UpdateUser
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer")]
    public JsonResult UpdateUser(string user, string newSignature)
    {
      return JSON(String.IsNullOrEmpty(user) ? false : UserRepository.UpdateUserForm(user, ModelState, newSignature));
    }

    //DeleteUser
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult DeleteUser(long user_id)
    {
      return JSON(UserRepository.DeleteUser(user_id));
    }

    //ActivateUser
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult ActivateUser(long user_id)
    {
      return JSON(UserRepository.ActivatingUser(user_id));
    }

    //GetSellersForEvent
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult GetSellersForEvent(long event_id)
    {
      return JSON(UserRepository.GetSellersForEvent(event_id));
    }

    //GetBuyersForEvent
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult GetBuyersForEvent(long event_id)
    {
      return JSON(UserRepository.GetBuyersForEvent(event_id));
    }
  }
}