using System;

namespace Vauction.Models
{
  public interface ICategory
  {
    Int64 ID { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    Int32 Priority { get; set; }
    bool IsActive { get; set; }
    Int64 Owner_ID { get; set; }
  }
}
