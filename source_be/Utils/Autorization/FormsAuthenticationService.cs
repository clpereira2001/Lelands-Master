using System;
using System.Web;
using System.Web.Security;
using Vauction.Models;

namespace Vauction.Utils.Autorization
{
  public class FormsAuthenticationService : IFormsAuthenticationService
  {
    public void SignIn(string userName, bool createPersistentCookie, User user)
    {
      if (String.IsNullOrEmpty(userName) || user == null)
        throw new ArgumentException("Value cannot be null or empty.", "userName");
      SignIn(userName, createPersistentCookie, user.ID);
      AppHelper.CurrentUser = SessionUser.Create(user);
    }

    public void SignIn(string userName, bool createPersistentCookie, long user_id)
    {
      if (String.IsNullOrEmpty(userName))
        throw new ArgumentException("Value cannot be null or empty.", "userName");
      FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(Consts.FormsAuthenticationTicketTime), createPersistentCookie, String.Format("{0}|{1}|{2}", user_id, DateTime.Now, createPersistentCookie), FormsAuthentication.FormsCookiePath);
      string encTicket = FormsAuthentication.Encrypt(authTicket);
      HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));      
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
      HttpContext.Current.Session.Abandon();
      //AppHelper.CurrentUser = null;
    }
  }
}