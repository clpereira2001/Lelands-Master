using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;

namespace Vauction.Utils.Perfomance
{
  public class VCache : ActionFilterAttribute
  {
    private int _time;
    private List<string> _params;
    private bool iseveryone;

    #region contructors
    public VCache()
    {
      _time = 0;
      _params = new List<string>();
      iseveryone = false;
    }

    public VCache(int Duration) : this()
    {
      _time = Duration;
    }

    public VCache(int Duration, string Params)
      : this(Duration)
    {
      if (String.IsNullOrEmpty(Params)) return;
      _params = Params.Split(',').ToList();
    }

    public VCache(int Duration, bool AllUsers)
      : this(Duration)
    {
      iseveryone = AllUsers;
    }

    public VCache(int Duration, string Params, bool AllUsers)
      : this(Duration, Params)
    {
      iseveryone = AllUsers;
    }
    #endregion

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpResponseBase response = filterContext.HttpContext.Response;
      if (response == null || response.StatusCode != 200) return;
      response.Cache.SetCacheability(HttpCacheability.Public);
      response.Cache.SetExpires(DateTime.Now.AddSeconds(_time));
      response.Cache.SetMaxAge(TimeSpan.FromSeconds(_time));
      response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
      if (!iseveryone) response.Cache.SetVaryByCustom("CurrentUser");
      foreach (string key in _params)
        response.Cache.VaryByParams[key] = true;
    }
  }
}