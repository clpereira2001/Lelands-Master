using System;
using System.Linq;
using System.Data.Linq;
using Vauction.Utils;
using Vauction.Utils.Lib;
using System.Text;
using QControls.Security;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class UserRepository : IUserRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public UserRepository(VauctionDataContext dataContext, ICacheDataProvider CacheRepository)
    {
      this.dataContext = dataContext;
      this.CacheRepository = CacheRepository;
    }

    //SubmitChanges
    public void SubmitChanges()
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
    #endregion

    #region different
    //ValidatePasswordForUser
    public bool ValidatePasswordForUser(string password, long user_id)
    {
      //return dataContext.Users.Where(p => p.Password == password && p.ID == user_id).Count() > 0;
      return dataContext.spSelect_User(user_id, password, null, null, null, null).FirstOrDefault() != null;
    }

    //ValidateUser
    public User ValidateUser(string login, string password)
    {
      //return dataContext.Users.Where(p => p.Login == login && p.Password == password && p.IsConfirmed && p.Status == (byte)Consts.UserStatus.Active).SingleOrDefault();
      return dataContext.spSelect_User(null, password, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();
    }

    //ValidateLogin
    public bool ValidateLogin(string login, long user_id)
    {
      //return dataContext.Users.Where(U => U.Login == login.Trim() && U.ID != user_id).Count() == 0;
      return dataContext.spUser_Validate(user_id, null, login.Trim()).FirstOrDefault() == null;
    }

    //ValidateEmail
    public bool ValidateEmail(string email, long user_id)
    {
      //return dataContext.Users.Where(u => u.ID != user_id && u.Email.ToLower() == email.Trim().ToLower()).Count() == 0;
      return dataContext.spUser_Validate(user_id, email.Trim(), null).FirstOrDefault() == null;
    }

    //GetUserActiveAndApproved
    public User GetUserActiveAndApproved(string login)
    {
      //return dataContext.Users.Where(p => p.Login == login && p.IsConfirmed && p.Status == (byte)Consts.UserStatus.Active).SingleOrDefault();
      return dataContext.spSelect_User(null, null, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();
    }

    //GetUserActiveAndApproved
    public User GetUserActiveAndApproved(long user_id, string login)
    {
      //return dataContext.Users.Where(p => p.ID == user_id && p.Login == login && p.IsConfirmed && p.Status == (byte)Consts.UserStatus.Active).SingleOrDefault();
      return dataContext.spSelect_User(user_id, null, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();
    }

    //ConfirmUser
    public long ConfirmUser(string confirmCode)
    {
      long user_id = 0;
      try
      {
        User user = GetUserByConfirmationCode(confirmCode);
        if (user == null) throw new Exception("Confirmation code doesn't exist.");
        user.IsConfirmed = true;
        SubmitChanges();
        user_id = user.ID;        
      }
      catch (Exception ex)
      {
        Logger.LogException("[confirmationcode = " + confirmCode + "]", ex);
        user_id = 0;
      }
      finally
      {
        RemoveUserObjectCache(user_id);
      }
      return user_id;
    }

    //UpdateEmailSettings
    public bool UpdateEmailSettings(long user_id, bool IsRecievingWeeklySpecials, bool IsRecievingNewsUpdates, bool IsRecievingBidConfirmation, bool IsRecievingOutBidNotice, bool IsRecievingLotSoldNotice, bool IsRecievingLotClosedNotice)
    {
      try
      {
        User user = GetUser(user_id, false);
        user.RecieveWeeklySpecials = IsRecievingWeeklySpecials;
        user.RecieveNewsUpdates = IsRecievingNewsUpdates;
        user.IsRecievingBidConfirmation = IsRecievingBidConfirmation;
        user.IsRecievingOutBidNotice = IsRecievingOutBidNotice;
        user.IsRecievingLotSoldNotice = IsRecievingLotSoldNotice;
        user.IsRecievingLotClosedNotice = IsRecievingLotClosedNotice;        
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return false;
      }
      finally 
      {
        RemoveUserObjectCache(user_id);
      }
      return true;
    }

    //SetNewUserPassword
    public User SetNewUserPassword(string email)
    {
      User user = GetUserByEmail(email, false);
      if (user == null) return null;
      try
      {
        Guid newCode = Guid.NewGuid();
        PasswordGenerator generator = new PasswordGenerator();
        StringBuilder sb = new StringBuilder(generator.Generate());
        if (sb.Length > 6) sb.Remove(6, sb.Length - 6);
        user.Password = sb.ToString();
        user.ConfirmationCode = newCode.ToString().Replace("-", "");
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return null;
      }      
      return user;
    }

    //UpdatePassword
    public bool UpdatePassword(long user_id, string password)
    {
      User user = GetUser(user_id, false);
      if (user == null) return false;
      try
      {
        user.Password = password;
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        RemoveUserObjectCache(user_id);
        return false;
      }      
      return true;
    }

    //GenerateNewConfirmationCode
    public User GenerateNewConfirmationCode(string email)
    {
      User user = GetUserByEmail(email, false);
      if (user == null) return null;
      try
      {
        Guid newCode = Guid.NewGuid();
        user.ConfirmationCode = newCode.ToString().Replace("-", "");
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        RemoveUserObjectCache(user.ID);
        user = null;
      }      
      return user;
    }

    //TryToUpdateNormalAttempts
    public void TryToUpdateNormalAttempts(User usr)
    {
      try
      {
        if (usr == null) throw new Exception("The user doesn't exist");
        usr.FailedAttempts = 0;
        usr.IP = Consts.UsersIPAddress;
        usr.LastAttempt = DateTime.Now;
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + usr != null ? usr.ID.ToString() : "null" + "]", ex);
      }
    }

    #endregion

    #region get methods
    //GetUserByEmail
    public User GetUserByEmail(string email, bool iscache)
    {
      email = email.ToLower().Trim();
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSERBYEMAIL",
                                                new object[] { email }, CachingExpirationTime.Minutes_30);
      User result = CacheRepository.Get(dco) as User;
      if (result != null && iscache) return result;
      //result = dataContext.Users.Where(U => U.Email == email).SingleOrDefault();
      result = dataContext.spSelect_User(null, null, null, null, null, email).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetUser
    public User GetUser(long user_id, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER",
                                                new object[] { user_id }, CachingExpirationTime.Minutes_10);
      User result = CacheRepository.Get(dco) as User;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_User(user_id, null, null, null, null, null).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetAddressCard
    public AddressCard GetAddressCard(long addresscard_id, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETADDRESSCARD",
                                                new object[] { addresscard_id }, CachingExpirationTime.Minutes_30);
      AddressCard result = CacheRepository.Get(dco) as AddressCard;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_AddressCard(addresscard_id).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetRegisterInfo
    public RegisterInfo GetRegisterInfo(long user_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETREGISTERINFO",
                                                new object[] { user_id }, CachingExpirationTime.Minutes_30);
      RegisterInfo info = CacheRepository.Get(dco) as RegisterInfo;
      if (info != null) return info;
      info = new RegisterInfo();

      IUser user = GetUser(user_id, true);
      if (user == null) return null;

      info.ID = user.ID;
      info.Login = user.Login;
      info.Email = user.Email;
      info.ConfirmEmail = user.Email;
      info.Password = user.Password;
      info.ConfirmPassword = user.Password;
      info.RecieveNewsUpdates = user.RecieveNewsUpdates.GetValueOrDefault(false);
      info.RecieveWeeklySpecials = user.RecieveWeeklySpecials.GetValueOrDefault(false);
      info.Fax = user.Fax;      
      info.BillingLikeShipping = user.BillingLikeShipping;
      info.MobilePhone = user.MobilePhone;
      info.TaxpayerID = user.TaxpayerID;
      info.EbayID = user.EbayID;
      info.DayPhone = user.DayPhone;
      info.EveningPhone = user.EveningPhone;      
      info.EbayFeedback = user.EbayFeedback;
      info.IsModifyed = user.IsModifyed;

      IAddressCard BillingCard = user.Billing_AddressCard_ID != null ? GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true) : new AddressCard();
      info.BillingFirstName = BillingCard.FirstName;
      info.BillingLastName = BillingCard.LastName;
      info.BillingMIName = BillingCard.MiddleName;
      info.BillingAddress1 = BillingCard.Address1;
      info.BillingAddress2 = BillingCard.Address2;
      info.BillingCity = BillingCard.City;
      info.BillingZip = BillingCard.Zip;
      info.BillingState = BillingCard.State;
      info.BillingPhone = BillingCard.HomePhone;
      info.BillingCountry = BillingCard.Country_ID;
      info.BillingCompany = BillingCard.Company;
      info.BillingInternationalState = BillingCard.InternationalState;

      IAddressCard ShippingCard = user.Shipping_AddressCard_ID != null ? GetAddressCard(user.Shipping_AddressCard_ID.GetValueOrDefault(-1), true) : new AddressCard();
      info.ShippingFirstName = ShippingCard.FirstName;
      info.ShippingLastName = ShippingCard.LastName;
      info.ShippingMIName = ShippingCard.MiddleName;
      info.ShippingAddress1 = ShippingCard.Address1;
      info.ShippingAddress2 = ShippingCard.Address2;
      info.ShippingCity = ShippingCard.City;
      info.ShippingZip = ShippingCard.Zip;
      info.ShippingState = ShippingCard.State;
      info.ShippingWorkPhone = ShippingCard.WorkPhone;
      info.ShippingPhone = ShippingCard.HomePhone;
      info.ShippingCountry = ShippingCard.Country_ID;
      info.ShippingInternationalState = ShippingCard.InternationalState;

      IUserReference Reference1 = user.IDUserReference1.HasValue ? GetUserReferences(user.IDUserReference1.GetValueOrDefault(-1)) : new UserReference();
      info.Reference1AuctionHouse = Reference1.AuctionHouse;
      info.Reference1LastBidPlaced = Reference1.LastBidPlaced;
      info.Reference1PhoneNumber = Reference1.PhoneNumber;

      IUserReference Reference2 = user.IDUserReference2.HasValue ? GetUserReferences(user.IDUserReference2.GetValueOrDefault(-1)) : new UserReference();
      info.Reference2AuctionHouse = Reference2.AuctionHouse;
      info.Reference2LastBidPlaced = Reference2.LastBidPlaced;
      info.Reference2PhoneNumber = Reference2.PhoneNumber;

      dco.Data = info;
      CacheRepository.Add(dco);

      return info;
    }

    //GetUserByConfirmationCode
    public User GetUserByConfirmationCode(string code)
    {
      return dataContext.Users.SingleOrDefault(p => p.ConfirmationCode == code.Trim());
    }

    //GetUserReferences
    public UserReference GetUserReferences(long userreference_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSERREFERENCES",
                                                new object[] { userreference_id }, CachingExpirationTime.Minutes_30);
      UserReference result = CacheRepository.Get(dco) as UserReference;
      if (result != null) return result;
      result = dataContext.UserReferences.Where(ur => ur.IDReference == userreference_id).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;      
    }

    //RemoveUserObjectCache
    private void RemoveUserObjectCache(long user_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { user_id }));
    }

    //UpdateUserObjectCache
    private void UpdateUserObjectCache(User user)
    {
      CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { user.ID }, CachingExpirationTime.Minutes_10, user));
    }

    //RemoveUserFromCache
    public void RemoveUserFromCache(long user_id, string email)
    {
      User usr = GetUser(user_id, true);
      RemoveUserObjectCache(user_id);
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETREGISTERINFO", new object[] { user_id });
      CacheRepository.Remove(dco);
      if (usr != null)
      {
        dco.Method = "GETADDRESSCARD";
        dco.Params = new object[] {usr.Billing_AddressCard_ID};
        CacheRepository.Remove(dco);
      }
      if (usr != null && usr.Shipping_AddressCard_ID.HasValue)
      {
        dco.Method = "GETADDRESSCARD";
        dco.Params = new object[] { usr.Shipping_AddressCard_ID.GetValueOrDefault(0) };
        CacheRepository.Remove(dco);
      }
      dco.Method = "GETUSERBYEMAIL";
      dco.Params = new object[] {email};
      CacheRepository.Remove(dco);
    }
    #endregion

    #region add / update user

    //UpdateUser
    public User UpdateUser(RegisterInfo info)
    {
      User usr = GetUser(info.ID, false);
      try
      {
        if (usr == null)
        {
          usr = new User();
          dataContext.Users.InsertOnSubmit(usr);
        }

        usr.Login = info.Login;
        usr.Email = info.Email;
        usr.Password = info.Password;
        usr.RecieveNewsUpdates = info.RecieveNewsUpdates;
        usr.RecieveWeeklySpecials = info.RecieveWeeklySpecials;
        usr.Fax = info.Fax;        
        usr.BillingLikeShipping = info.BillingLikeShipping;
        usr.MobilePhone = info.MobilePhone;
        usr.TaxpayerID = info.TaxpayerID;
        usr.EbayID = info.EbayID;
        usr.EbayFeedback = info.EbayFeedback;
        usr.DayPhone = info.DayPhone;
        usr.EveningPhone = info.EveningPhone;        
        usr.IsModifyed = true;        
        
        State BillingState, ShippingState;
        long state1, state2;

        IDifferentRepository diff = new DifferentRepository(dataContext, CacheRepository);
        BillingState = (!String.IsNullOrEmpty(info.BillingState)) ? diff.GetStateByCode(info.BillingState.ToLower()) : null;
        state1 = (BillingState == null) ? 0 : BillingState.ID;        
        if (!info.BillingLikeShipping)
        {
          ShippingState = (!String.IsNullOrEmpty(info.ShippingState)) ? diff.GetStateByCode(info.ShippingState.ToLower()) : null;
          state2 = (ShippingState == null) ? 0 : ShippingState.ID;
        }
        else state2 = state1;

        IAddressCard ac1 = GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), false);        
        if (ac1 == null)
        {
          ac1 = new AddressCard();
          dataContext.AddressCards.InsertOnSubmit(ac1 as AddressCard);
          usr.AddressCard_Billing = ac1 as AddressCard;
        }
        ac1.FirstName = info.BillingFirstName;
        ac1.LastName = info.BillingLastName;
        ac1.MiddleName = info.BillingMIName;
        ac1.Address1 = info.BillingAddress1;
        ac1.Address2 = info.BillingAddress2;
        ac1.City = info.BillingCity;
        ac1.State = String.IsNullOrEmpty(info.BillingState) ? String.Empty : info.BillingState;
        ac1.Zip = info.BillingZip;
        ac1.Country_ID = info.BillingCountry;
        ac1.Company = info.BillingCompany;
        ac1.InternationalState = info.BillingInternationalState;
        ac1.HomePhone = ac1.WorkPhone = info.BillingPhone;
        ac1.State_ID = state1;

        IAddressCard ac = usr.Shipping_AddressCard_ID.HasValue ? GetAddressCard(usr.Shipping_AddressCard_ID.GetValueOrDefault(-1), false) : GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), false);
        if (ac == null)
        {
          ac = new AddressCard();
          dataContext.AddressCards.InsertOnSubmit(ac as AddressCard);
          usr.AddressCard_Shipping = ac as AddressCard;
        }
        ac.FirstName = info.BillingFirstName;
        ac.LastName = info.BillingLastName;
        ac.MiddleName = info.BillingMIName;
        ac.Address1 = (usr.BillingLikeShipping) ? info.BillingAddress1 : info.ShippingAddress1;
        ac.Address2 = (usr.BillingLikeShipping) ? info.BillingAddress2 : info.ShippingAddress2;
        ac.City = (usr.BillingLikeShipping) ? info.BillingCity : info.ShippingCity;
        ac.State = (usr.BillingLikeShipping) ? info.BillingState : info.ShippingState;
        ac.State = String.IsNullOrEmpty(ac.State)?String.Empty:ac.State;
        ac.InternationalState = (usr.BillingLikeShipping) ? info.BillingInternationalState : info.ShippingInternationalState;
        ac.Zip = (usr.BillingLikeShipping) ? info.BillingZip : info.ShippingZip;
        ac.Country_ID = (usr.BillingLikeShipping) ? info.BillingCountry : info.ShippingCountry;
        ac.HomePhone = ac.WorkPhone = (usr.BillingLikeShipping) ? info.BillingPhone : info.ShippingPhone;
        ac.State_ID = (usr.BillingLikeShipping) ? state1 : state2;

        SubmitChanges();

        RemoveUserFromCache(usr.ID, usr.Email);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        throw ex;
      }
      return usr;
    }

    //AddUser
    public User AddUser(RegisterInfo info)
    {
      User user = new User();
      try
      {
        dataContext.Users.InsertOnSubmit(user);
        user.Login = info.Login;
        user.Email = info.Email;
        user.ConfirmationCode = Guid.NewGuid().ToString().Replace("-", "");
        user.Password = info.Password;
        user.DateRegistered = DateTime.Now;
        user.UserType = (byte)Consts.UserTypes.Buyer;
        user.IsConfirmed = false;        
        user.Fax = info.Fax;        
        user.BillingLikeShipping = info.BillingLikeShipping;
        user.Status = (byte)Consts.UserStatus.Pending;
        user.MobilePhone = info.MobilePhone;
        user.TaxpayerID = info.TaxpayerID;
        user.EbayID = info.EbayID;
        user.EbayFeedback = info.EbayFeedback;        
        user.DayPhone = info.DayPhone;
        user.EveningPhone = info.EveningPhone;
        user.IsModifyed = true;
        user.IsRecievingOutBidNotice = info.RecievingOutBidNotice;
        user.CommissionRate_ID = Consts.DefaultCommissionRate;
        user.RecieveNewsUpdates = true;
        user.RecieveWeeklySpecials = true;
        user.IsRecievingBidConfirmation = true;
        user.IsRecievingLotClosedNotice = true;
        user.IsRecievingLotSoldNotice = true;

        IDifferentRepository diff = new DifferentRepository(dataContext, CacheRepository);
        State BillingState = diff.GetStateByCode(info.BillingState);
        State ShippingState;
        long state1 = (BillingState == null) ? 0 : BillingState.ID; ;
        long state2;
        if (!info.BillingLikeShipping)
        {
          ShippingState = diff.GetStateByCode(info.ShippingState);
          state2 = (ShippingState == null) ? 0 : ShippingState.ID;
        }
        else state2 = state1;

        AddressCard ac = new AddressCard();
        dataContext.AddressCards.InsertOnSubmit(ac);
        ac.FirstName = info.BillingFirstName;
        ac.LastName = info.BillingLastName;
        ac.MiddleName = info.BillingMIName;
        ac.Address1 = info.BillingAddress1;
        ac.Address2 = info.BillingAddress2;
        ac.State = String.IsNullOrEmpty(info.BillingState) ? String.Empty : info.BillingState;
        ac.City = info.BillingCity;
        ac.Zip = info.BillingZip;
        ac.Country_ID = info.BillingCountry;
        ac.Company = info.BillingCompany;
        ac.InternationalState = info.BillingInternationalState;
        ac.HomePhone = ac.WorkPhone = info.BillingPhone;
        ac.State_ID = state1;
        user.AddressCard_Billing = ac;

        ac = new AddressCard();
        dataContext.AddressCards.InsertOnSubmit(ac);
        ac.FirstName = info.BillingFirstName;
        ac.LastName = info.BillingLastName;
        ac.MiddleName = info.BillingMIName;
        ac.Address1 = (user.BillingLikeShipping) ? info.BillingAddress1 : info.ShippingAddress1;
        ac.Address2 = (user.BillingLikeShipping) ? info.BillingAddress2 : info.ShippingAddress2;
        ac.State = (user.BillingLikeShipping) ? info.BillingState : info.ShippingState;
        ac.City = (user.BillingLikeShipping) ? info.BillingCity : info.ShippingCity;
        ac.Zip = (user.BillingLikeShipping) ? info.BillingZip : info.ShippingZip;
        ac.Country_ID = (user.BillingLikeShipping) ? info.BillingCountry : info.ShippingCountry;
        ac.InternationalState = (user.BillingLikeShipping) ? info.BillingInternationalState : info.ShippingInternationalState;
        ac.State_ID = (user.BillingLikeShipping) ? state1 : state2;
        ac.HomePhone = ac.WorkPhone = (user.BillingLikeShipping) ? info.BillingPhone : info.ShippingPhone;
        ac.State_ID = (user.BillingLikeShipping) ? state1 : state2;
        user.AddressCard_Shipping = ac;

        UserReference ur = new UserReference();
        dataContext.UserReferences.InsertOnSubmit(ur);
        ur.AuctionHouse = info.Reference1AuctionHouse;
        ur.PhoneNumber = info.Reference1PhoneNumber;
        ur.LastBidPlaced = info.Reference1LastBidPlaced;
        user.UserReference = ur;

        ur = new UserReference();
        dataContext.UserReferences.InsertOnSubmit(ur);
        ur.AuctionHouse = info.Reference2AuctionHouse;
        ur.PhoneNumber = info.Reference2PhoneNumber;
        ur.LastBidPlaced = info.Reference2LastBidPlaced;
        user.UserReference1 = ur;

        SubmitChanges();
        RemoveUserFromCache(user.ID, user.Email);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return user;
    }

    #endregion

    #region subscribe / unsubscribe
    //AddOuterSubscription
    public void AddOuterSubscription(OuterSubscription os)
    {
      try
      {
        os.IsActive = false;
        dataContext.OuterSubscriptions.InsertOnSubmit(os);
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
    }

    //ActivateOuterSubscription
    public bool ActivateOuterSubscription(long outersubscription_id)
    {
      try
      {
        OuterSubscription os = dataContext.OuterSubscriptions.SingleOrDefault(OS => OS.ID == outersubscription_id);
        if (os == null) return false;
        User user = GetUserByEmail(os.Email, false);
        if (user != null)
        {
          user.RecieveNewsUpdates = os.IsRecievingUpdates;
          user.RecieveWeeklySpecials = os.IsRecievingWeeklySpecials;
        }
        os.IsActive = true;
        SubmitChanges();
        if (user != null) UpdateUserObjectCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return false;
      }
      return true;
    }

    //UnsubscribeFromEmail
    public bool UnsubscribeFromEmail(string email)
    {
      try
      {
        email = email.Trim();
        OuterSubscription sub = dataContext.OuterSubscriptions.Where(S => S.Email == email).SingleOrDefault();
        if (sub == null) return false;
        dataContext.OuterSubscriptions.DeleteOnSubmit(sub);
        SubmitChanges();
      }
      catch
      {
        return false;
      }
      return true;
    }

    //UnsubscribeRegisterUser
    public bool UnsubscribeRegisterUser(string email)
    {
      try
      {
        email = email.Trim();
        User user = GetUserByEmail(email, false);
        if (user == null) return false;// throw new Exception("User doesn't exist");
        user.RecieveNewsUpdates = user.RecieveWeeklySpecials = false;
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch
      {
        return false;
      }
      return true;
    }

    //UnsubscribeFromOuterSubscribtionByID
    public bool UnsubscribeFromOuterSubscribtionByID(long id)
    {
      try
      {
        OuterSubscription os = dataContext.OuterSubscriptions.Where(OS => OS.ID == id).SingleOrDefault();
        if (os == null) return false;// throw new Exception("Outer subscription doesn't exist.");
        dataContext.OuterSubscriptions.DeleteOnSubmit(os);
        dataContext.SubmitChanges();
      }
      catch
      {
        return false;
      }
      return true;
    }

    //SubscribeRegisterUser
    public bool SubscribeRegisterUser(User u)
    {
      try
      {
        if (u == null) throw new Exception("The user doesn't exist");
        OuterSubscription os = dataContext.OuterSubscriptions.SingleOrDefault(OU => OU.Email == u.Email);
        if (os != null)
        {
          os.IsActive = true;
          os.IsRecievingUpdates = u.RecieveNewsUpdates.GetValueOrDefault(false);
          os.IsRecievingWeeklySpecials = u.RecieveWeeklySpecials.GetValueOrDefault(false);
          SubmitChanges();
          return false;
        }
        os = new OuterSubscription();        
        os.Email = u.Email;
        os.EmailConfirm = u.Email;
        os.IsActive = true;        
        os.IsRecievingUpdates = u.RecieveNewsUpdates.GetValueOrDefault(false);
        os.IsRecievingWeeklySpecials = u.RecieveWeeklySpecials.GetValueOrDefault(false);
        os.FirstName = u.AddressCard_Billing.FirstName;
        os.LastName = u.AddressCard_Billing.LastName;
        dataContext.OuterSubscriptions.InsertOnSubmit(os);
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + u != null ? u.ID.ToString() : "null" + "]", ex);
        return false;
      }
      return true;
    }

    //UnsubscribeRegisterUser
    public bool UnsubscribeRegisterUser(long user_id)
    {
      User user = GetUser(user_id, false);
      if (user == null) return false;
      try
      {
        user.RecieveNewsUpdates = user.RecieveWeeklySpecials = false;
        SubmitChanges();
        UpdateUserObjectCache(user);
      }
      catch
      {
        return false;
      }
      return true;
    }
    #endregion
  }
}