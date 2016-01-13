using System;

namespace Vauction.Models
{
  public interface IAuctionTag
  {
    long ID { get; set; }
    long AuctionID { get; set; }
    long TagID { get; set; }
  }
}