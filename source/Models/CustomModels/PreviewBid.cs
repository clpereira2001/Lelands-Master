using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class PreviewBid
  {
    public bool IsProxy { get; set; }
    public decimal Amount { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsOutBid { get; set; }
    public int Quantity { get; set; }
    public decimal RealAmount { get; set; }
    public decimal PreviousAmount { get; set; }
    public decimal PreviousMaxBid { get; set; }
    public LinkParams LinkParams { get; set; }

    public PreviewBid()
    {
      IsUpdate = IsOutBid = false;      
    }
    
    public decimal TotalRealAmount
    {
      get { return RealAmount * Quantity; }
    }
  }
}