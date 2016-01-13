using System;

namespace Vauction.Models
{
  public class InvoiceDetail
  {
    public long UserInvoice_ID { get; set; }
    public long Invoice_ID { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Shipping { get; set; }
    public decimal Tax { get; set; }
    public decimal LateFee { get; set; }
    public Int64 Auction_ID { get; set; }
    public Int16 Lot { get; set; }
    public string Title { get; set; }
    public decimal Cost { get; set; }
    public LinkParams LinkParams { get; set; }

    public decimal TotalCost
    {
      get { return Cost + Shipping + Tax + LateFee; }
    }
  }
}