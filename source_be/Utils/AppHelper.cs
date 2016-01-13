using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Autorization;

namespace Vauction.Utils
{
  public static class AppHelper
  {
    #region properties

    //CurrentUser
    public static SessionUser CurrentUser
    {
      get { return AppSession.CurrentUser; }
      set { AppSession.CurrentUser = value; }
    }
    
    //ContentRoot
    public static string ContentRoot
    {
      get { return VirtualPathUtility.ToAbsolute("~/content"); }
    }

    //ImageRoot
    public static string ImageRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "images"); }
    }

    //ImageMenuRoot
    public static string ImageMenuRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "menu/images"); }
    }

    //AuctionImagesRoot
    public static string AuctionImagesRoot
    {
      get { return String.Format("{0}/{1}",Consts.GetVauctionFrontendDir, "public/auctionimages"); }
    }

    //CssRoot
    public static string CssRoot
    {
      get { return String.Format("{0}", ContentRoot); }
    }

    //ScriptRoot
    public static string ScriptRoot
    {
      get { return String.Format("{0}/{1}", "~", "scripts"); }
    }
    //ScriptVauctionRoot
    public static string ScriptVauctionRoot
    {
      get { return String.Format("{0}/{1}", ScriptRoot, "vauction"); }
    }

    //ImageCompressMethod
    public static string ImageCompressMethod
    {
      get { return "/Zip/Image"; }
    }
    
    //ScriptCompressMethod
    public static string ScriptCompressMethod
    {
      get { return "/Zip/Script"; }
    }

    //ScriptCompressMethod
    public static string StyleCompressMethod
    {
      get { return "/Zip/Style"; }
    }
    #endregion

    #region methods
    
    //ImageUrl
    public static string ImageUrl(string file)
    {
      return String.Format("{0}/{1}", ImageRoot, file.ToLower());
    }

    //ImageMenuUrl
    public static string ImageMenuUrl(string file)
    {
      return String.Format("{0}/{1}", ImageMenuRoot, file.ToLower());
    }

    //AuctionImage
    public static string AuctionImage(long auction_id, string file)
    {
      return String.Format("{0}/{1}/{2}", AuctionImagesRoot, String.Format("{0}/{1}/{2}", auction_id / 1000000, auction_id / 1000, auction_id), file.ToLower());
    }

    //CssUrl
    public static string CssUrl(string file)
    {
      return String.Format("{0}/{1}", CssRoot, file.ToLower());
    }

    //ScriptUrl
    public static string ScriptUrl(string file)
    {
      return String.Format("{0}/{1}", ScriptRoot, file.ToLower());
    }

    //ScriptVauctionUrl
    public static string ScriptVauctionUrl(string file)
    {
      return String.Format("{0}/{1}", ScriptVauctionRoot, file.ToLower());
    }

    //GetSiteUrl
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
      BaseUrl = Protocol + HttpContext.Current.Request.ServerVariables["Server_Name"].ToString() + Port + (HttpContext.Current.Request.ApplicationPath.ToString() == @"/" ? "" : HttpContext.Current.Request.ApplicationPath.ToString());
      return BaseUrl;
    }

    //GetSiteUrl
    public static string GetSiteUrl(string url)
    {
      return String.Format("{0}{1}", GetSiteUrl(), url);
    }

    //CompressImage
    public static string CompressImage(string file)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, ImageUrl(file));
    }

    //CompressImage
    public static string CompressImageMenu(string file)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, ImageMenuUrl(file));
    }

    //CompressImagePath
    public static string CompressImagePath(string path)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, path.ToLower());
    }

    //CompressStyle
    public static string CompressStyle(string file)
    {
      return String.Format("{0}?path={1}", StyleCompressMethod, CssUrl(file));
    }

    //CompressScript
    public static string CompressScript(string file)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, AppHelper.ScriptUrl(file));
    }

    //CompressScriptVauction
    public static string CompressScriptVauction(string file)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, AppHelper.ScriptVauctionUrl(file));
    }

    //CompressScriptForFiles
    public static string CompressScriptForFiles(string path)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, path);
    }

    //IsUrlLocalToHost
    public static bool IsUrlLocalToHost(this HttpRequestBase request, string url)
    {
      if (String.IsNullOrEmpty(url)) return false;
      Uri absoluteUri;
      return (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri)) ? String.Equals(request.Url.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase) : !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) && Uri.IsWellFormedUriString(url, UriKind.Relative);
    }
    #endregion

    #region debug/release
    public static string CacheLabel
    {
      get { return Consts.CacheModeDebuging ? String.Format("<span style='font-size:10px;color:Blue;font-weight:bold'>cached: {0}</span>", DateTime.Now) : String.Empty; }
    }
    #endregion
  }
}