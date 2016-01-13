using System;
using System.Web;
using System.Web.Mvc;

namespace Vauction.Utils.Perfomance
{
  public class NoCacheAttribute : ActionFilterAttribute
  {
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      if (filterContext.IsChildAction) return;

      HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;

      cachePolicy.SetExpires(DateTime.UtcNow.AddDays(-1));
      cachePolicy.SetValidUntilExpires(false);
      //cachePolicy.SetRevalidation(HttpCacheRevalidation.AllCaches);
      cachePolicy.AppendCacheExtension("must-revalidate, proxy-revalidate");
      cachePolicy.SetCacheability(HttpCacheability.NoCache);
      cachePolicy.SetNoStore();
      
      base.OnResultExecuting(filterContext);
    }
  }
}