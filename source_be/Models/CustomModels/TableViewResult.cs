using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class TableViewResult
  {
    public int TotalRecords { get; set; }
    public List<AuctionShort> Records { get; set; }

    public TableViewResult()  
    {
      TotalRecords = 0;
      Records = new List<AuctionShort>();
    }
  }
}