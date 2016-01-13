using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class SalesController : BaseController
  {
    private readonly IAuctionRepository _auctionRepository;
    private readonly IEventRepository _eventRepository;

    public SalesController()
    {
      _auctionRepository = dataProvider.AuctionRepository;
      _eventRepository = dataProvider.EventRepository;
    }

    //Index
    [HttpGet, Compress]
    public ActionResult Index()
    {
      var evnt = _eventRepository.GetCurrent();
      if (evnt.Type_ID != (int) Consts.EventTypes.Sales)
      {
        return RedirectToAction("Index", "Home");
      }
      ViewData["Title"] = string.Format("{0}", evnt.Title);
      ViewData["Event"] = evnt;
      return View(_auctionRepository.GetProductsForSales(evnt.ID));
    }

    //SaleDetail
    [HttpGet, Compress]
    public ActionResult Details(long id)
    {
      InitCurrentEvent();
      var evnt = ViewData["CurrentEvent"] as Event;
      var auction = _auctionRepository.GetAuctionDetail(id, evnt.ID, true);
      if (auction == null || auction.Status != Consts.AuctionStatus.Open) return RedirectToAction("Index");
      return View(auction);
    }
  }
}