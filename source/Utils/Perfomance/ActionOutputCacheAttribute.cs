using System;
using System.Web.Mvc;
using System.Web;
using System.Reflection;
using System.IO;
using System.Web.UI;
using System.Web.Caching;
using System.Text;

namespace Vauction.Utils.Perfomance
{
  //ActionOutputCacheAttribute
  public class ActionOutputCacheAttribute : ActionFilterAttribute
  {
    private static MethodInfo _switchWriterMethod = typeof(HttpResponse).GetMethod("SwitchWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

    public ActionOutputCacheAttribute(int cacheDuration)
    {
      _cacheDuration = cacheDuration;
      _cacheDurationAuthorized = 0;
      isseparateusers = false;
    }

    public ActionOutputCacheAttribute(int cacheDuration, int cacheDurationAthorized)
    {
      _cacheDuration = cacheDuration;
      _cacheDurationAuthorized = cacheDurationAthorized;
      isseparateusers = false;
    }

    public ActionOutputCacheAttribute(int cacheDuration, int cacheDurationAthorized, bool isSeparateUser)
    {
      _cacheDuration = cacheDuration;
      _cacheDurationAuthorized = cacheDurationAthorized;
      isseparateusers = isSeparateUser;
    }

    private bool isseparateusers;
    private int _cacheDuration;
    private int _cacheDurationAuthorized;
    private TextWriter _originalWriter;
    private string _cacheKey;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      _cacheKey = ComputeCacheKey(filterContext);
      string cachedOutput = (string)filterContext.HttpContext.Cache[_cacheKey];
      if (cachedOutput != null)
        filterContext.Result = new ContentResult { Content = cachedOutput };
      else
        _originalWriter = (TextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { new HtmlTextWriter(new StringWriter()) });
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      if (_originalWriter != null)
      {
        HtmlTextWriter cacheWriter = (HtmlTextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { _originalWriter });
        string textWritten = ((StringWriter)cacheWriter.InnerWriter).ToString();
        filterContext.HttpContext.Response.Write(textWritten);
        int _time = filterContext.HttpContext.User.Identity.IsAuthenticated && _cacheDurationAuthorized > 0 ? _cacheDurationAuthorized : _cacheDuration;
        filterContext.HttpContext.Cache.Add(_cacheKey, textWritten, null, DateTime.Now.AddSeconds(_time), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
      }
    }

    private string ComputeCacheKey(ActionExecutingContext filterContext)
    {
      var keyBuilder = new StringBuilder();
      foreach (var pair in filterContext.RouteData.Values)
        keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value != null ? pair.Value.GetHashCode() : "null".GetHashCode());
      foreach (var pair in filterContext.ActionParameters)
        keyBuilder.AppendFormat("ap{0}_{1}_", pair.Key.GetHashCode(), pair.Value != null ? pair.Value.GetHashCode() : "null".GetHashCode());
      if (_cacheDurationAuthorized > 0)
      {
        string user = filterContext.HttpContext.User.Identity.IsAuthenticated ? (isseparateusers ? filterContext.HttpContext.User.Identity.Name : "authorized_vuser") : "unauthorized_vuser";
        keyBuilder.AppendFormat("usr{0}_{1}_", "vauctionanouthorizeduser".GetHashCode(), user.GetHashCode());
      }
      return keyBuilder.ToString();
    }
  }
}