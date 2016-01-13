using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class AuctionShortDescription
  {
    public long ID { get; set; }
    public long? Lot { get; set; }
    public string Title { get; set; }
    public string CurrentBid { get; set; }
    public string UserType { get; set; }
    public int? BidNumber { get; set; }
    public DateTime? DateMade { get; set; }
    public Auction Auction { get; set; }
  }
}
