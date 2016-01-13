using System;

namespace Vauction.Models
{
  public interface ICategoriesMap
  {
    long ID { get; set; }
    Int32 MainCategory_ID { get; set; }
    Int64 Category_ID { get; set; }
    string Descr { get; set; }
    string Intro { get; set; }    
    Int32 Priority { get; set; }
    bool IsActive { get; set; }
    Int64 Owner_ID { get; set; }
  }
}