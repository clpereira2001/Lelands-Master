using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class AuctionUpdate
  {
    public short Lot { get; set; }
    public string Title { get; set; }
    public string Addendum { get; set; }
    public bool IsPulledOut { get; set; }
    public LinkParams LinkParams { get; set; }
  }
}