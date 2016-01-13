using System.Web.Mvc;
using Vauction.Utils.Autorization;

namespace Vauction.Controllers
{
  [CrossSessionCheck]
  public class ErrorController : BaseController
  {
    public ActionResult HttpError()
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request. (404)";
      return View("Error");
    }

    public ActionResult HttpError404(string error)
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request. (404)";
      return View("Error");
    }

    public ActionResult HttpError500(string error)
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request. (500)";
      return View("Error");
    }

    public ActionResult General(string error)
    {
      ViewData["Title"] = "Sorry, an error occurred while processing your request.";
      return View("Error");
    }

    public ActionResult Index(string error)
    {
      ViewData["Title"] = error;
      return View("Error");
    }

  }
}
