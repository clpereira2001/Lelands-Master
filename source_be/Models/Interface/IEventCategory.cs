using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IEventCategory
  {
    Int64 ID { get; set; }    
    Int64 Event_ID { get; set; }
    Int32 MainCategory_ID { get; set; }
    Int64 Category_ID { get; set; }
    string Descr { get; set; }        
    Int32 Priority { get; set; }
    Int64 Owner_ID { get; set; }
    bool IsActive { get; set; }
  }
}
