using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IUserReference
    {
        Int64 IDReference { get; set; }
        string AuctionHouse { get; set; }
        string PhoneNumber { get; set; }
        string LastBidPlaced { get; set; }        
    }
}
