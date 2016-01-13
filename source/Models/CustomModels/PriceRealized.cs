using System;

namespace Vauction.Models
{
  public class PriceRealized
  {
    public Int64 ID { get; set; }
    public Int16 Lot { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public LinkParams LinkParams { get; set; }
  }
}
