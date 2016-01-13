using System;

namespace Vauction.Models
{
  [Serializable]
  public class IdTitleCount
  {
    public long ID { get; set; }
    public string Title { get; set; }
    public int Count { get; set; }
  }
}