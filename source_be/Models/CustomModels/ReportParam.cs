using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class ReportParam
  {
    public int Report_ID { get; set; }
    public long? Event_ID { get; set; }
    public long? User_ID { get; set; }
    public int? Status { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DateTo { get; set; }
  }
}