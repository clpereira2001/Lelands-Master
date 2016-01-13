using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Vauction.Utils.Lib;
using Vauction.Utils.Autorization;

namespace Vauction
{
  public class MvcApplication : HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      
      routes.MapRoute(
          "Default",                                              // Route name
          "{controller}/{action}/{id}",                           // URL with parameters
          new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
      );
    }

    //Application_Error
    void Application_Error(object sender, EventArgs e)
    {      
      Exception exception = Server.GetLastError();
      Logger.LogException("[" + Request.Path + "]", exception);
    }

    // Application_AuthenticateRequest
    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      if (HttpContext.Current.User != null && Request.IsAuthenticated)
      {
        HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
        if (authCookie == null || String.IsNullOrEmpty(authCookie.Value)) return;
        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
        Context.User = new VauctionPrincipal(new VauctionIdentity(ticket.Name, ticket.UserData));
      }
    }

    //Application_Start
    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);
      Logger.LogInfo("Application started");
    }

    //GetVaryByCustomString
    //public override string GetVaryByCustomString(HttpContext context, string custom)
    //{
    //  return (custom != "CurrentUser") ? base.GetVaryByCustomString(context, custom) : ((context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated) ? context.User.Identity.Name : "none");
    //}

    //Session_Start
    protected void Session_Start(object sender, EventArgs e)
    {
      HttpContext.Current.Session.Add("__Application", "App");
    }

    //Session_Start
    protected void Session_End(object sender, EventArgs e)
    {
    }
  }
}