using System;

namespace Vauction.Models
{
  public interface IBidLog
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    Int64 User_ID { get; set; }
    Decimal Amount { get; set; }
    Decimal MaxBid { get; set; }
    Int32 Quantity { get; set; }
    DateTime DateMade { get; set; }
    string IP { get; set; }        
    bool IsProxy { get; set; }    
    bool IsProxyRaise { get; set; }
    bool IsAutoBid { get; set; }
  }
}
