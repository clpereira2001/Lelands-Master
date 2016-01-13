using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IReportRepository
  {
    List<sp_Report_ConsignorSettlementResult> GetConsignorSettlementResult(long event_id, long user_id);
    List<spReport_SalesTaxResult> GetSalesTaxResult(long event_id, int status);
    List<sp_Report_BuyerInvoicesResult> GetBuyerInvoices(long event_id, int? ispaid);
    List<sp_Report_AuctionPaidResult> GetAuctionPaid(long event_id);
    List<sp_Report_PrintedInvoicesResult> GetPrintedInvoicesReport(long event_id, long user_id, DateTime datetime, int ispaid);
    List<sp_Report_DayPaymentsResult> GetDayPayments(DateTime datefrom, DateTime dateto);
    List<spReport_PrintingConsignorStatementsResult> GetPrintedConsignorStatements(long event_id, long user_id, int status, DateTime datetime);
    List<spReport_ConsignorsVsLelandsResult> GetConsignorsVsLelands(long event_id);
  }
}