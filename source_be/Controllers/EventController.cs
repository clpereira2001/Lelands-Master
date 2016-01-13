using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Vauction.Models;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [Compress]
  public class EventController : BaseController
  {   
    private IEventRepository EventRepository;
    private ICategoryRepository CategoryRepository;
    private IGeneralRepository GeneralRepository;
    public EventController()
    {
      EventRepository = dataProvider.EventRepository;
      CategoryRepository = dataProvider.CategoryRepository;
      GeneralRepository = dataProvider.GeneralRepository;
    }

    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult Events()
    {
      return View();
    }
    
    [VauctionAuthorize(Roles = "Root,SpecialistViewer")]
    public ActionResult EventManagment()
    {
      //ViewData["Events"] = EventRepository.GetEvents();
      //ViewData["Event"] = EventRepository.GetCurrentEvent();
      return View();
    }

    //SetEventAsCurrent
    [VauctionAuthorize(Roles = "Root")]
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult SetEventAsCurrent(long event_id)
    {
      return JSON(EventRepository.SetEventAsCurrent(event_id));
    }

    [VauctionAuthorize(Roles = "Root")]
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult StopCurrentEvent(long event_id, byte step)
    {
      return JSON(EventRepository.StopCurrentEvent(event_id, step==1));
    }
    
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetEventsListShort(string sidx, string sord, int page, int rows, bool _search, long? Event_ID, string Title)
    {
      return JSON(EventRepository.GetEventsListShort(sidx, sord, page, rows, _search, Event_ID, Title));
    }

    //GetEventsList
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetEventsList(string sidx, string sord, int page, int rows, bool _search, long? Event_ID, string Title, DateTime? DateStart, DateTime? DateEnd, decimal? BuyerFee, string Description,byte? CloseStep )
    {
      return JSON(EventRepository.GetEventsList(sidx, sord, page, rows, _search, Event_ID, Title, DateStart, DateEnd, BuyerFee, Description, CloseStep));
    }

    //GetEventDataJSON
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin,SpecialistViewer")]
    public JsonResult GetEventDataJSON(long? event_id)
    {
      return JSON((!event_id.HasValue) ? false : EventRepository.GetEventDataJSON(event_id.Value));
    }

    //UpdateEvent
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult UpdateEvent(string evnt)
    {
      return JSON(String.IsNullOrEmpty(evnt) ? false : EventRepository.UpdateEventForm(evnt, ModelState));
    }

    //GetEventDefaultCategories
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetEventDefaultCategories()
    {
      return JSON(CategoryRepository.GetCategoriesMapDefault());
    }

    //GetInActiveCategories
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult GetInActiveCategories(long? event_id)
    {
      return JSON((!event_id.HasValue) ? false : EventRepository.GetInactiveCategories(event_id.Value));
    }

    //DeleteEvent
    [AcceptVerbs(HttpVerbs.Post)]
    [VauctionAuthorize(Roles = "Root,Admin")]
    public JsonResult DeleteEvent(long event_id)
    {
      return JSON(EventRepository.DeleteEvent(event_id));
    }

    //EventRegistrations
    [VauctionAuthorize(Roles = "Root,Admin")]
    public ActionResult EventRegistrations()
    {
      ViewData["UserTypes"] = GeneralRepository.GetUserTypesForSearchSelect();
      ViewData["EventsList"] = GeneralRepository.GetEventsListForSearchSelect();
      ViewData["HouseBiddersAmountForEventRegstration"] = Vauction.Utils.Consts.HouseBiddersAmountForEventRegstration;
      return View();
    }

    //GetEventRegistration
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult GetEventRegistration(string sidx, string sord, int page, int rows, bool? isfirstload, bool _search, long? EventRegistrationID, long? Event, long? UserID, string Login, byte? UserType)
    {
      return (!isfirstload.HasValue || isfirstload.Value) ? JSON(false) : JSON(EventRepository.GetEventRegistrations(sidx, sord, page, rows, _search, EventRegistrationID, Event, UserID, Login, UserType));
    }

    //UpdateEventRegistrations
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult UpdateEventRegistrations(long event_id, long? user_id, bool ishb)
    {
      return JSON(EventRepository.AddEventRegistration(event_id, (user_id.HasValue) ? user_id.Value : 0, ishb));
    }

    //DeleteEventRegistrations
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult DeleteEventRegistrations(long eventreg_id)
    {
      return JSON(EventRepository.DeleteEventRegistration(eventreg_id));
    }

    [VauctionAuthorize(Roles = "Root")]
    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult SendEndOfAuctionLetters(long event_id)
    {
      return JSON(EventRepository.SendAfterClosingEmails(event_id));
    }
  }
}
