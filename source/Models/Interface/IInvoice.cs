using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IInvoice
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    Int64 User_ID { get; set; }
    DateTime DateCreated { get; set; }
    bool IsPaid { get; set; }
    decimal Amount { get; set; }
    decimal Shipping { get; set; }
    decimal Tax { get; set; }
    Int32 Quantity { get; set; }    
    bool IsSent { get; set; }
    Int64? UserInvoices_ID { get; set; }
    Int64? Consignments_ID { get; set; }
    decimal Cost { get; set; }
    decimal LateFee { get; set; }
    string Comments { get; set; }
    
  }
}
