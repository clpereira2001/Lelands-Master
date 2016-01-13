using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Vauction.Utils;

namespace Vauction.Utils.Autorization
{
  public class VauctionPrincipal : IPrincipal
  {
    VauctionIdentity identity;

    public VauctionPrincipal(VauctionIdentity _identity)
    {
      identity = _identity;
    }

    public VauctionIdentity UIdentity
    {
      get { return identity; }
    }

    public IIdentity Identity
    {
      get { return identity; }
    }

    public bool IsInRole(string role)
    {
      //return identity.UserType.HasValue && identity.UserType.Value.ToString() == role;
      return false;
    }

    //public bool IsInRole(Consts.UserTypes ut)
    //{
    //  return identity.UserType.HasValue && identity.UserType.Value == ut;
    //}

    //public bool IsFrontEndUser
    //{
    //  get { return IsInRole(Consts.UserTypes.Buyer) || IsInRole(Consts.UserTypes.SellerBuyer) || IsInRole(Consts.UserTypes.Seller) || IsInRole(Consts.UserTypes.HouseBidder); }
    //}

    //public bool IsBackEndUser
    //{
    //  get { return IsInRole(Consts.UserTypes.Admin) || IsInRole(Consts.UserTypes.Root) || IsInRole(Consts.UserTypes.HouseBidder); }
    //}

    //public bool IsInStatus(Consts.UserStatus us)
    //{
    //  return identity.UserStatus.HasValue && identity.UserStatus.Value == us;
    //}

    public bool IsNeedToCheckStatus(TimeSpan checktime)
    {
      return DateTime.Now.Subtract(identity.LastCheckTime) > checktime;
    }
  }
}