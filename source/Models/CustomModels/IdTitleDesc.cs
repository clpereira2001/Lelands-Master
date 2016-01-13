using System;

namespace Vauction.Models
{
  [Serializable]
  public class IdTitleDesc : IdTitle
  {
    public string Description { get; set; }
  }
}