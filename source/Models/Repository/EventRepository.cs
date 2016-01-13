using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using Vauction.Configuration;
using System.Data.Common;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Lib;
using System.Data.Linq;

namespace Vauction.Models
{
  public class EventRepository : IEventRepository
  {
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public EventRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
    {
      this.dataContext = dataContext;
      this.CacheRepository = CacheRepository;
    }

    //SubmitChanges
    private void SubmitChanges()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }

    //GetEventByID
    public Event GetEventByID(long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETEVENTBYID",
                                                 new object[] { event_id }, CachingExpirationTime.Hours_01);
      Event evnt = CacheRepository.Get(dco) as Event;
      if (evnt != null) return evnt;
      evnt = dataContext.spSelect_Event(event_id).SingleOrDefault();
      if (evnt != null)
      {
        dco.Data = evnt;
        CacheRepository.Add(dco);
      }
      return evnt;
    }

    //GetCurrent()
    public Event GetCurrent()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETCURRENT", null, CachingExpirationTime.Hours_01);
      Event evnt = CacheRepository.Get(dco) as Event;
      if (evnt != null) return evnt;
      evnt = dataContext.spEvent_Current().FirstOrDefault();
      if (evnt != null)
      {
        dco.Data = evnt;
        CacheRepository.Add(dco);
      }
      return evnt;
    }

    //GetPastedEventsList
    public List<Event> GetPastedEventsList()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETPASTEDEVENTSLIST",
                                                 new object[] { }, CachingExpirationTime.Hours_01);
      List<Event> result = CacheRepository.Get(dco) as List<Event>;
      if (result != null) return result;
      result = dataContext.spEvent_PastList().ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetConsingedEvents
    public List<Event> GetConsingedEvents(long user_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETCONSINGEDEVENTS",
                                                 new object[] { user_id }, CachingExpirationTime.Hours_01);
      List<Event> evnt = CacheRepository.Get(dco) as List<Event>;
      if (evnt != null) return evnt;
      evnt = dataContext.spEvent_Consigned(user_id).ToList();
      if (evnt.Count() > 0)
      {
        dco.Data = evnt;
        CacheRepository.Add(dco);
      }
      return evnt;
    }

    //GetEventRegistration
    private EventRegistration GetEventRegistration(long user_id, long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.EVENTREGISTRATIONS, "GETEVENTREGISTRATION",
                                                new object[] { user_id, event_id }, CachingExpirationTime.Days_01);
      EventRegistration result = CacheRepository.Get(dco) as EventRegistration;
      if (result != null) return result;
      result = dataContext.spSelect_EventRegistration(event_id, user_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //RegisterForEvent
    public bool RegisterForEvent(long user_id, long event_id)
    {
      try
      {
        EventRegistration registration = GetEventRegistration(user_id, event_id);
        if (registration != null) return true;
        registration = new EventRegistration();
        registration.Event_ID = event_id;
        registration.User_ID = user_id;
        dataContext.EventRegistrations.InsertOnSubmit(registration);
        SubmitChanges();
        CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.EVENTREGISTRATIONS, "GETEVENTREGISTRATION",
                                                new object[] { user_id, event_id }, CachingExpirationTime.Days_01, registration));
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user_id.ToString() + ";event_id=" + event_id.ToString() + "]", ex);
        return false;
      }
      return true;
    }

    //IsUserRegisterForEvent
    public bool IsUserRegisterForEvent(long user_id, long event_id)
    {
      return GetEventRegistration(user_id, event_id) != null;
    }
  }
}
