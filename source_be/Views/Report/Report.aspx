<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ReportParam>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
  Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <title></title>
  <style type="text/css">
    .report_cssParam{display:none}
  </style>  
</head>
<body>
  <script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
      if (IsPostBack) return;
      tbEvent.Text = Model.Event_ID.HasValue ? Model.Event_ID.Value.ToString() : String.Empty;
      tbUser.Text = Model.User_ID.HasValue ? Model.User_ID.Value.ToString() : String.Empty;
      tbDate.Text = Model.Date.HasValue ? Model.Date.Value.ToString("d") : DateTime.Now.ToString("d");
      tbDateTo.Text = Model.DateTo.HasValue ? Model.DateTo.Value.ToString("d") : DateTime.Now.ToString("d");
      tbStatus.Text = Model.Status.GetValueOrDefault(-1).ToString();
      repview.LocalReport.DataSources.Clear();
      switch (Model.Report_ID)
      {
        case 0: 
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/PrintedConsignorStatementReport.rdlc");          
          repview.LocalReport.DataSources.Add(new ReportDataSource{ DataSourceId="odsPrintedConsignorStatementReport", Name="spReport_PrintingConsignorStatementsResult"});
          break;
        case 1:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/AuctionPaidReport.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsAuctionPaid", Name = "sp_Report_AuctionPaidResult" });
          break;
        case 2:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/BuyerInvoices.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsBuyerInvoice", Name = "sp_Report_BuyerInvoicesResult" });
          break;
        case 3:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/ConsignorSettlement.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsConsignorSettlementReport", Name = "sp_Report_ConsignorSettlementResult" });
          break;
        case 4:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/ConsignorsVsLelands.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsConsignorVsLelands", Name = "spReport_ConsignorsVsLelandsResult" });
          break;
        case 5:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/DayPaymentsReport.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsDayPaymentsReport", Name = "sp_Report_DayPaymentsResult" });
          break;
        case 6:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/PrintedInvoicesReport.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsPrintedInvoicesReport", Name = "sp_Report_PrintedInvoicesResult" });
          break;
        case 7:
          repview.LocalReport.ReportPath = Server.MapPath("~/Templates/Reports/SalesTaxReport.rdlc");
          repview.LocalReport.DataSources.Add(new ReportDataSource { DataSourceId = "odsSalesTaxReport", Name = "spReport_SalesTaxResult" });
          break;
        default: break;
      }
    }
  </script>

  <form id="frmReport" runat="server">
  <asp:TextBox runat="server" ID="tbEvent" CssClass="report_cssParam" Text="" />
  <asp:TextBox runat="server" ID="tbUser" CssClass="report_cssParam" Text="" />
  <asp:TextBox runat="server" ID="tbStatus" CssClass="report_cssParam" Text="" />
  <asp:TextBox runat="server" ID="tbDate" CssClass="report_cssParam" Text="" />
  <asp:TextBox runat="server" ID="tbDateTo" CssClass="report_cssParam" Text="" />
  
  <% switch (Model.Report_ID){
       case 0: %>
  <!-- Consignor Statement (p) -->
  <asp:ObjectDataSource ID="odsPrintedConsignorStatementReport" runat="server" SelectMethod="GetPrintedConsignorStatements"
    TypeName="Vauction.Controllers.ReportController">
    <SelectParameters>
      <asp:ControlParameter ControlID="tbEvent" DefaultValue="" Name="event_id" PropertyName="Text" Type="Int64" />
      <asp:ControlParameter ControlID="tbUser" DefaultValue="" Name="user_id" PropertyName="Text" Type="Int64" />
      <asp:ControlParameter ControlID="tbStatus" DefaultValue="" Name="status" PropertyName="Text" Type="Int32" />
      <asp:ControlParameter ControlID="tbDate" DefaultValue="" Name="printeddate" PropertyName="Text" Type="DateTime" />
    </SelectParameters>
  </asp:ObjectDataSource>
  <% break; case 1: %>
  <!-- Auction Paid -->
  <asp:ObjectDataSource ID="odsAuctionPaid" runat="server" 
        SelectMethod="GetAuctionPaid" TypeName="Vauction.Controllers.ReportController">
        <SelectParameters>
          <asp:ControlParameter ControlID="tbEvent" DefaultValue="-1" Name="event_id" PropertyName="Text" Type="Int64" />
        </SelectParameters>
      </asp:ObjectDataSource>
 <% break; case 2: %>
  <!-- Buyer Invoice -->
  <asp:ObjectDataSource ID="odsBuyerInvoice" runat="server" SelectMethod="GetBuyerInvoices" TypeName="Vauction.Controllers.ReportController">
    <SelectParameters>
      <asp:ControlParameter ControlID="tbEvent" DefaultValue="-1" Name="event_id" PropertyName="Text" Type="Int64" />
      <asp:ControlParameter ControlID="tbStatus" DefaultValue="" Name="ispaid" PropertyName="Text" Type="Int32" />
    </SelectParameters>
  </asp:ObjectDataSource>
  <% break; case 3: %>
  <!-- Consignor Settlement Report -->
  <asp:ObjectDataSource ID="odsConsignorSettlementReport" runat="server" SelectMethod="GetUserInvoices" TypeName="Vauction.Controllers.ReportController">
    <SelectParameters>
      <asp:ControlParameter ControlID="tbEvent" DefaultValue="" Name="event_id" PropertyName="Text" Type="Int64" />
      <asp:ControlParameter ControlID="tbUser" DefaultValue="" Name="user_id" PropertyName="Text" Type="Int64" />      
    </SelectParameters>    
  </asp:ObjectDataSource>
  <% break; case 4: %>
  <!-- Consignor vs Lelands -->
    <asp:ObjectDataSource ID="odsConsignorVsLelands" runat="server" SelectMethod="GetConsignorsVsLelands" TypeName="Vauction.Controllers.ReportController">
      <SelectParameters>
        <asp:ControlParameter ControlID="tbEvent" DefaultValue="-1" Name="event_id" PropertyName="Text" Type="Int64" />
      </SelectParameters>
    </asp:ObjectDataSource>
  <% break; case 5: %>
  <!-- Daily payments -->
  <asp:ObjectDataSource ID="odsDayPaymentsReport" runat="server" SelectMethod="GetDayPayments" TypeName="Vauction.Controllers.ReportController">
    <SelectParameters>
      <asp:ControlParameter ControlID="tbDate" DefaultValue="" Name="datefrom" PropertyName="Text" Type="DateTime" />
      <asp:ControlParameter ControlID="tbDateTo" DefaultValue="" Name="dateto" PropertyName="Text" Type="DateTime" />      
    </SelectParameters>    
  </asp:ObjectDataSource>
  <% break; case 6: %>
  <!-- Printed Invoices -->
  <asp:ObjectDataSource ID="odsPrintedInvoicesReport" runat="server" SelectMethod="GetPrintedInvoices" TypeName="Vauction.Controllers.ReportController">
    <SelectParameters>
      <asp:ControlParameter ControlID="tbEvent" DefaultValue="" Name="event_id" PropertyName="Text" Type="Int64" />
      <asp:ControlParameter ControlID="tbUser" DefaultValue="" Name="user_id" PropertyName="Text" Type="Int64" />      
      <asp:ControlParameter ControlID="tbDate" DefaultValue="" Name="printeddate" PropertyName="Text" Type="DateTime" />
      <asp:ControlParameter ControlID="tbStatus" DefaultValue="" Name="status" PropertyName="Text" Type="Int32" />    
    </SelectParameters>    
  </asp:ObjectDataSource>
  <% break; case 7: %>
  <!-- Sales Tax -->
    <asp:ObjectDataSource ID="odsSalesTaxReport" runat="server" SelectMethod="GetSalesTax" TypeName="Vauction.Controllers.ReportController">
      <SelectParameters>
        <asp:ControlParameter ControlID="tbEvent" DefaultValue="" Name="event_id" PropertyName="Text" Type="Int64" />
        <asp:ControlParameter ControlID="tbStatus" DefaultValue="" Name="status" PropertyName="Text" Type="Int32" />
      </SelectParameters>
    </asp:ObjectDataSource>
  <% break; default: break;
  } %>

  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" ScriptMode="Release" AsyncPostBackTimeOut="36000"> </asp:ScriptManager>
    <asp:UpdatePanel ID="ReportViewerUP" runat="server">
      <ContentTemplate>
        <rsweb:ReportViewer ID="repview" runat="server" Width="1000px" Font-Names="Verdana" Font-Size="8pt" InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" AsyncRendering="False" Height="580px">
        </rsweb:ReportViewer>
      </ContentTemplate>
    </asp:UpdatePanel>
  </form>
</body>
</html>

