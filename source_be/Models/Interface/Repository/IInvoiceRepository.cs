using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IInvoiceRepository
  {
    JsonExecuteResult UpdatePayment(long payment_id, byte method, decimal amount, DateTime date, string note, bool iscleared, string cdetails, long id, bool isadding, bool isinvoice);
    JsonExecuteResult UpdateInvoice(long id, decimal amount, decimal shipping, decimal tax, decimal latefee, bool ispaid, bool issent);
    JsonExecuteResult UpdateStatementInvoice(long invoice_id, int commissionrate_id, bool ispaid);
    object GetUserInvoicesBySearch(string sidx, string sord, int page, int rows, long? invoice_id, long? auction_id, long? event_id, long? user_id);
    object GetInvoicesByUserInvoice(string sidx, string sord, int page, int rows, long userinvoice_id);    
    object GetPayments(string sidx, string sord, int page, int rows, long id, bool isuserinvoice);
    object GetConsignorStatementsBySearch(string sidx, string sord, int page, int rows, long? invoice_id, long? auction_id, long? event_id, long? user_id);
    object GetInvoicesByConsignorStatement(string sidx, string sord, int page, int rows, long cons_id);





    // NOT DONE



    Invoice GetInvoice(long invoice_id);
    decimal? GetVariableCoefficient(string Name);
    //void AddInvoicesForAuctions(List<Auction> auctions);
    
    
    bool AddEditPayment(long Payment_Id, byte Method, decimal Amount, DateTime Date, string Note, bool IsCleared, string CDetails, long ID, bool IsAdding, bool IsInvoice);
    
    
    long GetConsignmentStatement(long user_id, long event_id);    
    Consignment GetConsignment(long cons_id);
    JsonExecuteResult RefundingInvoice(long invoice_id);
    JsonExecuteResult RefundingUserInvoice(long userinvoice_id);
    JsonExecuteResult SellLotToBuyer(long auction_id, long user_id);
  }
}
