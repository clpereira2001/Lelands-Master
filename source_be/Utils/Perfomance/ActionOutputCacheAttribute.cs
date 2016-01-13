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
  public class ActionOutputCacheAttribute : ActionFilterAttribute
  {    
    private static MethodInfo _switchWriterMethod = typeof(HttpResponse).GetMethod("SwitchWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

    public ActionOutputCacheAttribute(int cacheDuration)
    {
      _cacheDuration = cacheDuration;
    }

    private int _cacheDuration;
    private TextWriter _originalWriter;
    private string _cacheKey;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      _cacheKey = ComputeCacheKey(filterContext);
      string cachedOutput = (string)filterContext.HttpContext.Cache[_cacheKey];
      if (cachedOutput != null)
      {
        filterContext.Result = new ContentResult { Content = cachedOutput };
      }
      else
      {
        _originalWriter = (TextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { new HtmlTextWriter(new StringWriter()) });
      }
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      if (_originalWriter != null) // Must complete the caching
      {
        HtmlTextWriter cacheWriter = (HtmlTextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { _originalWriter });
        string textWritten = ((StringWriter)cacheWriter.InnerWriter).ToString();
        filterContext.HttpContext.Response.Write(textWritten);
        filterContext.HttpContext.Cache.Add(_cacheKey, textWritten, null, DateTime.Now.AddSeconds(_cacheDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
      }
    }

    private string ComputeCacheKey(ActionExecutingContext filterContext)
    {
      var keyBuilder = new StringBuilder();
      foreach (var pair in filterContext.RouteData.Values)
        keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value != null ? pair.Value.GetHashCode() : (-1).GetHashCode());
      foreach (var pair in filterContext.ActionParameters)
        keyBuilder.AppendFormat("ap{0}_{1}_", pair.Key.GetHashCode(), pair.Value != null ? pair.Value.GetHashCode() : (-1).GetHashCode());
      return keyBuilder.ToString();
    }
  }
}