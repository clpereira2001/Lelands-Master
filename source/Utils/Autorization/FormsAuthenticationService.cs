using System;
using System.Web;
using System.Web.Security;
using Vauction.Models;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Autorization
{
  public class FormsAuthenticationService : IFormsAuthenticationService
  {
    public void SignIn(string userName, bool createPersistentCookie, User user)
    {
      if (String.IsNullOrEmpty(userName) || user == null)
        throw new ArgumentException("Value cannot be null or empty.", "userName");

      FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(Consts.FormsAuthenticationTicketTime), createPersistentCookie, String.Format("{0}|{1}|{2}", user.ID, DateTime.Now, createPersistentCookie), FormsAuthentication.FormsCookiePath);
      string encTicket = FormsAuthentication.Encrypt(authTicket);
      HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { HttpOnly = true });

      AppHelper.CurrentUser = SessionUser.Create(user);

      // begin 2013-03-17 -> check added
      Logger.LogInfo(String.Format("[LOG-IN] Id: {0} | Email: {1} | IP: {2} | Local IP: {3} | SessionID: {4}\n\t\t\tUser Agent: {5}", user.ID, user.Email, user.IP, HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"], HttpContext.Current.Session.SessionID, HttpContext.Current.Request.UserAgent));
      // end 2013-03-17 -> check added
    }

    public void SignOut()
    {
      var user = AppHelper.CurrentUser;
      // begin 2013-03-17 -> check added
      Logger.LogInfo(String.Format("[LOG-OUT] Id: {0} | Email: {1} | IP: {2} | Local IP: {3} | SessionID: {4}\n\t\t\tUser Agent: {5}", user == null ? "---" : user.ID.ToString(), user == null ? "---" : user.Email, Consts.UsersIPAddress, HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"], HttpContext.Current.Session != null ? HttpContext.Current.Session.SessionID : "---", HttpContext.Current.Request.UserAgent));
      // end 2013-03-17 -> check added

      FormsAuthentication.SignOut();
      
      if (HttpContext.Current.Session != null)
        HttpContext.Current.Session.Abandon();
      HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
    }
  }
}