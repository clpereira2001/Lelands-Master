using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils;

namespace Vauction.Controllers
{
  public class TrckController : BaseController
  {
    #region init
    private IDifferentRepository DifferentRepository;
    public TrckController(IDifferentRepository DifferentRepository)
    {
      this.DifferentRepository = DifferentRepository;      
    }
    #endregion

    //TrckEmail
    public ActionResult TrckEmail(string event_id, long? user_id, string type)
    {
      DifferentRepository.TrackEmail(Consts.UsersIPAddress, event_id, user_id, type);
      return RedirectToAction("Image", "Zip", new { path = "/public/images/1px.png" });      
    }

    //TrckFU
    public ActionResult TrckFU(string event_id, string url, long? user_id, string type)
    {
      if (!String.IsNullOrEmpty(url))
      {
        DifferentRepository.TrackForwardingURL(Consts.UsersIPAddress, event_id, url, user_id, type);
        return Redirect(url);
      }
      return RedirectToAction("Index", "Home");
    }

    
  }
}
