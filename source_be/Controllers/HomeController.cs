using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, VauctionAuthorize, Compress]
  public class HomeController : BaseController
  {

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Error()
    {
      ViewData["Msg"] = "";
      return View();
    }
    
    public ActionResult Error(string msg)
    {
      ViewData["Msg"] = msg;
      return View();
    }

    public ActionResult AccessDenyed()
    {
      return View();
    }    
  }
}
