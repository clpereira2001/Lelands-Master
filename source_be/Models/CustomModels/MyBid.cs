using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public class MyBid
  { 
    public decimal Amount { get; set; }
    public decimal MaxBid { get; set; }
    public decimal WinBid { get; set; }
    public DateTime DateMade { get; set; }
    public bool IsUnsold { get; set; }
    public bool IsWinner { get; set; }
    public int BidsCount { get; set; }
    public string ThubnailImage { get; set; }

    public PriceRealized PriceRealized { get; set; }

    public Int64 ID
    {
      get { return (PriceRealized == null) ? 0 : PriceRealized.ID; }
    }
    public Int16 Lot
    {
      get { return (PriceRealized == null) ? (short)0 : PriceRealized.Lot; }
    }
    public string Title
    {
      get { return (PriceRealized == null) ? String.Empty : PriceRealized.Title; }
    }
    public decimal Cost
    {
      get { return (PriceRealized == null) ? 0 : PriceRealized.Price; }
    }
  }
}
