using System;
using System.Web;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Configuration;
using Vauction.Utils.Lib;

namespace Vauction.Utils.Autorization
{
  public class VauctionAuthorizeAttribute : AuthorizeAttribute
  {
    private TimeSpan statusCheckTime = Consts.AuthorizeStatusCheckTime;
    public new string Roles { get; set; }
    public string AccessDenyURL { get; set; }
    public string IsBackendUser { get; set; }

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

    private void LogOutUser(AuthorizationContext filterContext)
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

        var user = AppHelper.CurrentUser;
        if (user == null) { LogOutUser(filterContext); return; }
        
        //#region added 2013-03-15 -> cross session check
        //VauctionIdentity videntity = principal.UIdentity;
        //if (videntity.ID != user.ID)
        //{
        //  Logger.LogInfo(String.Format("[SESSION-ERROR]: SessionID:{0} | CurrentUser:{1} | CrossedUser: {2} | IP: {3} | Local IP: {4}\n\t\t\tUser Agent: {5}", HttpContext.Current.Session.SessionID, videntity.ID, user.ID, Consts.UsersIPAddress, HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"], HttpContext.Current.Request.UserAgent));
        //  LogOutUser(filterContext); return;
        //}
        //#endregion

        bool isNeedToCheckStatus = principal.IsNeedToCheckStatus(statusCheckTime);
        if (isNeedToCheckStatus)
        {
          User usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserActiveAndApproved(identity.ID, identity.Name);
          if (usr != null && user.Status != (byte)Consts.UserStatus.Active)
          { 
            //IFormsAuthenticationService formsService = new FormsAuthenticationService();
            //formsService.SignIn(identity.Name, identity.RememberMe, usr);
          }
          else
          {
            LogOutUser(filterContext);
            return;
          }
        }
      
        bool isbackend = false;
        if (!String.IsNullOrEmpty(IsBackendUser) && Boolean.TryParse(IsBackendUser, out isbackend) && isbackend && !user.IsAdminType)
        {
          LogOutUser(filterContext);
          return;
        }

        if (!String.IsNullOrEmpty(Roles))
        {
          string[] roles = Roles.Split(',');
          bool res = false;
          foreach (string role in roles)
            if (res = (role.Equals(((Consts.UserTypes)user.UserType).ToString(), StringComparison.InvariantCulture)))
              break;
          if (!res)
            NotAuthorized(filterContext);
        }
      }
      else if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
      {
        NotAuthorized(filterContext);
      }
    }
  }
}