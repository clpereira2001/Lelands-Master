using System;
using System.Web;
using System.Web.Mvc;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Autorization
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class CrossSessionCheckAttribute : AuthorizeAttribute
  {
    private void AddNoOutputCacheHeaders(AuthorizationContext filterContext)
    {
      HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
      cachePolicy.SetExpires(DateTime.UtcNow.AddDays(-1));
      cachePolicy.SetValidUntilExpires(false);
      cachePolicy.AppendCacheExtension("must-revalidate, proxy-revalidate");
      cachePolicy.SetCacheability(HttpCacheability.NoCache);
      cachePolicy.SetNoStore();
    }

    public void LogOutUser(AuthorizationContext filterContext)
    {
      IFormsAuthenticationService formsService = new FormsAuthenticationService();
      formsService.SignOut();
      AddNoOutputCacheHeaders(filterContext);
      filterContext.Result = new HttpUnauthorizedResult();
    }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
      if (filterContext == null)
        throw new ArgumentNullException("filterContext");

      if (filterContext.HttpContext.Session == null || filterContext.IsChildAction)
        return;

      var user = AppHelper.CurrentUser;

      if (user != null && !filterContext.HttpContext.User.Identity.IsAuthenticated)
      {
        filterContext.HttpContext.Session.Abandon();
        return;
      }

      if (user != null && filterContext.HttpContext.User.Identity.IsAuthenticated)
      {
        var principal = (filterContext.HttpContext.User as VauctionPrincipal);
        if (principal == null) { /*LogOutUser(filterContext);*/ return; }
        VauctionIdentity identity = principal.UIdentity;
        if (identity.ID != user.ID)
        {
          Logger.LogInfo(String.Format("[SESSION-ERROR][CHECK]: Type:Mixed | SessionID:{0} | User_ID:{1} | CrossedUser_ID: {2}", HttpContext.Current.Session.SessionID, identity.ID, user.ID));
          LogOutUser(filterContext);
        }
      }
    }
  }
}