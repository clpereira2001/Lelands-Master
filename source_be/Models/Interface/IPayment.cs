using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IPayment
    {
      Int64 ID { get; set; }
	    Int64 User_ID { get; set; }
	    string Description { get; set; }
	    Int64 Auction_ID { get; set; }
	    Decimal Amount { get; set; }
	    DateTime PostDate { get; set; }
	    Int64 PaymentType_ID { get; set; }
	    DateTime? PaidDate { get; set; }
	    string Notes { get; set; }	    
      Int64 UserInvoices_ID { get; set; }
      Boolean IsCleared { get; set; }
      string ClearedDetails { get; set; }       
    }
}
