using System;

namespace Vauction.Models
{
  [Serializable]
  public class IdTitleValue : IdTitle
  {
    public decimal Value { get; set; }
  }
}