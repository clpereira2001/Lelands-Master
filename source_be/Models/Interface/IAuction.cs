using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IAuction
  {
    Int64 ID { get; set; }
    Int64 Category_ID { get; set; }
    byte? Status { get; set; }
    bool IsFeatured { get; set; }
    decimal? Cost { get; set; }
    DateTime? NotifiedOn { get; set; }
    Int16? Lot { get; set; }
    Int64 Owner_ID { get; set; }
    string Title { get; set; }
    string Description { get; set; }    
    decimal Price { get; set; }
    Int32 Quantity { get; set; }
    decimal? Reserve { get; set; }    
    DateTime StartDate { get; set; }
    DateTime EndDate { get; set; }    
    decimal? Shipping { get; set; }
    Int64 Event_ID { get; set; }
    Int64 AuctionType_ID { get; set; }
    bool IsBold { get; set; }
    byte Priority { get; set; }    
    string Addendum { get; set; }
    bool? PulledOut { get; set; }
    string Estimate { get; set; }
    bool IsUnsold { get; set; }

    bool IsCatalog { get; set; }
    int CommissionRate_ID { get; set; }
    bool IsLimitDisabled { get; set; }
    string UrlLotTitle { get; }    
  }
}
