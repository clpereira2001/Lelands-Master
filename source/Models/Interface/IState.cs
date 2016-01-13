using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IState
    {
      Int64 ID { get; set; }
      string Title { get; set; }
      string Code { get; set; }
      Int64 Country_ID { get; set; }
    }
}
