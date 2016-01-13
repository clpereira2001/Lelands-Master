using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class EndOfAuction
  {
    public string EventTitle { get; set; }
    public string UserName { get; set; }
    
    public List<InvoiceDetail> Invoices { get; set; }
    public List<UserBidWatch> LoserLots { get; set; }

    public decimal TotalCost
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.TotalCost).GetPrice(); }
    }

    public decimal TotalCst
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.Cost).GetPrice(); }
    }

    public decimal TotalTax
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.Tax).GetPrice(); }
    }

    public decimal TotalShipping
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.Shipping).GetPrice(); }
    }

    public string TotalShippingString
    {
      get { return TotalShipping == 0 || Invoices.Where(i => i.Shipping <= 0).Count() > 0 ? "not calculated yet" : TotalShipping.GetCurrency(); }
    }

    public string TotalCostString
    {
      get { return TotalShipping == 0 || Invoices.Where(i => i.Shipping <= 0).Count() > 0 ? "not calculated yet" : TotalCost.GetCurrency(); }
    } 

    public decimal TotalLateFee
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.LateFee).GetPrice(); }
    }

    public EndOfAuction()
    {
      Invoices = new List<InvoiceDetail>();
      LoserLots = new List<UserBidWatch>();
    }
  }
}