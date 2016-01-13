using System;

namespace Vauction.Models
{
  public interface ICommissionRate
  {
    Int32 RateID { get; set; }
    String RateCode { get; set; }
    String Description { get; set; }
    Decimal? Highbid { get; set; }
    Decimal? Reserve { get; set; }
    Decimal? Overage { get; set; }
    Decimal MaxPercent { get; set; }
    Decimal MinPercent { get; set; }
    Decimal Highbid2 { get; set; }
    Decimal MiddlePercent { get; set; }
    String LongDescription { get; set; }
  }
}
