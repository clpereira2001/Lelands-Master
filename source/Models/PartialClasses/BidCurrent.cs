using System;

namespace Vauction.Models
{
  [Serializable]
  public partial class BidCurrent : IBid
  {
    public BidCurrent(BidCurrent bc)
    {
      Amount = bc.Amount;
      Auction_ID = bc.Auction_ID;
      DateMade = bc.DateMade;
      ID = bc.ID;
      IP = bc.IP;
      IsActive = bc.IsActive;
      IsProxy = bc.IsProxy;
      MaxBid = bc.MaxBid;
      Quantity = bc.Quantity;
      User_ID = bc.User_ID;
    }
  }
}
