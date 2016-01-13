using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class CategoryMenuItem
  {
    public long MainCategory_ID { get; set; }
    public long Category_ID { get; set; }
    public long EventCategory_ID { get; set; }
    public int AuctionCount { get; set; }
    public LinkParams LinkParams { get; set; }
  }
}