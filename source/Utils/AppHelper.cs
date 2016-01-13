using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

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

    //CacheDataProvider
    public static ICacheDataProvider CacheDataProvider
    {
      get { return AppApplication.CacheProvider; }
      set { AppApplication.CacheProvider = value; }
    }

    //ContentRoot
    private static string ContentRoot
    {
      get { return VirtualPathUtility.ToAbsolute(String.Format("{0}/public", "~")); }
    }

    //ImageRoot
    private static string ImageRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "images"); }
    }

    //ImageMenuRoot
    private static string ImageMenuRoot
    {
      get { return String.Format("{0}/{1}", ImageRoot, "menu"); }
    }

    //AuctionImagesRoot
    private static string AuctionImagesRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "auctionimages"); }
    }

    //CssRoot
    private static string CssRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "css"); }
    }

    //ScriptRoot
    private static string ScriptRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "scripts"); }
    }

    //ImageCompressMethod
    private static string ImageCompressMethod
    {
      get { return String.Format("{0}/Zip/Image", Consts.ResourceHostName); }
    }

    //ScriptCompressMethod
    private static string ScriptCompressMethod
    {
      get { return String.Format("{0}/Zip/Script", Consts.ResourceHostName); }
    }

    //ScriptCompressMethod
    private static string StyleCompressMethod
    {
      get { return String.Format("{0}/Zip/Style", Consts.ResourceHostName); }
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

    //CompressImagePath
    public static string CompressImagePath(string path)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, path.ToLower());
    }

    //CompressAuctionImage
    public static string CompressAuctionImage(long auction_id, string file)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, AuctionImage(auction_id, file));
    }

    //CompressStyle
    public static string CompressStyle(string file)
    {
      return String.Format("{0}?path={1}", StyleCompressMethod, CssUrl(file));
    }

    //CompressStyleForFiles
    public static string CompressStyleForFiles(string path)
    {
      return String.Format("{0}?path={1}", StyleCompressMethod, path);
    }

    //CompressScript
    public static string CompressScript(string file)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, AppHelper.ScriptUrl(file));
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

    //ConsignmentContractOnDisk
    public static string ConsignmentContractOnDisk(long id, string filename)
    {
      string path = HttpContext.Current.Server.MapPath(String.Format("{0}/Files/ConsignmentContract/{1}/{2}/{3}", ContentRoot,
        id / 1000000, (id / 1000), id));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      return String.Format("{0}/{1}", path, filename);
    }

    //PublicImagesOnDisk
    public static string PublicImagesOnDisk(string filename)
    {
      return HttpContext.Current.Server.MapPath(String.Format("{0}/{1}", ImageRoot, filename));
    }

    //SignatureImagesOnDisk
    public static string SignatureImagesOnDisk(string filename)
    {
      return HttpContext.Current.Server.MapPath(String.Format("/Public/Files/Signature/{0}", filename));
    }
    #endregion

  }
}