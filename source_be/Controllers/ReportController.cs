using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [VauctionAuthorize(Roles = "Root,Admin"), Compress]
  public class ReportController : BaseController
  { 
    private IReportRepository ReportRepository;
    public ReportController()
    {
      ReportRepository = dataProvider.ReportRepository;
    }

    //Report
    public ActionResult Report(int report, long? event_id, long? user_id, int? status, DateTime? date, DateTime? dateTo)
    {
      return View(new ReportParam { Report_ID = report, Event_ID = event_id, User_ID = user_id, Status = status, Date = date, DateTo=dateTo });
    }

    //ConsignorSettlementReport
    public ActionResult ConsignorSettlementReport()
    {
      if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetUserInvoices
    public List<sp_Report_ConsignorSettlementResult> GetUserInvoices(long? event_id, long? user_id)
    {
      return (!event_id.HasValue) ? new List<sp_Report_ConsignorSettlementResult>() : ReportRepository.GetConsignorSettlementResult(event_id.Value, (user_id.HasValue) ? user_id.Value : -1);
    }

    //SalesTax
    public ActionResult SalesTax()
    {
      //if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetSalesTex
    public List<spReport_SalesTaxResult> GetSalesTax(long? event_id, int? status)
    {
      return (!event_id.HasValue) ? new List<spReport_SalesTaxResult>() : ReportRepository.GetSalesTaxResult(event_id.Value, !status.HasValue?0:status.Value);
    }

    //BuyerInvoices
    public ActionResult BuyerInvoices()
    {
      //if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetBuyerInvoices
    public List<sp_Report_BuyerInvoicesResult> GetBuyerInvoices(long? event_id, int? ispaid)
    {
      return (!event_id.HasValue) ? new List<sp_Report_BuyerInvoicesResult>() : ReportRepository.GetBuyerInvoices(event_id.Value, ispaid);
    }

    //AuctionPaid
    public ActionResult AuctionPaid()
    {
      //if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetAuctionPaid    
    public List<sp_Report_AuctionPaidResult> GetAuctionPaid(long? event_id)
    {      
      return (!event_id.HasValue) ? new List<sp_Report_AuctionPaidResult>() : ReportRepository.GetAuctionPaid(event_id.Value);
    }

    //PrintedInvoices
    public ActionResult PrintedInvoices()
    {
      //if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetUserInvoices
    public List<sp_Report_PrintedInvoicesResult> GetPrintedInvoices(long? event_id, long? user_id, DateTime? printeddate, int? status)
    {
      return (!event_id.HasValue) ? new List<sp_Report_PrintedInvoicesResult>() : ReportRepository.GetPrintedInvoicesReport(event_id.Value, (user_id.HasValue) ? user_id.Value : -1, printeddate.HasValue ? printeddate.Value : DateTime.Now, status.GetValueOrDefault(-1));
    }

    //DayPayments
    public ActionResult DayPayments()
    {
      //if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetDayPayments
    public List<sp_Report_DayPaymentsResult> GetDayPayments(DateTime? datefrom, DateTime? dateto)
    {
      DateTime from, to;
      from = datefrom ?? DateTime.Now.Date;
      to = dateto ?? DateTime.Now.Date;      
      to = to.AddDays(1).AddSeconds(-1);
      return ReportRepository.GetDayPayments(from, to);
    }

    //PrintedConsignorStatements    
    public ActionResult PrintedConsignorStatements()
    {
      if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetDayPayments
    public List<spReport_PrintingConsignorStatementsResult> GetPrintedConsignorStatements(long? event_id, long? user_id, int? status, DateTime? printeddate)
    {
      return (!event_id.HasValue) ? new List<spReport_PrintingConsignorStatementsResult>() : ReportRepository.GetPrintedConsignorStatements(event_id.Value, (user_id.HasValue) ? user_id.Value : -1, status.HasValue ? status.Value : -1, printeddate.HasValue ? printeddate.Value : DateTime.Now);
    }

    //ConsignorsVsLelands
    public ActionResult ConsignorsVsLelands()
    {
      if (AppHelper.CurrentUser.ID != 21254) return RedirectToAction("Index", "Home");
      return View();
    }

    //GetAuctionConsignorTotals    
    public List<spReport_ConsignorsVsLelandsResult> GetConsignorsVsLelands(long? event_id)
    {
      return (!event_id.HasValue) ? new List<spReport_ConsignorsVsLelandsResult>() : ReportRepository.GetConsignorsVsLelands(event_id.Value);
    } 
  }
}
