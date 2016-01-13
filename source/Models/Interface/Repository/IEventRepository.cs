using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Vauction.Models.CustomModels;

namespace Vauction.Models
{
  public interface IEventRepository
  {
    Event GetEventByID(long event_id);
    Event GetCurrent();
    List<Event> GetPastedEventsList();
    List<Event> GetConsingedEvents(long user_id);
    bool RegisterForEvent(long id, long p);
    bool IsUserRegisterForEvent(long user_id, long event_id);
  }
}
