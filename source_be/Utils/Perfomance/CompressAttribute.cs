using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;

namespace Vauction.Utils.Perfomance
{
  public class CompressAttribute : ActionFilterAttribute
  { 
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      HttpRequestBase request = filterContext.HttpContext.Request;
      HttpResponseBase response = filterContext.HttpContext.Response;

      string acceptEncoding = request.Headers["Accept-Encoding"];
      if (string.IsNullOrEmpty(acceptEncoding)) return;
      acceptEncoding = acceptEncoding.ToUpperInvariant();
      if (acceptEncoding.Contains("GZIP"))
      {
        response.AppendHeader("Content-encoding", "gzip");
        response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
      }
      else if (acceptEncoding.Contains("DEFLATE"))
      {
        response.AppendHeader("Content-encoding", "deflate");
        response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
      }
    }
  }
}