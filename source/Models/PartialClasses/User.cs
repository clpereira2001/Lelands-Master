using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  partial class User : IUser
  {
    public User(Int64 id)
    {
      ID = id;
    }
        
    public bool IsSeller {get{ return (Consts.UserTypes)UserType == Consts.UserTypes.Seller; }}
    public bool IsSellerBuyer {get{ return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer; }}
    public bool IsSellerType {get{ return IsSeller || IsSellerBuyer; }}
    public bool IsBuyer {get{ return (Consts.UserTypes)UserType == Consts.UserTypes.Buyer; }}
    public bool IsHouseBidder {get{ return (Consts.UserTypes)UserType == Consts.UserTypes.HouseBidder; }}
    public bool IsAdmin {get{ return (Consts.UserTypes)UserType == Consts.UserTypes.Admin; }}
    public bool IsRoot { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Root;} }
    public bool IsAdminType { get { return IsAdmin || IsRoot;} }
  }
}
