using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Helper;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public class ReportRepository : IReportRepository
  {
    private VauctionDataContext dataContext;

    public ReportRepository(VauctionDataContext dataContext)
    {
      this.dataContext = dataContext;
    }

    //GetConsignorSettlementResult
    public List<sp_Report_ConsignorSettlementResult> GetConsignorSettlementResult(long event_id, long user_id)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.sp_Report_ConsignorSettlement(event_id, user_id).ToList();
    }

    //GetSalesTaxResult
    public List<spReport_SalesTaxResult> GetSalesTaxResult(long event_id, int status)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.spReport_SalesTax(event_id, status).ToList();
    }

    //GetBuyerInvoices
    public List<sp_Report_BuyerInvoicesResult> GetBuyerInvoices(long event_id, int? ispaid)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.sp_Report_BuyerInvoices(null, null, event_id, null, (!ispaid.HasValue || ispaid.Value==-1)?null:(bool?)Convert.ToBoolean(ispaid.GetValueOrDefault(0))).ToList();
    }

    //GetAuctionPaid
    public List<sp_Report_AuctionPaidResult> GetAuctionPaid(long event_id)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.sp_Report_AuctionPaid(event_id).ToList();
    }

    //GetPrintedInvoicesReport
    public List<sp_Report_PrintedInvoicesResult> GetPrintedInvoicesReport(long event_id, long user_id, DateTime datetime, int ispaid)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.sp_Report_PrintedInvoices(event_id, user_id, datetime, ispaid).ToList();
    }

    //GetDayPayments
    public List<sp_Report_DayPaymentsResult> GetDayPayments(DateTime datefrom, DateTime dateto)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.sp_Report_DayPayments(datefrom, dateto).ToList();
    }

    //GetPrintedConsignorStatements
    public List<spReport_PrintingConsignorStatementsResult> GetPrintedConsignorStatements(long event_id, long user_id, int status, DateTime datetime)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.spReport_PrintingConsignorStatements(event_id, user_id, status, datetime).ToList();
    }

    //GetAuctionConsignorTotals
    public List<spReport_ConsignorsVsLelandsResult> GetConsignorsVsLelands(long event_id)
    {
      dataContext.CommandTimeout = 10 * 60 * 1000;
      return dataContext.spReport_ConsignorsVsLelands(event_id).ToList();
    }
  }
}
