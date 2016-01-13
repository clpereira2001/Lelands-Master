using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class UICInvoice
  {
    public decimal TotalCost { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
  }
}