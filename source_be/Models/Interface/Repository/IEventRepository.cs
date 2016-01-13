using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IEventRepository
  {


    // NOT DONE


    List<Event> GetEvents();
    Event GetEvent(long event_id);
    Event GetCurrentEvent();
    //IEvent GetPreviousEvent();
    JsonExecuteResult StopCurrentEvent(long event_id, bool IsFirstStep);
    JsonExecuteResult SetEventAsCurrent(long event_id);
    object GetEventsListShort(string sidx, string sord, int page, int rows, bool _search, long? Event_ID, string Title);
    object GetEventDataJSON(long event_id);
    object GetEventsList(string sidx, string sord, int page, int rows, bool _search, long? event_id, string title, DateTime? datestart, DateTime? dateend, decimal? buyerfee, string description, byte? closestep);
    bool UpdateEvent(EventForm info);
    object UpdateEventForm(string evnt, ModelStateDictionary ModelState);
    object GetInactiveCategories(long event_id);
    object DeleteEvent(long event_id);
    object GetEventRegistrations(string sidx, string sord, int page, int rows, bool _search, long? evreg_id, long? event_id, long? user_id, string login, byte? usertype);
    JsonExecuteResult AddEventRegistration(long event_id, long user_id, bool ishb);
    JsonExecuteResult DeleteEventRegistration(long eventreg_id);
    JsonExecuteResult SendAfterClosingEmails(long event_id);
  }
}
