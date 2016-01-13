using System;

namespace Vauction.Models
{
  public interface IMainCategory
  {
    Int32 ID { get; set; }
    string Title { get; set; }
    string Descr { get; set; }
    string Notes { get; set; }
    Int32? CatalogID { get; set; }
    bool IsActive { get; set; }
    Int32 Priority { get; set; }
    Int64 Owner_ID { get; set; }
  }
}
