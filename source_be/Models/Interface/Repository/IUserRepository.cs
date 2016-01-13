using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Vauction.Utils;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IUserRepository
  {
    User GetUserAdministrator(string login, string password);
    User GetUserAdministrator(string login);
    User GetUserAdministrator(long user_id, string login);
    void UserLogInSuccessfuly(IUser user);
    object GetUsersListSearchBox(string sidx, string sord, int page, int rows, byte usersearchtype, long? user_id, string login, string fn, string email, string phone);
    JsonExecuteResult ActivatingUserWithoutEmail(long user_id);
    JsonExecuteResult ChangeUserTypeToSellerBuyer(long user_id);
    object GetBuyersAndHBList(string sidx, string sord, int page, int rows, bool _search, long? User_ID, string Login, string FN, string Email, string Phone);





    //NOT DONE

    byte GetUserTypeID(string type);
    User GetUser(Int64 User_ID);
    User GetUser(string login);
    object GetUserDataJSON(long user_id);
    IQueryable<User> UsersList();
    IEnumerable<User> GetBuyers(bool only_active);    
    object GetInvoiceUser(long id, bool IsUserInvoice);
    object GetInvoiceOwners(string sidx, string sord, int page, int rows, long userinvoice_id);
    object GetInvoiceBuyers(string sidx, string sord, int page, int rows, long cons_id);
    object UpdateUserForm(string user, ModelStateDictionary ModelState, string newSignature);
    object DeleteUser(long user_id);
    bool ValidateLogin(string login, long ID);
    bool ValidateEmail(string email, long ID);
    object GetUsersList(string sidx, string sord, int page, int rows, bool _search, long? user_id, string login, byte? status, byte? usertype, int? commrate_id, string name, string email, DateTime? datein, string mobilephone, string dayphone);
    JsonExecuteResult ActivatingUser(long user_id);

    JsonExecuteResult GetSellersForEvent(long event_id);
    JsonExecuteResult GetBuyersForEvent(long event_id);


  }
}