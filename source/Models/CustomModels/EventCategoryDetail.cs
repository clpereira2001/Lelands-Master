using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class EventCategoryDetail
  {
    public bool IsCurrent { get; set; }
    public bool IsClickable { get; set; }
    public byte Step { get; set; }
    public DateTime DateEnd { get; set; }
    public LinkParams LinkParams { get; set; }
  }
}