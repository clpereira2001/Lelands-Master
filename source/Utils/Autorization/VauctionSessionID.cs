using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Vauction.Utils.Security;

namespace Vauction.Utils.Autorization
{
  public class VauctionSessionID : SessionIDManager
  {
    //private string GetSessionID(string text, long user_id)
    //{
    //  StringBuilder sb = new StringBuilder();
    //  foreach (char c in text) sb.Insert(0, (byte) c);
    //  int l = sb.Length;
    //  if (l>14) sb.Remove(14, l - 14);
    //  DateTime dt = DateTime.Now;
    //  sb.Insert(0, String.Format("{0}{1}{2}{3}", user_id, dt.Month, dt.DayOfYear, dt.Day ));
    //  return sb.ToString();
    //}

    //public override string CreateSessionID(HttpContext context)
    //{
    //  VauctionPrincipal principal = (context.User as VauctionPrincipal);
    //  return (principal == null || !context.Request.IsAuthenticated) ? Guid.NewGuid().ToString() : GetSessionID(principal.UIdentity.Name, principal.UIdentity.ID);
    //}

    //public override bool Validate(string id)  
    //{
    //  try
    //  {
    //    HttpContext context = HttpContext.Current;
    //    VauctionPrincipal principal = (context.User as VauctionPrincipal);
    //    return id == ((principal == null || !context.Request.IsAuthenticated) ? (new Guid(id)).ToString() : GetSessionID(principal.UIdentity.Name, principal.UIdentity.ID));
    //  }
    //  catch
    //  {
    //  }
    //  return false;
    //}

    private string GetSessionID(string ip, long user_id, string browser, string version, string platform)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0}{1}{2}{3}{4}", user_id.ToString("D7"), ip, browser, version, platform);
      sb = new StringBuilder(Encryption.EncryptPassword(sb.ToString(), "wAlla62Angl="));
      sb.Replace("+", String.Empty).Replace("=", String.Empty).Replace("/", String.Empty);
      if (sb.Length > 80)
        sb.Remove(80, sb.Length - 80);
      return sb.ToString();
    }

    public override string CreateSessionID(HttpContext context)
    {
      VauctionPrincipal principal = (context.User as VauctionPrincipal);
      return (principal == null || !context.Request.IsAuthenticated) ? Guid.NewGuid().ToString() : GetSessionID(context.Request.UserHostAddress, principal.UIdentity.ID, context.Request.Browser.Browser, context.Request.Browser.Version, context.Request.Browser.Platform);
    }

    public override bool Validate(string id)
    {
      try
      {
        HttpContext context = HttpContext.Current;
        VauctionPrincipal principal = (context.User as VauctionPrincipal);
        return id == ((principal == null || !context.Request.IsAuthenticated) ? (new Guid(id)).ToString() : GetSessionID(context.Request.UserHostAddress, principal.UIdentity.ID, context.Request.Browser.Browser, context.Request.Browser.Version, context.Request.Browser.Platform));
      }
      catch (Exception)
      {
      }
      return false;
    }
  }
}