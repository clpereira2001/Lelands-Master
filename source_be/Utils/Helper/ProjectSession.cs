using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Configuration;
using Vauction.Models;
using Vauction.Controllers;

namespace Vauction.Utils.Helper
{
  public class SessionKeys
  {
    public const string UserSessionKey = "CurrentUser";    
  }
  public class AppHelper
  {
    public static IVauctionDataProvider DataProvider
    {
      get
      {
        return ProjectConfig.Config.DataProvider.GetInstance();
      }
    }

    public static bool IsAuthenticated
    {
      get
      {
        if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
          return true;

        return false;
      }
    }

    public static byte ItemViewType = (byte)Consts.AuctonViewMode.Table;

    public static IUser CurrentUser
    {
      get
      {
        IUser user = (IUser)HttpContext.Current.Session[SessionKeys.UserSessionKey];

        if (user == null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
        {
          if (BaseController.UserRepository != null)
          {
            user = BaseController.UserRepository.GetUser(HttpContext.Current.User.Identity.Name);
            HttpContext.Current.Session[SessionKeys.UserSessionKey] = user;
          }
          else
            user = null;
        }
        return user;
      }
      set
      {
        HttpContext.Current.Session[SessionKeys.UserSessionKey] = value;
      }
    }

    public static string GetSiteUrl()
    {
      string Port = String.Empty, Protocol = String.Empty, BaseUrl = String.Empty;

      //get the port we are operating on
      Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"].ToString();
      Port = (Port == null || Port == "80" || Port == "443") ? "" : ":" + Port;

      //load the protocol value
      Protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"].ToString();
      Protocol = Protocol == null || Protocol == "0" ? "http://" : "https://";

      //assemble the base path
      BaseUrl = Protocol + HttpContext.Current.Request.ServerVariables["Server_Name"].ToString() +
          Port + (HttpContext.Current.Request.ApplicationPath.ToString() == @"/" ? "" : HttpContext.Current.Request.ApplicationPath.ToString());

      return BaseUrl;
    }   
  }
}
