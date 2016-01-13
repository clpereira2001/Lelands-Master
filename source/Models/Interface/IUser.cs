using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Validation;

namespace Vauction.Models
{  
	public interface IUser
	{
		Int64 ID { get; set; }

    [FieldTitle("User ID")]
    [FieldRequired]
    [FieldCheckMaxLength(20)]
    string Login { get; set; }

    string Password { get; set; }
    string ConfirmationCode { get; set; }
    bool IsConfirmed { get; set; }
    byte Status { get; set; }
    byte UserType { get; set; }

    [FieldRequired]
    string Email { get; set; }

    string Fax { get; set; }    
    DateTime DateRegistered { get; set; }    
    string IP { get; set; }    
    byte? FailedAttempts { get; set; }
    DateTime? LastAttempt { get; set; }        
    bool? RecieveWeeklySpecials { get; set; }
    bool? RecieveNewsUpdates { get; set; }
    Int64? Shipping_AddressCard_ID { get; set; }
    Int64? Billing_AddressCard_ID { get; set; }
    bool BillingLikeShipping { get; set; }
    string EbayID { get; set; }
    string EbayFeedback { get; set; }

    string DayPhone { get; set; }
    string EveningPhone { get; set; }
    string Notes { get; set; }
    string MobilePhone { get; set; }
    string TaxpayerID { get; set; }
    bool IsModifyed { get; set; }
    DateTime? LastUpdate { get; set; }
    Int32? CommissionRate_ID { get; set; }

    Int64? IDUserReference1 { get; set; }
    Int64? IDUserReference2 { get; set; }        

    bool IsRecievingBidConfirmation { get; set;}
    bool IsRecievingOutBidNotice  { get; set;}
    bool IsRecievingLotSoldNotice  { get; set;}
    bool IsRecievingLotClosedNotice { get; set; }
    bool IsCatalog { get; set; }
    bool IsPostCards { get; set; }
    
    bool IsSellerBuyer { get; }
    bool IsSellerType { get; }
    bool IsSeller { get; }
    bool IsBuyer { get; }
    bool IsHouseBidder { get; }
    bool IsAdmin { get; }
    bool IsRoot { get; }
    bool IsAdminType { get;}
	}
}
