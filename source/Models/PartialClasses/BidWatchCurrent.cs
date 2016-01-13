using System;

namespace Vauction.Models
{
  [Serializable]
  partial class BidWatchCurrent : IBidWatch
  {
    public BidWatchCurrent(BidWatchCurrent bwc)
    {
      Auction_ID = bwc.Auction_ID;
      ID = bwc.ID;
      User_ID = bwc.User_ID;
    }
  }
}
