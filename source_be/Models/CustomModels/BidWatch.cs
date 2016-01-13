using System;
using Vauction.Utils;

namespace Vauction.Models
{
  public class UserBidWatch
  {
    public decimal Amount { get; set; }
    public decimal MaxBid { get; set; }
    public string HighBidder { get; set; }    
    public decimal CurrentBid { get; set; }
    public int Bids { get; set; }
    public int? Quantity { get; set; }
    public byte Option { get; set; }

    public LinkParams LinkParams { get; set; }

    public bool HasBid
    {
      get { return Bids > 0 && CurrentBid > 0; }
    }
  }
}