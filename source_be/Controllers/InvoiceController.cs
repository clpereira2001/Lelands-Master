using System;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [Compress]
  public class InvoiceController : BaseController
  {
    #region init
    private IInvoiceRepository InvoiceRepository;
    private IGeneralRepository GeneralRepository;
    public InvoiceController()
    {
      InvoiceRepository = dataProvider.InvoiceRepository;
      GeneralRepository = dataProvider.GeneralRepository;
    }
    #endregion

    #region dialog methods
    //GetPaymentByStatement
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpGet]
    public JsonResult GetPaymentByStatement(string sidx, string sord, int page, int rows, long? cons_id)
    {
      return (!cons_id.HasValue) ? JSON(false) : JSON(InvoiceRepository.GetPayments(sidx, sord, page, rows, cons_id.Value, false));
    }

    #endregion

    #region buyer invoice
    //BuyerInvoices
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public ActionResult BuyerInvoices()
    {
      ViewData["PaymentTypes"] = GeneralRepository.GetPaymentMethodsForSelect();
      return View();
    }

    //GetUserInvoicesBySearch
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public JsonResult GetUserInvoicesBySearch(string sidx, string sord, int page, int rows, bool? _firstload, long? invoice_id, long? auction_id, long? event_id, long? user_id)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) : JSON(InvoiceRepository.GetUserInvoicesBySearch(sidx, sord, page, rows, invoice_id, auction_id, event_id, user_id));
    }

    //GetInvoicesByUserInvoice
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public JsonResult GetInvoicesByUserInvoice(string sidx, string sord, int page, int rows, long? userinvoice_id)
    {
      return (!userinvoice_id.HasValue) ? JSON(false) : JSON(InvoiceRepository.GetInvoicesByUserInvoice(sidx, sord, page, rows, userinvoice_id.Value));
    }

    //GetPaymentByUserInvoice
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public JsonResult GetPaymentByUserInvoice(string sidx, string sord, int page, int rows, long? userinvoice_id)
    {
      return (!userinvoice_id.HasValue) ? JSON(false) : JSON(InvoiceRepository.GetPayments(sidx, sord, page, rows, userinvoice_id.Value, true));
    }
    #endregion

    #region consignor statement
    //ConsignorStatements
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public ActionResult ConsignorStatements()
    {
      ViewData["CommissionRates"] = GeneralRepository.GetCommissionRatesForSelect();
      ViewData["PaymentTypes"] = GeneralRepository.GetPaymentMethodsForSelect();

      return View();
    }
    //GetStatementsBySearch
    [VauctionAuthorize(Roles = "Root,Admin"), HttpGet]
    public JsonResult GetStatementsBySearch(string sidx, string sord, int page, int rows, bool? _firstload, long? invoice_id, long? auction_id, long? event_id, long? user_id)
    {
      return (!_firstload.HasValue || _firstload.Value) ? JSON(false) : JSON(InvoiceRepository.GetConsignorStatementsBySearch(sidx, sord, page, rows, invoice_id, auction_id, event_id, user_id));
    }
    //GetInvoicesByConsignorStatement
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult GetInvoicesByConsignorStatement(string sidx, string sord, int page, int rows, long? cons_id)
    {
      return (!cons_id.HasValue) ? JSON(false) : JSON(InvoiceRepository.GetInvoicesByConsignorStatement(sidx, sord, page, rows, cons_id.Value));
    }
    #endregion


    // NOT DONE



       

    //EditInvoice
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult EditInvoice(long? id, decimal? Amount, decimal? Shipping, decimal? SalesTax, decimal? LateFees, bool? IsPaid, bool? IsSend)
    {      
      return (!id.HasValue)?JSON(false):JSON(InvoiceRepository.UpdateInvoice(id.GetValueOrDefault(0), Amount.GetValueOrDefault(0), Shipping.GetValueOrDefault(0), SalesTax.GetValueOrDefault(0), LateFees.GetValueOrDefault(0), IsPaid.GetValueOrDefault(false), IsSend.GetValueOrDefault(false)));
    }

    //EditPayment
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult EditUserInvoicePayment(long? Payment_Id, byte? Method, decimal? Amount, DateTime Date, string Note, bool? Cleared, string CDetails, long? userinvoice_id, string oper, string id)
    {
      return JSON(InvoiceRepository.UpdatePayment(Payment_Id.GetValueOrDefault(0), Method.GetValueOrDefault(0), Amount.GetValueOrDefault(0), Date, Note, Cleared.GetValueOrDefault(false), CDetails, userinvoice_id.GetValueOrDefault(0), oper == "add", true));
    }

    //EditStatementInvoice
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult EditStatementInvoice(long? id, int CommRate, bool IsPaid)
    {
      return JSON(InvoiceRepository.UpdateStatementInvoice(id.GetValueOrDefault(0), CommRate, IsPaid));      
    }
    
    //EditPayment
    [VauctionAuthorize(Roles = "Root,Admin,Specialist,SpecialistViewer"), HttpPost]
    public JsonResult EditStatementPayment(long? Payment_Id, byte? Method, decimal? Amount, DateTime Date, string Note, bool? Cleared, string CDetails, long? cons_id, string oper, string id)
    {
      return (!cons_id.HasValue) ? JSON(false) : JSON(InvoiceRepository.UpdatePayment(Payment_Id.GetValueOrDefault(0), Method.GetValueOrDefault(0), Amount.GetValueOrDefault(0), Date, Note, Cleared.GetValueOrDefault(false), CDetails, cons_id.GetValueOrDefault(0), oper == "add", false));
    }

    //RefundDeposit
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult RefundInvoice(long invoice_id)
    {
      return Json(InvoiceRepository.RefundingInvoice(invoice_id));
    }

    //RefundDeposit
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]    
    public JsonResult RefundUserInvoice(long invoice_id)
    {
      return Json(InvoiceRepository.RefundingUserInvoice(invoice_id));
    }

    //SellToBuyer
    [VauctionAuthorize(Roles = "Root,Admin"), HttpPost]
    public JsonResult SellLotToBuyer(long auction_id, long user_id)
    {
      return Json(InvoiceRepository.SellLotToBuyer(auction_id, user_id));
    }
    

    

    

    //public ActionResult SendingInvoices()
    //{
    //  InvoiceRepository.SendingInvoices();
    //  return View();
    //}
  }
}
