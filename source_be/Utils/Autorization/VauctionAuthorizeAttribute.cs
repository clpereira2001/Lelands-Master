using System;
using System.Web;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Configuration;

namespace Vauction.Utils.Autorization
{
  public class VauctionAuthorizeAttribute : AuthorizeAttribute
  {
    private TimeSpan statusCheckTime = Consts.AuthorizeStatusCheckTime;
    public new string Roles { get; set; }
    public string AccessDenyURL { get; set; }

    private void NotAuthorized(AuthorizationContext filterContext)
    {
      var cachePolicy = filterContext.HttpContext.Response.Cache;
      cachePolicy.SetExpires(DateTime.UtcNow.AddDays(-1));
      cachePolicy.SetProxyMaxAge(new TimeSpan(0));
      cachePolicy.SetValidUntilExpires(false);
      cachePolicy.AppendCacheExtension("must-revalidate, proxy-revalidate");
      cachePolicy.SetCacheability(HttpCacheability.NoCache);
      cachePolicy.SetNoStore();

      if (!String.IsNullOrEmpty(AccessDenyURL))
        filterContext.HttpContext.Response.Redirect(AccessDenyURL);
      else filterContext.Result = new HttpUnauthorizedResult();
    }

    public void LogOutUser(AuthorizationContext filterContext)
    {
      IFormsAuthenticationService formsService = new FormsAuthenticationService();
      formsService.SignOut();
      NotAuthorized(filterContext);
    }

    protected virtual bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (httpContext == null)
        throw new ArgumentNullException("httpContext");

      if (!httpContext.User.Identity.IsAuthenticated)
        return false;

      VauctionPrincipal principal = (httpContext.User as VauctionPrincipal);
      if (principal == null)
        return false;

      return true;
    }

    public override void OnAuthorization(AuthorizationContext filterContext)
    {
      if (filterContext == null)
        throw new ArgumentNullException("filterContext");

      if (AuthorizeCore(filterContext.HttpContext))
      {
        VauctionPrincipal principal = (filterContext.HttpContext.User as VauctionPrincipal);
        if (principal == null) { LogOutUser(filterContext); return; }
        VauctionIdentity identity = principal.UIdentity;

        var user = filterContext.HttpContext.Session[SessionKeys.User] as SessionUser;
        if (user == null || (string.Compare(user.IP, filterContext.HttpContext.Request.UserHostAddress, true) != 0 && user.IsBuyer )) { LogOutUser(filterContext); return; }

        bool isNeedToCheckStatus = principal.IsNeedToCheckStatus(statusCheckTime);
        if (isNeedToCheckStatus)
        {
          User usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserAdministrator(identity.ID, identity.Name);
          if (usr != null && user.Status != (byte)Consts.UserStatus.Active)
          { 
            IFormsAuthenticationService formsService = new FormsAuthenticationService();
            formsService.SignIn(usr.Login, identity.RememberMe, usr);
          }
          else
          {
            LogOutUser(filterContext);
            return;
          }
        }

        if (!String.IsNullOrEmpty(Roles))
        {
          string[] roles = Roles.Split(',');
          bool res = false;
          foreach (string role in roles)
            if (res = (role.Equals(((Consts.UserTypes)user.UserType).ToString(), StringComparison.InvariantCulture)))
              break;
          if (!res)
            filterContext.HttpContext.Response.Redirect("/Home/AccessDenyed");
        }
      }
      else if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
      {
        NotAuthorized(filterContext);
      }
    }
  }
}