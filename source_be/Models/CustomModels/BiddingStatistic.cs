using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.CustomClasses
{
  public class BiddingStatistic
  {
    public int WithoutBids { get; set; }
    public int OneBid { get; set; }
    public int MoreThanOneBid { get; set; }
  }
}
