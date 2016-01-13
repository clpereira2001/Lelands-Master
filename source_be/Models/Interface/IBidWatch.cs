using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IBidWatch
    {
        Int64 ID { get; set; }
		Int64 Auction_ID { get; set; }
		Int64 User_ID { get; set; }
    }
}
