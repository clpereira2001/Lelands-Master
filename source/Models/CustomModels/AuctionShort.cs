using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionShort
  {    
    public short Lot { get; set; }
    public string Title { get; set; }
    public bool IsBold { get; set; }
    public bool IsFeatured { get; set; }
    public string Estimate { get; set; }    
    public int? Bids { get; set; }
    public decimal Price { get; set; }
    public decimal? CurrentBid { get; set; }
    public bool IsUnsoldOrPulledOut { get; set; }
    public string UnsoldOrPulledOut { get; set; }
    public string ThumbnailPath { get; set; }
    public decimal PriceRealized { get; set; }
    public bool PulledOut {get; set;}
    public byte Status { get; set; }
    public DateTime EndDate { get; set; }

    public LinkParams LinkParams { get; set; }

    public bool HasBids
    {
      get { return Bids.GetValueOrDefault(0) > 0 && CurrentBid.GetValueOrDefault(0) > 0; }
    }
  }
}