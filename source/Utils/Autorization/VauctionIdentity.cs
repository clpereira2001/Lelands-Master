using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Vauction.Configuration;
using Vauction.Models;
using Vauction.Utils;

namespace Vauction.Utils.Autorization
{
  public class VauctionIdentity : IIdentity
  {
    //private User user;
    private bool isAuthenticated;
    //private IUserRepository userRepository;
    private string userName;
    private long userID;
    private DateTime lastCheckTime;
    //private Consts.UserTypes? userType;
    //private string firstName;
    private bool rememberme;

    public VauctionIdentity()
    {
      //isAuthenticated = false;
      //user = null;
      userName = String.Empty;
      //userRepository = ;
      userID = 0;
      //userType = null;
      //firstName = String.Empty;
      rememberme = false;
    }

    public VauctionIdentity(string UserName, string UserData)
      : this()
    {
      userName = UserName;
      string[] str = UserData.Split('|');
      if (str.Length == 3)
      {
        userID = Int64.Parse(str[0]);
        //userType = (Consts.UserTypes)Byte.Parse(str[1]);
        lastCheckTime = Convert.ToDateTime(str[1]);
        //firstName = str[3];
        rememberme = Convert.ToBoolean(str[2]);
      }
      isAuthenticated = userID > 0 && !String.IsNullOrEmpty(userName);
    }

    //private User InitializeUser()
    //{
    //  return (!String.IsNullOrEmpty(userName)) ? userRepository.GetUserActiveAndApproved(userName) : null;
    //}

    public string AuthenticationType
    {
      get { return "Forms"; }
    }

    //public Consts.UserTypes? UserType
    //{
    //  get { return userType; }
    //}

    //public string UserTypeString
    //{
    //  get { return (IsAuthenticated && UserType.HasValue) ? UserType.Value.ToString() : String.Empty; }
    //}

    public bool IsAuthenticated
    {
      get { return isAuthenticated; }
    }

    public string Name
    {
      get { return (IsAuthenticated) ? userName : String.Empty; }
    }

    public long ID
    {
      get { return (IsAuthenticated) ? userID : 0; }
    }

    //public string FirstName
    //{
    //  get { return (isAuthenticated) ? firstName : String.Empty; }
    //}

    //public Consts.UserStatus? UserStatus
    //{
    //  get
    //  {
    //    if (!IsAuthenticated) return null;
    //    if (user == null) user = InitializeUser();        
    //    return user == null ? (Consts.UserStatus?)null : (Consts.UserStatus?) user.Status;
    //  }
    //}

    public DateTime LastCheckTime
    {
      get { return lastCheckTime; }
      set { lastCheckTime = value; }
    }

    //public User User
    //{
    //  get
    //  {
    //    if (!IsAuthenticated) return null;
    //    if (user == null) user = InitializeUser();
    //    return user;
    //  }
    //}

    public bool RememberMe
    {
      get { return rememberme; }
    }

    //public bool IsSellerBuyer
    //{
    //  get { return UserType.HasValue && UserType.Value == Consts.UserTypes.SellerBuyer; }
    //}
    //public bool IsSeller
    //{
    //  get { return UserType.HasValue && UserType.Value == Consts.UserTypes.Seller; }
    //}
    //public bool IsBuyer
    //{
    //  get { return UserType.HasValue && UserType.Value == Consts.UserTypes.Buyer; }
    //}
    //public bool IsSellerType
    //{
    //  get { return IsSeller || IsSellerBuyer; }
    //}
    //public bool IsHouseBidder
    //{
    //  get { return UserType.HasValue && UserType.Value == Consts.UserTypes.HouseBidder; }
    //}
    //public bool IsAdmin
    //{
    //  get { return UserType.HasValue && UserType.Value == Consts.UserTypes.Admin; }
    //}

  }
}