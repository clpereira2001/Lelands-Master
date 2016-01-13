using System;

namespace Vauction.Models
{
  public interface ITag
  {
    Int64 ID { get; set; }
    string Title { get; set; }
    bool IsSystem { get; set; }
    bool IsViewable { get; set; }
  }
}