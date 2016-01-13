using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class EventController : BaseController
  {
    #region init
    private IAuctionRepository AuctionRepository;
    private IEventRepository EventRepository;    
    public EventController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      EventRepository = dataProvider.EventRepository;
    }
    #endregion

    [VauctionAuthorize, HttpGet, Compress]
    public ActionResult Register(int? id)
    {
      InitCurrentEvent();
      Event evnt = (ViewData["CurrentEvent"] as Event);
      EventRepository.RegisterForEvent(AppHelper.CurrentUser.ID, evnt.ID);
      return RedirectToAction("AuctionDetail", "Auction", new { id = id });
    }

  }
}
