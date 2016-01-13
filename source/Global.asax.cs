using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Vauction.Controllers;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Routes;

namespace Vauction
{
  public class MvcApplication : HttpApplication
  {
    // RegisterRoutes
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("crossdomain.xml");
      routes.Add(new LegacyURLRoutes());

      routes.MapRoute("lot", "{controller}/{action}/{id}/{evnt}/{maincat}/{cat}/{lot}/", "Vauction", new {});
      routes.MapRoute("categorydetails", "{controller}/{action}/{id}/{evnt}/{maincat}/{cat}/", "Vauction", new {});
      routes.MapRoute("category", "{controller}/{action}/{id}/{evnt}/{maincat}/", "Vauction", new {});
      routes.MapRoute("event", "{controller}/{action}/{id}/{evnt}/", "Vauction", new {});
      routes.MapRoute("tag", "{controller}/{action}/{id}/{tag}/", "Vauction", new {});
      routes.MapRoute("past", "{controller}/{action}/{id}/", "Vauction", new {});
      routes.MapRoute("rest", "{controller}/{action}", "Vauction", new {});
      routes.MapRoute("Default.aspx", "{controller}/{action}", new {controller = "Home", action = "Index"});
    }

    // Application_Start
    protected void Application_Start()
    {
      RegisterRoutes(RouteTable.Routes);

      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT)
      {
        AppHelper.CacheDataProvider = new CacheDataProvider();
        Logger.LogInfo("Init: Memory Cache");
      }
      else
      {
        try
        {
          AppHelper.CacheDataProvider = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
          Logger.LogInfo("Init: AppFabric Cache");
        }
        catch
        {
          AppHelper.CacheDataProvider = new CacheDataProvider();
          Logger.LogInfo("Init: Memory Cache");
        }
      }
      Logger.LogInfo("Application starting");
    }

    // Application_AuthenticateRequest
    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      if (Request.Browser.Browser != "IE" &&
          (Context.Request.AcceptTypes != null && Context.Request.AcceptTypes.Any() &&
           Context.Request.AcceptTypes[0] != "text/html" && Context.Request.AcceptTypes[0] != "application/json" &&
           Context.Request.AcceptTypes[0] != "application/xml" && Context.Request.AcceptTypes[0] != "text/*" &&
           Context.Request.AcceptTypes[0] != "*/*" && Context.Request.AcceptTypes[0] != "application/x-ms-application"))
        return;

      if (HttpContext.Current.User != null && Request.IsAuthenticated)
      {
        var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
        if (authCookie == null || String.IsNullOrEmpty(authCookie.Value)) return;
        var ticket = FormsAuthentication.Decrypt(authCookie.Value);
        if (ticket != null)
          Context.User = new VauctionPrincipal(new VauctionIdentity(ticket.Name, ticket.UserData));
      }
    }

    //GetVaryByCustomString
    public override string GetVaryByCustomString(HttpContext context, string custom)
    {
      return (custom != "CurrentUser")
        ? base.GetVaryByCustomString(context, custom)
        : ((context.User != null && context.User.Identity.IsAuthenticated) ? context.User.Identity.Name : "none");
    }

    // Application_Error
    protected void Application_Error(object sender, EventArgs e)
    {
      var exception = Server.GetLastError();
      var cuser = AppHelper.CurrentUser;
      var adds = "[" + Request.Path + " | " + Consts.UsersIPAddress + " | " +
                 (cuser != null ? cuser.Login : String.Empty) + " | " +
                 (Request.Browser != null ? Request.Browser.Browser + "-" + Request.Browser.Version : String.Empty) +
                 "]" +
                 ((exception is HttpUnhandledException)
                   ? "\n >> Inner: " + (exception as HttpUnhandledException).InnerException.Message
                   : String.Empty);
      Logger.LogException(adds, exception);

      Response.Clear();

      var httpException = exception as HttpException;

      var routeData = new RouteData();
      if (exception is HttpAntiForgeryException)
      {
        Server.ClearError();
        Response.Redirect("/");
        return;
      }

      routeData.Values.Add("controller", "Error");

      if (httpException == null)
      {
        routeData.Values.Add("action", "General");
      }
      else //It's an Http Exception, Let's handle it.
      {
        switch (httpException.GetHttpCode())
        {
          case 404:
            // Page not found.
            routeData.Values.Add("action", "HttpError404");
            break;
          case 500:
            // Server error.
            routeData.Values.Add("action", "HttpError500");
            break;

            // Here you can handle Views to other error codes.
            // I choose a General error template  
          default:
            routeData.Values.Add("action", "General");
            break;
        }
      }

      // Pass exception details to the target error View.
      routeData.Values.Add("error", exception);

      // Clear the error on server.
      Server.ClearError();
      try
      {
        // Call target Controller and pass the routeData.
        IController errorController = new ErrorController();
        errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
      }
      catch (Exception exc)
      {
          string sexc = exc.Message;
      }
    }

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