using System;
using System.Web;
using System.Web.Routing;
using Vauction.Models;

namespace Vauction.Utils.Routes
{
  public class LegacyURLRoutes : RouteBase
  {
    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      const string status = "301 Moved Permanently";
      HttpRequestBase request = httpContext.Request;
      HttpResponseBase response = httpContext.Response;
      string legacyUrl = request.Url.ToString();
      string newUrl = String.Empty;
      string legacyUrl_lower = legacyUrl.ToLower();

      if ((legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/?").ToLower()) && !legacyUrl_lower.Contains("trng=spa")) || legacyUrl_lower.Contains("/public/auctionimages/array") || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/auction/auctioneer.exe").ToLower()) || legacyUrl_lower.Contains("setup.php") || legacyUrl_lower.Contains("admin.php") || legacyUrl_lower.Contains("index.php") || legacyUrl_lower.Contains("phpmyadmin"))
      {
        newUrl = "/Error/HttpError";
      } else
      if (legacyUrl_lower.Contains("/public/scripts/jquery-1.3.2.min.js"))
      {
        newUrl = "/public/scripts/jquery-1.4.1.min.js";      
      } else
        if (legacyUrl_lower.Contains("index.aspx") || legacyUrl_lower.Contains("app_themes") || legacyUrl_lower.Contains("catalog_images_") || legacyUrl_lower.Contains("/msoffice/") || legacyUrl_lower.Contains("/_vti_bin/") || legacyUrl_lower.Contains("/photosofus") || legacyUrl_lower.Contains("/popupimage") || legacyUrl_lower.Contains("/images/lelands/"))
      {
        newUrl = "/Home/Index";
      }
      else if (legacyUrl_lower.Contains("forsale.aspx") || legacyUrl_lower.Contains("/home/forsale"))
      {
        newUrl = "/Home/Index"; //"/Home/ForSale";
      }
      else if (legacyUrl_lower.Contains("categories.aspx") || legacyUrl_lower.Contains("itemlist.aspx") || legacyUrl_lower.Contains("gallery.aspx") || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/events").ToLower()) || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/catalog").ToLower()) || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/itemlist").ToLower()) || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/bid").ToLower()) || legacyUrl_lower.Contains("updates.aspx") || legacyUrl_lower.Contains("pricesrealized.asp") || legacyUrl_lower.Contains("bid.asp") || legacyUrl_lower.Contains("lotdetail.asp") || legacyUrl_lower.Contains("mylelands.asp"))
      {
        newUrl = "/Auction/Category";
      }
      else if (legacyUrl_lower.Contains("signin.aspx"))
      {
        newUrl = "/Account/LogOn";
      }
      else if (legacyUrl_lower.Contains("search.aspx") || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/searchlist").ToLower()))
      {
        newUrl = "/Home/AdvancedSearch";
      }      
      else if (legacyUrl_lower.Contains("relatedlinks.aspx"))
      {
        newUrl = "/Home/RelatedLinks";
      }
      else if (legacyUrl_lower.Contains("sitemap.aspx") || legacyUrl_lower.Contains("auction.aspx?auctionid=") || legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/auctionresults").ToLower()))
      {
        newUrl = "/Auction/PastAuctionResults";
      }
      else if (legacyUrl_lower.Contains("consign.aspx"))
      {
        newUrl = "/Home/Consign";
      }
      else if (legacyUrl_lower.Contains("aboutus.aspx"))
      {
        newUrl = "/Home/About";
      }
      else if (legacyUrl_lower.Contains("contact.aspx"))
      {
        newUrl = "/Home.aspx/Consign#contactus";
      }
      else if (legacyUrl_lower.Contains("webrules.aspx"))
      {
        newUrl = "/Home/FAQs";
      }
      else if (legacyUrl_lower.Contains("password.aspx") || legacyUrl_lower.Contains("/account/fogotpassword"))
      {
        newUrl = "/Account/ForgotPassword";
      }      
      else if (legacyUrl_lower.Contains("/lotimages/") || legacyUrl_lower.Contains("/cgi-bin/"))
      {
        newUrl = "/Error/HttpError";
      }
      else if (legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/viewuserdefinedpage").ToLower()))
      {
        newUrl = "/Account/MyAccount";
      }
      else if (legacyUrl_lower.Contains(AppHelper.GetSiteUrl("/americanaitems").ToLower()))
      {
        newUrl = "/Auction/SubCategory/2/Americana";
      }
      else if (legacyUrl_lower.Contains("/auctiondetailed/"))
      {
        newUrl = legacyUrl.Replace("AuctionDetailed", "AuctionDetail");
      }
      else if (legacyUrl_lower.Contains("*"))
      {
        newUrl = legacyUrl.Replace("*", "-").Replace("--","-");
      }
      else if (legacyUrl_lower.Length>=250 && legacyUrl_lower.Contains("/auction/auctiondetail/"))
      {
        int index = legacyUrl_lower.IndexOf("/auction/auctiondetail/");
        if (index!=-1){
          index = legacyUrl_lower.IndexOf("/", index + 23);
          newUrl = legacyUrl.Substring(0, index);
        }
      }


      if (String.IsNullOrEmpty(newUrl))
      {
        if (legacyUrl_lower.Contains("/page1")) newUrl = legacyUrl.Replace("/page1", "");
        if (legacyUrl_lower.Contains(".aspx")) newUrl = legacyUrl.Replace(".aspx", "");
      }

      if (!String.IsNullOrEmpty(newUrl))
      {        
        response.Status = status;
        response.RedirectLocation = newUrl;
        response.End();
      }

      return null;
    }

    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
    {
      return null;
    }
  }
}