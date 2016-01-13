using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq;
using System.Text;
using Vauction.Utils;
using Vauction.Utils.Helper;
using System.Transactions;
using Vauction.Utils.Lib;
using Vauction.Models.CustomClasses;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace Vauction.Models
{
  public class EventRepository : IEventRepository
  {
    private VauctionDataContext dataContext;

    public EventRepository(VauctionDataContext dataContext)
    {
      this.dataContext = dataContext;
    }



    //NOT DONE



    private void SubmitChanges()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
      catch (ForeignKeyReferenceAlreadyHasValueException f)
      {
        Vauction.Utils.Lib.Logger.LogWarning(f.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
      catch (DuplicateKeyException d)
      {
        Vauction.Utils.Lib.Logger.LogWarning(d.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }

    //GetEvent
    public Event GetEvent(long event_id)
    {
      return dataContext.Events.SingleOrDefault(E => E.ID == event_id);
    }

    //GetEvents
    public List<Event> GetEvents()
    {
      return (from E in dataContext.Events
              where E.IsViewable && E.IsClickable
              orderby E.IsViewable descending, E.IsClickable descending, E.DateStart descending, E.DateEnd descending
              select E).ToList();
    }

    ////GetPreviousEvent
    //public IEvent GetPreviousEvent()
    //{
    //  var query = (from e in dataContext.Events
    //               where e.IsClickable && e.IsViewable
    //               orderby e.DateEnd descending
    //               select e);
    //  return (query.Count() > 0) ? query.First() : null;
    //}

    //GetCurrentEvent
    public Event GetCurrentEvent()
    {
      return dataContext.spEvent_Current().FirstOrDefault();
    }

    //GetEventsListShort
    public object GetEventsListShort(string sidx, string sord, int page, int rows, bool _search, long? Event_ID, string Title)
    {
      List<Event> events = ((from E in dataContext.Events
                             select E).OrderBy(sidx + " " + sord)).ToList();

      if (_search)
      {
        if (events.Count > 0 && Event_ID.HasValue)
          events = new List<Event>(events.Where(E => E.ID == Event_ID.Value));
        if (events.Count > 0 && !String.IsNullOrEmpty(Title))
          events = new List<Event>(events.Where(E => E.Title.ToLower().Contains(Title.ToLower())));
      }

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = events.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      events = new List<Event>(events.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in events
            select new
            {
              i = query.ID,
              cell = new string[] {
                query.ID.ToString(),
                query.Title,              
                query.DateStart.ToString(),
                query.DateEnd.ToString()
              }
            }).ToArray()
      };

      events = null;

      return jsonData;
    }

    //SendAfterClosingEmails
    public JsonExecuteResult SendAfterClosingEmails(long event_id)
    {
      bool iserror = false;
      try
      {
        Event evnt = GetEvent(event_id);
        if (evnt == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Event doesn't exist in the database.");
        if (evnt.CloseStep != 2) throw new Exception("This event is not closed");

        EndOfAuction eoa;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Emails already sent to users:");
        dataContext.CommandTimeout = 6000000;
        foreach (var user in dataContext.spUser_GetBiddersForEventClose(event_id).ToList())
        {
          try
          {
            eoa = new EndOfAuction();
            eoa.EventTitle = evnt.Title;
            eoa.UserName = String.Format("{0} {1}{2}", user.FirstName, String.IsNullOrEmpty(user.MiddleName) ? String.Empty : user.MiddleName + " ", user.LastName);
            eoa.Invoices.AddRange(
              (from p in dataContext.spInvoice_Detail_UserUnpaid(user.ID, event_id)
               where p.Event_ID == event_id
               select new InvoiceDetail
               {
                 Auction_ID = p.Auction_ID,
                 Cost = p.Cost,
                 Invoice_ID = p.Invoice_ID,
                 LateFee = p.LateFee,
                 LinkParams =
                   new LinkParams
                   {
                     ID = p.Auction_ID,
                     Lot = p.Lot.GetValueOrDefault(0),
                     Title = p.Title,
                     Event_ID = p.Event_ID,
                     EventTitle = p.EventTitle,
                     EventCategory_ID = p.EventCategory_ID,
                     CategoryTitle = p.Category
                   },
                 Lot = p.Lot.GetValueOrDefault(0),
                 SaleDate = p.DateCreated,
                 Shipping = p.Shipping,
                 Tax = p.Tax,
                 Title = p.Title,
                 UserInvoice_ID = p.UserInvoices_ID.GetValueOrDefault(0)

               }).ToList());

            eoa.LoserLots.AddRange((from p in dataContext.spBid_View_BidWatch(user.ID, event_id)
                                    where p.IsWatch.GetValueOrDefault(0) == 0
                                    select new UserBidWatch
                                    {
                                      Amount = p.Amount.GetValueOrDefault(0),
                                      Quantity = p.Quantity,
                                      MaxBid = p.MaxBid.GetValueOrDefault(0),
                                      Option = p.IsWatch.GetValueOrDefault(0),
                                      LinkParams =
                                        new LinkParams
                                        {
                                          ID = p.Auction_ID,
                                          Lot = p.Lot.GetValueOrDefault(0),
                                          Title = p.Title,
                                          EventTitle = p.EventTitle,
                                          CategoryTitle = p.CategoryTitle
                                        },
                                      Bids = p.Bids.GetValueOrDefault(0),
                                      CurrentBid = p.CurrentBid.GetValueOrDefault(0),
                                      HighBidder = p.HighBidder
                                    }).ToList());
            Mail.SendEndOfAuctionHTMLLetter(user.Email, eoa);
            sb.AppendLine(user.Email);
          }
          catch (Exception ex)
          {
            iserror = true;
            Logger.LogException("Winner email to user " + user.Login + " failed : " + ex.Message + " (" + ex.StackTrace + ")", ex);
          }
        }
        Logger.LogInfo(sb.ToString());


        //Event current = GetEvent(event_id);
        //if (current == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Event doesn't exist in the database.");
        //if (current.CloseStep != 2) throw new Exception("This event is not closed");

        //List<Auction> allAuctions = (
        //  from A in dataContext.Auctions
        //  where A.Event_ID == current.ID && A.Status == (byte)Consts.AuctionStatus.Closed && A.Bids.Count > 0
        //  orderby A.Lot.Value ascending
        //  select A).ToList();

        //User user;
        //sp_GetAuctionWinnerTableResult bid;
        //List<sp_GetAuctionWinnerTableResult> bids;
        //foreach (Auction auc in allAuctions)
        //{
        //  bid = dataContext.sp_GetAuctionWinnerTable(auc.ID, 1).FirstOrDefault();

        //  user = dataContext.Users.SingleOrDefault(U => U.ID == bid.User_ID);
        //  if ((auc.Reserve.HasValue && auc.Reserve.Value > bid.Amount) || (user.UserType_ID != (byte)Consts.UserTypes.Buyer && user.UserType_ID != (byte)Consts.UserTypes.SellerBuyer) || String.IsNullOrEmpty(user.Email)) continue;

        //  if (user == null || !user.IsRecievingLotClosedNotice || String.IsNullOrEmpty(user.Email)) continue;
        //  try
        //  {
        //    Mail.SendWinningLetter(user.AddressCard_Billing.FirstName, user.AddressCard_Billing.LastName, user.Email, (long)auc.Lot.Value, auc.Title, bid.Amount.GetCurrency());
        //  }
        //  catch (Exception ex)
        //  {
        //    Logger.LogInfo("Winner email to user " + user.Login + " failed : " + ex.Message);
        //  }

        //  if (auc.Bids.Count == 1) continue;

        //  bids = dataContext.sp_GetAuctionWinnerTable(auc.ID, 0).ToList();
        //  foreach (var winbid in bids)
        //  {
        //    bid = winbid;
        //    user = dataContext.Users.SingleOrDefault(U => U.ID == bid.User_ID);

        //    if ((user.UserType_ID != (byte)Consts.UserTypes.Buyer && user.UserType_ID != (byte)Consts.UserTypes.SellerBuyer) || String.IsNullOrEmpty(user.Email)) continue;
        //    if (user == null || !user.IsRecievingLotSoldNotice || String.IsNullOrEmpty(user.Email)) continue;
        //    try
        //    {
        //      Mail.SendEndOfAuctionLetter(user.Email, (long)auc.Lot.Value, auc.Title);
        //    }
        //    catch (Exception ex)
        //    {
        //      Logger.LogInfo("Winner email to user " + user.Login + " failed : " + ex.Message);
        //    }
        //  }
        //}
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, "The event was closed successfuly! " + (iserror ? "(Note: watch the log file for details)" : String.Empty));
    }

    //StopCurrent
    public JsonExecuteResult StopCurrentEvent(long event_id, bool IsFirstStep)
    {
      try
      {
        Event current = GetEvent(event_id);
        if (current == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Event doesn't exist in the database.");
        if (!current.IsCurrent) throw new Exception("This event is not current");

        dataContext.CommandTimeout = 600000;
        dataContext.sp_Event_Close(current.ID, IsFirstStep);
        try
        {
          System.Net.WebClient client = new System.Net.WebClient();
          client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
          client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod);
        }
        catch (Exception ex)
        {
          Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod + "]", ex);
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, IsFirstStep ? "The fisrt step was finished successfuly." : "The event was closed successfuly!", IsFirstStep ? 1 : 2);
    }

    //SetEventAsCurrent
    public JsonExecuteResult SetEventAsCurrent(long event_id)
    {
      try
      {
        Event evnt = dataContext.Events.SingleOrDefault(E => E.ID == event_id);
        if (evnt == null || evnt.IsCurrent)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't set this event as current, because it is current already.");
        List<Event> oldEvents = dataContext.Events.Where(E => E.IsCurrent).ToList();
        foreach (Event ev in oldEvents)
        {
          JsonExecuteResult result = StopCurrentEvent(ev.ID, false);
          if (result.Status != JsonExecuteResultTypes.SUCCESS.ToString())
            return result;
        }
        dataContext.CommandTimeout = 600000;
        dataContext.sp_Event_SetCurrent(event_id);

        try
        {
          System.Net.WebClient client = new System.Net.WebClient();
          client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
          client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod);
        }
        catch (Exception ex)
        {
          Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod + "]", ex);
        }

      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetEventDataJSON
    public object GetEventDataJSON(long eventId)
    {
      Event e = GetEvent(eventId);
      if (e == null) return null;
      return new
      {
        id = e.ID,
        t = e.Title,
        o = e.Ordinary.HasValue ? e.Ordinary.Value : 1,
        s_1 = e.DateStart.Date.ToString("yyyy-MM-dd"),
        s_2 = e.DateStart.TimeOfDay.ToString(),
        e_1 = e.DateEnd.Date.ToString("yyyy-MM-dd"),
        e_2 = e.DateEnd.TimeOfDay.ToString(),
        bf = e.BuyerFee.GetPrice(),
        iv = e.IsViewable,
        icl = e.IsClickable,
        ic = e.IsCurrent,
        d = e.Description,
        c = e.ActiveCategoriesList,
        clst = e.CloseStep,
        et = e.Type_ID
      };
    }

    //GetEventsList   
    public object GetEventsList(string sidx, string sord, int page, int rows, bool _search, long? event_id, string title, DateTime? datestart, DateTime? dateend, decimal? buyerfee, string description, byte? closestep)
    {
      byte orderby = 0;
      switch (sidx)
      {
        case "DateStart":
          orderby = (byte)((sord == "desc") ? 2 : 3);
          break;
        case "DateEnd":
          orderby = (byte)((sord == "desc") ? 4 : 5);
          break;
        case "Title":
          orderby = (byte)((sord == "desc") ? 6 : 7);
          break;
        case "IsClickable":
          orderby = (byte)((sord == "desc") ? 10 : 11);
          break;
        case "Ordinary":
          orderby = (byte)((sord == "desc") ? 12 : 13);
          break;
        case "BuyerFee":
          orderby = (byte)((sord == "desc") ? 14 : 15);
          break;
        case "IsViewable":
          orderby = (byte)((sord == "desc") ? 16 : 17);
          break;
        case "CloseStep":
          orderby = (byte)((sord == "desc") ? 18 : 19);
          break;
        case "LastUpdate":
          orderby = (byte)((sord == "desc") ? 20 : 21);
          break;
        default:
          orderby = (byte)((sord == "desc") ? 0 : 1);
          break;
      }
      dataContext.CommandTimeout = 10 * 60 * 1000;
      List<Event> events = dataContext.sp_GetEventsList(event_id, title, datestart, dateend, buyerfee, description, closestep, orderby).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = events.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      events = new List<Event>(events.Skip(pageIndex * pageSize).Take(pageSize));

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in events
            select new
            {
              i = query.ID,
              cell = new string[] {
                query.ID.ToString(),
                (query.Ordinary.HasValue)?query.Ordinary.Value.ToString():String.Empty,
                (query.IsCurrent)?"current":((query.DateEnd.CompareTo(DateTime.Now)<0)?"old":"pending"),
                query.Title,              
                query.DateStart.ToString(),
                query.DateEnd.ToString(),                
                query.IsViewable.ToString(),
                query.IsClickable.ToString(),
                query.CloseStep.ToString(),
                query.BuyerFee.GetCurrency(false),                
                query.IsCurrent.ToString()                
              }
            }).ToArray()
      };

      events = null;

      return jsonData;
    }

    //UpdateEventForm
    public object UpdateEventForm(string evnt, ModelStateDictionary ModelState)
    {
      try
      {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        EventForm e = serializer.Deserialize<EventForm>(evnt);

        e.Validate(ModelState);

        if (e.DateEnd.CompareTo(DateTime.Now) <= 0 && AppHelper.CurrentUser.IsAdmin)
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't update this event.");

        if (ModelState.IsValid)
        {
          if (!UpdateEvent(e))
            return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The event's information wasn't saved.");
        }
        else
        {
          ModelState.Remove("evnt");
          var errors = (from M in ModelState select new { field = M.Key, message = M.Value.Errors.FirstOrDefault().ErrorMessage }).ToArray();
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please correct the errors and try again.", errors);
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //AddCategoryChildToEventCategory
    private void AddCategoryChildToEventCategory(Event evnt, CategoriesMap cm)
    {
      EventCategory evnt_cat = dataContext.EventCategories.SingleOrDefault(EC => EC.Event_ID == evnt.ID && EC.MainCategory_ID == cm.MainCategory_ID && EC.Category_ID == cm.Category_ID);
      if (evnt_cat == null)
      {
        evnt_cat = new EventCategory();
        dataContext.EventCategories.InsertOnSubmit(evnt_cat);
      }
      evnt_cat.Category_ID = cm.Category_ID;
      evnt_cat.MainCategory_ID = cm.MainCategory_ID;
      evnt_cat.IsActive = true;
      evnt_cat.LastUpdate = DateTime.Now;
      evnt_cat.Owner_ID = AppHelper.CurrentUser.ID;
      evnt_cat.Event_ID = evnt.ID;
      evnt_cat.Descr = cm.FullCategory; //evnt_cat.FullCategory;
      evnt_cat.Priority = cm.Priority;
      evnt.EventCategories.Add(evnt_cat);
    }

    //UpdateEvent
    public bool UpdateEvent(EventForm info)
    {
      Event evnt = (info.ID > 0) ? GetEvent(info.ID) : null;
      bool newEvent = evnt == null;
      bool iscurrent = !newEvent && GetCurrentEvent().ID == info.ID;
      try
      {
        using (TransactionScope ts = new TransactionScope())
        {
          if (newEvent)
          {
            evnt = new Event();
            dataContext.Events.InsertOnSubmit(evnt);
          }
          evnt.BuyerFee = info.BuyerFee;
          evnt.DateEnd = info.DateEnd;
          evnt.DateStart = info.DateStart;
          evnt.Description = info.Description;
          evnt.IsClickable = info.IsClickable;
          evnt.IsCurrent = info.IsCurrent;
          evnt.IsViewable = info.IsViewable;
          evnt.LastUpdate = DateTime.Now;
          evnt.Ordinary = info.Ordinary;
          evnt.Title = info.Title;
          evnt.Type_ID = info.Type_ID;

          if (newEvent)
            GeneralRepository.SubmitChanges(dataContext);
          if (evnt.EventCategories.Count > 0 && !newEvent)
            foreach (EventCategory ec in evnt.EventCategories)
              ec.IsActive = false;

          CategoriesMap cat_map;
          foreach (long index in info.CategoriesList)
          {
            cat_map = dataContext.CategoriesMaps.SingleOrDefault(CM => CM.ID == index);
            if (cat_map != null)
              AddCategoryChildToEventCategory(evnt, cat_map);
          }
          GeneralRepository.SubmitChanges(dataContext);
          if (!newEvent && evnt.CloseStep == 0)
            dataContext.sp_Event_ChangeAuctionStartEnd(evnt.ID);

          ts.Complete();
        }
      }
      catch (ChangeConflictException cce)
      {
        Logger.LogException(cce);
        throw cce;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        throw ex;
      }
      if (iscurrent || GetCurrentEvent().ID == evnt.ID)
      {
        try
        {
          System.Net.WebClient client = new System.Net.WebClient();
          client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
          client.OpenRead(Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod);
        }
        catch (Exception ex)
        {
          Logger.LogException("[" + Consts.CacheClearFrontendIP + Consts.FrontEndClearMethod + "]", ex);
        }
      }
      return true;
    }

    //GetInactiveCategories
    public object GetInactiveCategories(long event_id)
    {
      List<EventCategory> event_cats = (from E in dataContext.EventCategories
                                        where !E.IsActive && E.Event_ID == event_id
                                        select E).ToList();

      SortedList<string, long> cats = new SortedList<string, long>();
      foreach (EventCategory ec in event_cats)
      {
        string str = ec.FullCategory;
        if (!cats.ContainsKey(str))
          cats.Add(str, ec.ID);
      }

      var jsonData = new
      {
        total = 1,
        page = 1,
        records = cats.Count(),
        rows = (
            from query in cats
            select new
            {
              i = query.Value.ToString(),
              cell = new string[] {
                query.Value.ToString(),
                query.Key,
              }
            }).ToArray()
      };

      event_cats = null;
      cats = null;

      return jsonData;
    }

    //DeleteEvent
    public object DeleteEvent(long event_id)
    {
      Event evnt = GetEvent(event_id);
      if (evnt == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The event doesn't exist. Operation failed");
      int count = (from A in dataContext.Auctions where A.Event_ID == event_id select A).Count();
      if (count > 0)
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this event because there are auctions which are reference to this event.");
      count = (from A in dataContext.Consignments where A.Event_ID == event_id select A).Count();
      if (count > 0)
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this event because there are " + evnt.Consignments.Count.ToString() + " consignment statements which are reference to this event.");
      count = (from A in dataContext.UserInvoices where A.Event_ID == event_id select A).Count();
      if (count > 0)
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "You can't delete this event because there are " + evnt.UserInvoices.Count.ToString() + " users invoices which are reference to this event.");
      try
      {
        dataContext.Events.DeleteOnSubmit(evnt);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //GetEventRegistrations
    public object GetEventRegistrations(string sidx, string sord, int page, int rows, bool _search, long? evreg_id, long? event_id, long? user_id, string login, byte? usertype)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      List<sp_Event_EventRegistrationsListResult> evreg = dataContext.sp_Event_EventRegistrationsList(evreg_id, event_id, user_id, login, usertype).AsQueryable().OrderBy(sidx + " " + sord).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = evreg.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      evreg = evreg.Skip(pageIndex * pageSize).Take(pageSize).ToList();

      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in evreg
            select new
            {
              i = query.EventRegistrationID,
              cell = new string[] { 
                query.EventRegistrationID.ToString(),
                query.Event,
                query.UserID.ToString(),
                query.Login,                
                query.UserType,
                query.Status,
                query.Email,
                query.DateRegistered.ToString()
              }
            }).ToArray()
      };

      evreg = null;

      return jsonData;
    }

    //AddEvetnRegistration
    public JsonExecuteResult AddEventRegistration(long event_id, long user_id, bool ishb)
    {
      try
      {
        EventRegistration er = new EventRegistration();
        if (!ishb)
        {
          er = dataContext.EventRegistrations.Where(ER => ER.Event_ID == event_id && ER.User_ID == user_id).FirstOrDefault();
          if (er != null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "This user already registered for this event.");
          er = new EventRegistration();
          er.Event_ID = event_id;
          er.User_ID = user_id;
          dataContext.EventRegistrations.InsertOnSubmit(er);
        }
        else
        {
          List<User> users = dataContext.Users.Where(U => U.UserType_ID == (byte)Consts.UserTypes.HouseBidder && U.UserStatus_ID == (byte)Consts.UserStatus.Active).ToList();
          int hbamount = (user_id < 1) ? Consts.HouseBiddersAmountForEventRegstration : (int)user_id;
          if (users.Count() > hbamount)
          {
            int index;
            while (users.Count() > hbamount)
            {
              Random r = new Random();
              index = r.Next(hbamount + 1);
              if (index <= users.Count())
                users.RemoveAt(index);
            }
          }
          foreach (User user in users)
          {
            er = dataContext.EventRegistrations.Where(ER => ER.User_ID == user.ID && ER.Event_ID == event_id).FirstOrDefault();
            if (er != null) continue;
            er = new EventRegistration();
            er.Event_ID = event_id;
            er.User_ID = user.ID;
            dataContext.EventRegistrations.InsertOnSubmit(er);
          }
        }
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //DeleteEventRegistration
    public JsonExecuteResult DeleteEventRegistration(long eventreg_id)
    {
      try
      {
        EventRegistration er = dataContext.EventRegistrations.Where(ER => ER.ID == eventreg_id).SingleOrDefault();
        if (er == null) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The registration doesn't exist");
        dataContext.EventRegistrations.DeleteOnSubmit(er);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
  }
}
