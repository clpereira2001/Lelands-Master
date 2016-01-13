using System;
using System.Data;
using System.Data.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Transactions;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public class InvoiceRepository : IInvoiceRepository
  {
    private VauctionDataContext dataContext;

    public InvoiceRepository(VauctionDataContext dataContext)
    {
      this.dataContext = dataContext;
    }

    #region buyer invoices
    //GetUserInvoicesBySearch
    public object GetUserInvoicesBySearch(string sidx, string sord, int page, int rows, long? invoice_id, long? auction_id, long? event_id, long? user_id)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      dataContext.CommandTimeout = 600000;
      var res = dataContext.spInvoice_View_UserInvoicesBE(invoice_id, auction_id, event_id, user_id, pageindex, rows, ref totalrecords);
      if (totalrecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        rows = (
            from query in res
            select new
            {
              i = query.UserInvoice_ID,
              cell = new string[] {
                query.UserInvoice_ID.ToString(),
                query.EventTitle,
                query.FLName,
                query.Cost.GetCurrency(false),
                query.Shipping.GetCurrency(false),
                query.Tax.GetCurrency(false),
                query.LateFee.GetCurrency(false),
                query.TotalCost.GetCurrency(false),
                query.APaid.GetCurrency(false),
                query.ADue.GetCurrency(false)
              }
            }).ToArray()
      };
    }

    //GetInvoicesByUserInvoice
    public object GetInvoicesByUserInvoice(string sidx, string sord, int page, int rows, long userinvoice_id)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      decimal? cost, shipping, tax, latefee, totalcost, balance;
      cost = shipping = tax = latefee = totalcost = balance = 0;
      dataContext.CommandTimeout = 600000;      
      var res = dataContext.spInvoice_View_InvoicesByUI(userinvoice_id, ref cost, ref shipping, ref tax,ref totalcost, ref latefee, ref balance, pageindex, rows, ref totalrecords);
      var userdt = new { Date = "Totals:", Amount = cost.GetCurrency(false), Shipping = shipping.GetCurrency(false), SalesTax = tax.GetCurrency(false), LateFees = latefee.GetCurrency(false), TCost = totalcost.GetCurrency(false), Balance = balance.GetCurrency(false) };
      if (totalrecords.GetValueOrDefault(0) == 0)
        return new { total = 0, page = page, records = 0, userdata = (userdt) };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        userdata = (userdt),
        rows = (
            from query in res
            select new
            {
              i = query.Invoice_ID,
              cell = new string[] { 
                query.Invoice_ID.ToString(),
                query.Lot.HasValue?query.Lot.Value.ToString():String.Empty,
                query.Title,
                query.CreatedDate.GetValueOrDefault(DateTime.Now).ToShortDateString(),
                query.Cost.GetPrice().ToString(),
                query.Shipping.GetPrice().ToString(),
                query.Tax.GetPrice().ToString(),
                query.LateFee.GetPrice().ToString(),
                query.TotalCost.GetPrice().ToString(),
                query.IsPaid.ToString(),
                query.IsSent.ToString()
              }
            }).ToArray()
      };

      //List<Invoice> invoices = new List<Invoice>(from I in dataContext.Invoices
      //                                           where I.UserInvoices_ID == userinvoice_id
      //                                           select I);

      //int pageIndex = Convert.ToInt32(page) - 1;
      //int pageSize = rows;
      //int totalRecords = invoices.Count();
      //int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      //decimal cost = invoices.Sum(I => I.Cost);
      //decimal ship = invoices.Sum(I => I.Shipping);
      //decimal tax = invoices.Sum(I => I.Tax);
      //decimal fee = invoices.Sum(I => I.LateFee);

      //List<Invoice> inv = new List<Invoice>(invoices.Where(a => a.IsPaid));
      //decimal ispaid = inv.Sum(I => I.Cost + I.Shipping + I.Tax + I.LateFee);

      //invoices = new List<Invoice>(invoices.Skip(pageIndex * pageSize).Take(pageSize));

      //var p = from P in dataContext.Payments
      //        where P.UserInvoices_ID == userinvoice_id
      //        select P;
      //ispaid = ((p.Count() > 0) ? p.Sum(s => s.Amount) : 0) - ispaid;
      //p = null;

      //var jsonData = new
      //{
      //  total = totalPages,
      //  page = page,
      //  records = totalRecords,
      //  userdata = (new { Date = "Totals:", Amount = cost.GetCurrency(false), Shipping = ship.GetCurrency(false), SalesTax = tax.GetCurrency(false), LateFees = fee.GetCurrency(false), TCost = (cost + ship + tax + fee).GetCurrency(false), Balance = ispaid.GetCurrency(false) }),
      //  rows = (
      //      from query in invoices
      //      select new
      //      {
      //        i = query.ID,
      //        cell = new string[] { 
      //          query.ID.ToString(),
      //          query.Auction.Lot.ToString(),
      //          query.Auction.Title,
      //          query.DateCreated.ToShortDateString(),
      //          query.Cost.GetPrice().ToString(),
      //          query.Shipping.GetPrice().ToString(),
      //          query.Tax.GetPrice().ToString(),
      //          query.LateFee.GetPrice().ToString(),
      //          query.TotalCost.GetPrice().ToString(),
      //          query.IsPaid.ToString(),
      //          query.IsSent.ToString()
      //        }
      //      }).ToArray()
      //};

      //invoices = null;

      //return jsonData;
    }
    #endregion

    #region consignor statements
    //GetConsignorStatementsBySearch
    public object GetConsignorStatementsBySearch(string sidx, string sord, int page, int rows, long? invoice_id, long? auction_id, long? event_id, long? user_id)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      dataContext.CommandTimeout = 600000;
      var res = dataContext.spInvoice_View_ConsignorStatements(invoice_id, auction_id, event_id, user_id, pageindex, rows, ref totalrecords);
      if (totalrecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        rows = (
            from query in res
            select new
            {
              i = query.Consignment_ID,
              cell = new string[] {                
                query.Consignment_ID.ToString(),
                query.EventTitle,
                query.FLName,
                query.Pending.GetCurrency(false),                
                query.TotalCost.GetCurrency(false),
                query.APaid.GetCurrency(false),
                query.ADue.GetCurrency(false),
                query.Owner_ID.GetValueOrDefault(0).ToString()
              }
            }).ToArray()
      };
    }
    //GetInvoicesByStatement
    public object GetInvoicesByConsignorStatement(string sidx, string sord, int page, int rows, long cons_id)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      decimal? amount, fee, cost, balance;
      amount = cost = fee = balance = 0;
      dataContext.CommandTimeout = 600000;      
      var res = dataContext.spInvoice_View_InvoicesByCS(cons_id, ref amount, ref fee, ref cost, ref balance, pageindex, rows, ref totalrecords);
      var userdt = new { Status = "Totals:", Amount = amount.GetCurrency(false), Fee = fee.GetCurrency(false), TCost = cost.GetCurrency(false), Balance = balance.GetCurrency(false) };
      if (totalrecords.GetValueOrDefault(0) == 0)
        return new { total = 0, page = page, records = 0, userdata = (userdt) };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        userdata = (userdt),
        rows = (
            from query in res
            select new
            {
              i = query.Invoice_ID,
              cell = new string[] { 
                query.Invoice_ID.ToString(),
                query.Lot.ToString(),
                query.Title,
                query.CreatedDate.GetValueOrDefault(DateTime.Now).ToShortDateString(),
                query.PaymentStatus,
                query.Amount.GetPrice().ToString(),
                query.CommissionRate,
                query.Fee.GetPrice().ToString(),
                query.Cost.GetPrice().ToString(),
                query.IsPaid.ToString()
              }
            }).ToArray()
      };   
    }


    #endregion

    #region payment methods
    //GetPayments
    public object GetPayments(string sidx, string sord, int page, int rows, long id, bool isuserinvoice)
    {
      int? totalrecords = 0;
      int pageindex = (page > 0) ? page - 1 : 0;
      decimal? totalcost = 0;
      dataContext.CommandTimeout = 600000;      
      var res = dataContext.spInvoice_View_Payments(id, isuserinvoice, ref totalcost, pageindex, rows, ref totalrecords);
      var userdt = new { Method = "Totals:", Amount = totalcost.GetCurrency(false) };
      if (totalrecords.GetValueOrDefault(0) == 0)
        return new { total = 0, page = page, records = 0, userdata = (userdt) };
      return new
      {
        total = (int)Math.Ceiling((float)totalrecords.GetValueOrDefault(0) / (float)rows),
        page = page,
        records = totalrecords.GetValueOrDefault(0),
        userdata = (userdt),
        rows = (
            from query in res
            select new
            {
              i = query.Payment_ID,
              cell = new string[] { 
                query.Payment_ID.ToString(),
                query.PaymentType,
                query.Amount.GetPrice().ToString(),
                query.PaidDate.Value.ToShortDateString(),
                query.Notes,
                query.IsCleared.ToString(),
                query.ClearedDetails
              }
            }).ToArray()
      };
    }

    //UpdatePayment
    public JsonExecuteResult UpdatePayment(long payment_id, byte method, decimal amount, DateTime date, string note, bool iscleared, string cdetails, long id, bool isadding, bool isinvoice)
    {
      try
      {
        Payment payment = (isadding) ? new Payment() : dataContext.Payments.SingleOrDefault(P => P.ID == payment_id);
        if (payment == null || id < 1) throw new Exception(isinvoice ? "The buyer invoice doesn't exist" : "The consignor statement doesn't exist");
        PaymentType pt = dataContext.PaymentTypes.SingleOrDefault(PT => PT.ID == method);
        payment.PaymentType = pt;
        payment.Amount = amount;
        payment.PaidDate = date;
        payment.Notes = note;
        payment.IsCleared = iscleared;
        payment.ClearedDetails = cdetails;
        if (isinvoice)
        {
          payment.UserInvoices_ID = id;
          UserInvoice ui = dataContext.UserInvoices.SingleOrDefault(UI => UI.ID == id);
          payment.User_ID = (ui != null) ? ui.User_ID : 0;
        }
        else
        {
          payment.Consignments_ID = id;
          Consignment ui = dataContext.Consignments.SingleOrDefault(UI => UI.ID == id);
          payment.User_ID = (ui != null) ? ui.User_ID : 0;
        }
        payment.PostDate = DateTime.Now;
        if (isadding) dataContext.Payments.InsertOnSubmit(payment);
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
    #endregion

    #region invoice methods

    //UpdateInvoice
    public JsonExecuteResult UpdateInvoice(long id, decimal amount, decimal shipping, decimal tax, decimal latefee, bool ispaid, bool issent)
    {
      try
      {        
        Invoice invoice = GetInvoice(id);
        if (invoice == null) throw new Exception("The invoice doesn't exist.");
        //Variable v = dataContext.Variables.Where(V=>V.Name=="SalesTaxRate").FirstOrDefault();        
        invoice.Cost = amount;
        invoice.Shipping = shipping;
        invoice.Tax = tax; //(String.IsNullOrEmpty(invoice.User.TaxpayerID) && invoice.User.AddressCard_Billing.State_ID==40)? ((amount+shipping)*v.Value).GetPrice() : 0;
        invoice.LateFee = latefee;
        invoice.IsPaid = ispaid;
        invoice.IsSent = issent;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //UpdateStatementInvoice
    public JsonExecuteResult UpdateStatementInvoice(long invoice_id, int commissionrate_id, bool ispaid)
    {
      try
      {
        Invoice invoice = GetInvoice(invoice_id);
        if (invoice == null) throw new Exception("The invoice doesn't exist.");
        CommissionRate cr = dataContext.CommissionRates.SingleOrDefault(CR => CR.RateID == commissionrate_id);
        if (cr == null) throw new Exception("The commission rate doesn't exist.");

        Auction auction = dataContext.Auctions.SingleOrDefault(q => q.ID == invoice.Auction_ID);
        if (auction == null) throw new Exception("The lot doesn't exist.");
        auction.CommissionRate_ID = cr.RateID;
        GeneralRepository.SubmitChanges(dataContext);

        invoice.Cost = invoice.Amount - dataContext.GetComissionForItem(invoice.Auction_ID, invoice.Amount).GetPrice();
        invoice.IsPaid = ispaid;
        invoice.Shipping = invoice.Tax = invoice.LateFee = 0;
        GeneralRepository.SubmitChanges(dataContext);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }


    #endregion




    // NOT DONE



    //SubmitChanges
    private bool SubmitChanges()
    {
      bool res = true;
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Vauction.Utils.Lib.Logger.LogWarning(e.Message);
        try
        {
          foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
          {
            occ.Resolve(RefreshMode.KeepCurrentValues);
          }
          res = true;
        }
        catch
        {
          res = false;
        }
      }
      return res;
    }

    //GetInvoice
    public Invoice GetInvoice(long invoice_id)
    {
      return dataContext.Invoices.SingleOrDefault(I => I.ID == invoice_id);
    }

    //GetVariableCoefficient
    public decimal? GetVariableCoefficient(string Name)
    {
      return (from I in dataContext.Variables where I.Name.Equals(Name) select I.Value).FirstOrDefault();
    }

    //AddInvoicesForAuctions
    //public void AddInvoicesForAuctions(List<Auction> auctions)
    //{
    //  decimal? Insurance = GetVariableCoefficient("InsuranceCoefficient");
    //  Insurance = (Insurance.HasValue) ? Insurance.Value : (decimal)0.4;

    //  //decimal? SalesTaxRate = GetVariableCoefficient("SalesTaxRate");
    //  decimal SalesTaxRate = (decimal)0.08625;//(SalesTaxRate.HasValue) ? SalesTaxRate.Value : (decimal)0.08625;

    //  List<Invoice> invoices = new List<Invoice>(auctions.Count);
    //  Invoice invoice;
    //  Bid winningBid;
    //  foreach (Auction auc in auctions)
    //  {
    //    winningBid = auc.WinningBid as Bid;
    //    if (winningBid == null || (auc.Reserve.HasValue && auc.Reserve.Value > winningBid.Amount) || (winningBid.User.UserType_ID == (byte)Consts.UserTypes.HouseBidder)) continue;
    //    invoice = new Invoice();
    //    invoice.Auction_ID = auc.ID;
    //    invoice.Amount = winningBid.Amount;
    //    invoice.Cost = winningBid.Amount * (1 + auc.Event.BuyerFee.Value / 100);
    //    invoice.Comments = "Regular auction won item";
    //    invoice.DateCreated = DateTime.Now;
    //    invoice.IsPaid = false;
    //    invoice.IsSent = false;
    //    invoice.Quantity = 1;
    //    invoice.Shipping = (auc.Shipping.HasValue) ? auc.Shipping.Value : 0;
    //    invoice.Tax = (winningBid.User.AddressCard_Billing.State.Equals("NY") && String.IsNullOrEmpty(winningBid.User.TaxpayerID)) ? invoice.Amount * SalesTaxRate : 0;
    //    invoice.User_ID = winningBid.User_ID;
    //    invoices.Add(invoice);
    //  }
    //  dataContext.Invoices.InsertAllOnSubmit(invoices);
    //  SubmitChanges();
    //}

    

    

    

    

    //AddEditPayment
    public bool AddEditPayment(long Payment_Id, byte Method, decimal Amount, DateTime Date, string Note, bool IsCleared, string CDetails, long ID, bool IsAdding, bool IsInvoice)
    {
      Payment payment = (IsAdding) ? new Payment() : dataContext.Payments.SingleOrDefault(P => P.ID == Payment_Id);
      if (payment == null || ID < 1) return false;
      PaymentType pt = dataContext.PaymentTypes.SingleOrDefault(PT => PT.ID == Method);
      payment.PaymentType = pt;
      payment.Amount = Amount;
      payment.PaidDate = Date;
      payment.Notes = Note;
      payment.IsCleared = IsCleared;
      payment.ClearedDetails = CDetails;
      if (IsInvoice)
      {
        payment.UserInvoices_ID = ID;
        UserInvoice ui = dataContext.UserInvoices.SingleOrDefault(UI => UI.ID == ID);
        payment.User_ID = (ui != null) ? ui.User_ID : 0;
      }
      else
      {
        payment.Consignments_ID = ID;
        Consignment ui = dataContext.Consignments.SingleOrDefault(UI => UI.ID == ID);
        payment.User_ID = (ui != null) ? ui.User_ID : 0;
      }
      payment.PostDate = DateTime.Now;
      if (IsAdding) dataContext.Payments.InsertOnSubmit(payment);
      return SubmitChanges();
    }

    
    //GetConsignmentStatetion
    public long GetConsignmentStatement(long user_id, long event_id)
    {
      Consignment cons = dataContext.Consignments.SingleOrDefault(C => C.User_ID == user_id && C.Event_ID == event_id);
      return (cons == null) ? -1 : cons.ID;
    }

    //GetConsignment
    public Consignment GetConsignment(long cons_id)
    {
      return dataContext.Consignments.SingleOrDefault(C => C.ID == cons_id);
    }

    //RefundingInvoice
    public JsonExecuteResult RefundingInvoice(long invoice_id)
    {
      try
      {
        Invoice invoice = GetInvoice(invoice_id);
        if (invoice == null) throw new Exception("You can't refund this invoice");
        if (dataContext.Invoices.Where(I => I.UserInvoices_ID == invoice.UserInvoices_ID).Count() == 1)
          return RefundingUserInvoice(invoice.UserInvoices_ID.GetValueOrDefault(-1));
        dataContext.spInvoice_Refund(invoice_id);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //RefundingUserInvoice
    public JsonExecuteResult RefundingUserInvoice(long userinvoice_id)
    {
      try
      {
        dataContext.spInvoice_RefundUserInvoice(userinvoice_id);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //SellLotToBuyer
    public JsonExecuteResult SellLotToBuyer(long auction_id, long user_id)
    {
      try
      {
        Auction auction = dataContext.Auctions.Where(A => A.ID == auction_id).FirstOrDefault();
        if (auction == null) throw new Exception("The lot doesn't exist.");
        if (auction.Status != (byte)Consts.AuctionStatus.Closed) throw new Exception("You can sell only the items with the 'Closed' status.");
        User user = dataContext.Users.Where(U => U.ID == user_id).FirstOrDefault();
        if (user == null) throw new Exception("The user doesn't exist.");
        if (user.UserStatus_ID != (byte)Consts.UserStatus.Active || !user.IsBuyerType) throw new Exception("You can't sell item to this user.");
        dataContext.spInvoice_SellItemToBuyer(auction_id, user_id);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    

    //public void SendingInvoices()
    //{
    //  List<UserInvoice> inv = (from I in dataContext.UserInvoices
    //                           where I.Event_ID == 910
    //                           orderby I.ID
    //                           select I).ToList();
    //  int count = 0;
    //  foreach (UserInvoice ui in inv)
    //  {
    //    try
    //    {
    //      Vauction.Utils.Helper.Mail.SendInvoice(ui.User.Email, ui.ID.ToString());
    //      Vauction.Utils.Lib.Logger.LogInfo(ui.ID.ToString());
    //      count++;
    //    }
    //    catch
    //    {
    //      Vauction.Utils.Lib.Logger.LogInfo("eror InvoiceId = " + ui.ID.ToString());
    //    }
    //  }
    //  Vauction.Utils.Lib.Logger.LogInfo("Count = " + count.ToString());

    //}
  }
}
