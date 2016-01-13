using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IConsignors
    {
        long ID { get; set; }
        string Name { get; set; }
    }
}
