using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models;
using Vauction.Configuration;

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
    public byte UserType { get; set; }
    public byte Status { get; set; }
    public string IP { get; set; }
    
    public bool IsRecievingBidConfirmation { get; set; }
    public bool IsRecievingOutBidNotice { get; set; }
    public bool IsRecievingLotSoldNotice { get; set; }
    public bool IsRecievingLotClosedNotice { get; set; }
    public bool RecieveWeeklySpecials { get; set; }
    public bool RecieveNewsUpdates { get; set; }

    public bool IsSeller { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Seller; } }
    public bool IsSellerBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer; } }
    public bool IsSellerType { get { return IsSeller || IsSellerBuyer; } }
    public bool IsBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Buyer; } }
    public bool IsHouseBidder { get { return (Consts.UserTypes)UserType == Consts.UserTypes.HouseBidder; } }
    public bool IsAdmin { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Admin; } }
    public bool IsSpecialist { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Specialist; } }
    public bool IsWriter { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Writer; } }
    public bool IsRoot { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Root; } }
    public bool IsAdminType { get { return IsAdmin || IsRoot; } }
    public bool IsSpecialistViewer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.SpecialistViewer; } }

    //SessionUser
    public SessionUser()
    {
      ID = 0;
      Login = FirstName = String.Empty;
      UserType = Status = 0;
      IsRecievingBidConfirmation = IsRecievingOutBidNotice = IsRecievingLotSoldNotice = IsRecievingLotClosedNotice = RecieveWeeklySpecials = RecieveNewsUpdates = false;
    }

    //SessionUser
    public SessionUser(long id, string l, string fn, byte type, byte status, bool rbc, bool robn, bool rlsn, bool rlcn, bool rws, bool rnu, string ip)
    {
      ID = id;
      Login = l;
      FirstName = fn;
      UserType = type;
      Status = status;
      IsRecievingBidConfirmation = rbc;
      IsRecievingOutBidNotice = robn;
      IsRecievingLotSoldNotice = rlsn;
      IsRecievingLotClosedNotice = rlcn;
      RecieveWeeklySpecials = rws;
      RecieveNewsUpdates = rnu;
      IP = ip;
    }

    //Create
    public static SessionUser Create(User user)
    {
      return new SessionUser(user.ID, user.Login, user.AddressCard_Billing.FirstName, user.UserType_ID, user.UserStatus_ID, user.IsRecievingBidConfirmation, user.IsRecievingOutBidNotice, user.IsRecievingLotSoldNotice, user.IsRecievingLotClosedNotice, user.RecieveWeeklySpecials.GetValueOrDefault(false), user.RecieveNewsUpdates.GetValueOrDefault(false), user.IP);
    }
  }
  #endregion
  
  #region AppSession
  public class AppSession
  {
    //IsAuthenticated
    public static bool IsAuthenticated
    {
      get { return (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated); }
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
            //User usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserActiveAndApproved(HttpContext.Current.User.Identity.Name);
            //user = SessionUser.Create(usr);
            //HttpContext.Current.Session[SessionKeys.User] = user;
            VauctionPrincipal principal = (HttpContext.Current.User as VauctionPrincipal);
            if (principal != null)
            {
              VauctionIdentity identity = principal.UIdentity;
              User usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserAdministrator(identity.ID, identity.Name);
              if (usr == null)
              {
                IFormsAuthenticationService formsService = new FormsAuthenticationService();
                formsService.SignOut();
              }
              user = SessionUser.Create(usr);
            }
            else
            {
              IFormsAuthenticationService formsService = new FormsAuthenticationService();
              formsService.SignOut();
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
}