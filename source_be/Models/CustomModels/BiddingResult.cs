using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  public class BiddingResult
  {
    private decimal Price;
    public BidCurrent WinningBid { get; set; }
    public BidCurrent UsersTopBid { get; set; }

    public decimal MinBid
    {
      get
      {
        decimal mb = WinningBid == null ? Price : WinningBid.Amount; // - Consts.GetIncrement(Price)
        if (UsersTopBid != null && mb < UsersTopBid.MaxBid && !UsersTopBid.IsProxy)
          mb = UsersTopBid.MaxBid;        
        return mb;
      }
    }

    public decimal Increment
    {
      get { return Consts.GetIncrement((WinningBid == null) ? Price : MinBid); }
    }

    public BiddingResult(decimal price)
    {
      Price = price;
    }
  }
}