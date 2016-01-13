using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IAuctionResult
  {
    long ID { get; set; }
    long Auction_ID { get; set; }
    long? User_ID { get; set; }
    decimal? CurrentBid { get; set; }
    decimal? MaxBid { get; set; }
    int? Bids { get; set; }
  }
}