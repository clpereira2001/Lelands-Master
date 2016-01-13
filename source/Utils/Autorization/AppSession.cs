using System;
using System.Web;
using Vauction.Models;
using Vauction.Configuration;
using Vauction.Utils.Perfomance;

namespace Vauction.Utils.Autorization
{
  #region SessionKeys
  public class SessionKeys
  {
    public const string User = "CurrentUser";
  }
  #endregion

  #region SessionUser
  [Serializable]
  public class SessionUser
  {
    public long ID { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte UserType { get; set; }
    public byte Status { get; set; }
    public int CommRate_ID { get; set; }
    public string IP { get; set; }

    public Address Address_Billing { get; set; }
    public Address Address_Shipping { get; set; }
    public bool IsAddressBillingLikeShipping { get; set; }

    public bool IsRecievingBidConfirmation { get; set; }
    public bool IsRecievingOutBidNotice { get; set; }
    public bool IsRecievingLotSoldNotice { get; set; }
    public bool IsRecievingLotClosedNotice { get; set; }
    public bool IsRecieveWeeklySpecials { get; set; }
    public bool IsRecieveNewsUpdates { get; set; }

    public bool IsSeller { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Seller; } }
    public bool IsSellerBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer; } }
    public bool IsSellerType { get { return IsSeller || IsSellerBuyer; } }
    public bool IsBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Buyer; } }
    public bool IsBuyerType { get { return IsBuyer || IsSellerBuyer; } }
    public bool IsHouseBidder { get { return (Consts.UserTypes)UserType == Consts.UserTypes.HouseBidder; } }
    public bool IsBidder { get { return IsBuyerType || IsHouseBidder; } }
    public bool IsAdmin { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Admin; } }
    public bool IsRoot { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Root; } }
    public bool IsAdminType { get { return IsAdmin || IsRoot; } }
    public bool IsBackendUser { get { return IsAdmin || IsRoot || IsHouseBidder; } }

    //SessionUser
    public SessionUser()
    {
      ID = CommRate_ID = 0;
      Login = FirstName = Email = LastName = String.Empty;
      UserType = Status = 0;
      IsRecievingBidConfirmation = IsRecievingOutBidNotice = IsRecievingLotSoldNotice = IsRecievingLotClosedNotice = IsRecieveWeeklySpecials = IsRecieveNewsUpdates = false;
    }

    //SessionUser
    public SessionUser(long id, string l, string fn, string ln, byte type, byte status, bool rbc, bool robn, bool rlsn, bool rlcn, bool rws, bool rnu, int commrate_id, string email, bool isbillinglikeshipping, AddressCard b, AddressCard s, string ip)
    {
      ID = id;
      Login = l;
      FirstName = fn;
      LastName = ln;
      UserType = type;
      Status = status;
      IsRecievingBidConfirmation = rbc;
      IsRecievingOutBidNotice = robn;
      IsRecievingLotSoldNotice = rlsn;
      IsRecievingLotClosedNotice = rlcn;
      IsRecieveWeeklySpecials = rws;
      IsRecieveNewsUpdates = rnu;
      CommRate_ID = commrate_id;
      Email = email;
      IsAddressBillingLikeShipping = isbillinglikeshipping;
      Address_Billing = new Address { Address_1 = b.Address1, Address_2 = b.Address2, City = b.City, Country_ID = b.Country_ID, State_ID = b.State_ID, Zip = b.Zip, Country = ProjectConfig.Config.DataProvider.GetInstance().DifferentRepository.GetCountry(b.Country_ID).Title, State = b.State, HomePhone = b.HomePhone, WorkPhone = b.WorkPhone, FirstName = b.FirstName, LastName = b.LastName, MiddleName = b.MiddleName };
      Address_Shipping = new Address { Address_1 = s.Address1, Address_2 = s.Address2, City = s.City, Country_ID = s.Country_ID, State_ID = s.State_ID, Zip = s.Zip, Country = ProjectConfig.Config.DataProvider.GetInstance().DifferentRepository.GetCountry(s.Country_ID).Title, State = s.State, HomePhone = s.HomePhone, WorkPhone = s.WorkPhone, FirstName = s.FirstName, LastName = s.LastName, MiddleName = s.MiddleName };
      IP = ip;
    }

    //Create
    public static SessionUser Create(IUser user)
    {
      AddressCard ac = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
      AddressCard ac2 = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetAddressCard(user.BillingLikeShipping || !user.Shipping_AddressCard_ID.HasValue ? user.Billing_AddressCard_ID.GetValueOrDefault(-1) : user.Shipping_AddressCard_ID.GetValueOrDefault(-1), true);
      return new SessionUser(user.ID, user.Login, ac.FirstName, ac.LastName, user.UserType, user.Status, user.IsRecievingBidConfirmation, user.IsRecievingOutBidNotice, user.IsRecievingLotSoldNotice, user.IsRecievingLotClosedNotice, user.RecieveWeeklySpecials.GetValueOrDefault(false), user.RecieveNewsUpdates.GetValueOrDefault(false), user.CommissionRate_ID.GetValueOrDefault(0), user.Email, user.BillingLikeShipping, ac, ac2, user.IP);
    }
  }
  #endregion

  #region AppSession
  public class AppSession
  {
    //IsAuthenticated
    public static bool IsAuthenticated
    {
      get { return (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated); }
    }

    //CurrentUser
    public static SessionUser CurrentUser
    {
      get
      {
        try
        {
          if (!IsAuthenticated) return null;
          SessionUser user = HttpContext.Current.Session[SessionKeys.User] as SessionUser;
          if (user == null)
          {
            VauctionPrincipal principal = (HttpContext.Current.User as VauctionPrincipal);
            if (principal != null)
            {
              VauctionIdentity identity = principal.UIdentity;
              User usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserActiveAndApproved(identity.ID, identity.Name);
              user = (usr != null) ? SessionUser.Create(usr) : null;
            }
            HttpContext.Current.Session[SessionKeys.User] = user;
          }
          return user;
        }
        catch
        {
          return null;
        }
      }
      set
      {
        HttpContext.Current.Session[SessionKeys.User] = value;
      }
    }
  }
  #endregion

  #region AppApplication
  public class AppApplication
  {
    public static string CacheProviderKey;
    
    static AppApplication()
    {
      CacheProviderKey = "CacheProvider";
    }

    public static ICacheDataProvider CacheProvider
    {
      get
      {
        if (HttpContext.Current.Application[CacheProviderKey] != null)
          return (ICacheDataProvider)HttpContext.Current.Application[CacheProviderKey];
        ICacheDataProvider cacheDataProvider;
        if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT) cacheDataProvider = new CacheDataProvider();
        else
        {
          try
          {
            cacheDataProvider = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
          }
          catch
          {
            cacheDataProvider = new CacheDataProvider();
          }
        }
        return cacheDataProvider;
      }
      set { HttpContext.Current.Application[CacheProviderKey] = value; }
    }
  }
  #endregion
}