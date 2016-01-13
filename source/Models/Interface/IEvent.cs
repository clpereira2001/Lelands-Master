using System;

namespace Vauction.Models
{
  public interface IEvent
  {
    Int64 ID { get; set; }
    string Title { get; set; }
    DateTime DateStart { get; set; }
    DateTime DateEnd { get; set; }
    decimal? BuyerFee { get; set; }
    string Description { get; set; }
    bool IsViewable { get; set; }
    bool IsClickable { get; set; }
    Int32? Ordinary { get; set; }
    bool IsCurrent { get; set; }
    byte CloseStep { get; set; }
    DateTime LastUpdate { get; set; }
    int Type_ID { get; set; }
  }
}