<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<ConsignmentDetail>>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%:ViewData["Event_Title"] %> < Your Consignment Statement - <%=Consts.CompanyTitleName %></title>
  <link href="/public/scripts/plugins/jquery.signaturepad.css" rel="stylesheet" />
  <!--[if lt IE 9]><script src="/public/scripts/plugins/flashcanvas.js"></script><![endif]-->
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div style="padding:0 10px;font-size:12px">
    <div class="page_title"><%= Html.ActionLink("Your Consignment Statement", "ConisgnorStatements", "Account", new { controller = "Account", action = "ConisgnorStatements" }, new { @style = "font-size:16px" })%> > <span style="color:#000"><%:ViewData["Event_Title"]%></span></div>
    
   <% 
   Consignment consignment = ViewData["ConsignmentStatement"] as Consignment;
   ConsignmentContract consignmentContract = ViewData["ConsignmentContract"] as ConsignmentContract;
    %>
    Consignment Statement ID:&nbsp;&nbsp;<u>&nbsp;<%=consignment.ID %>&nbsp;</u>&nbsp;&nbsp;&nbsp;    
<%
  if (consignmentContract != null && !string.IsNullOrWhiteSpace(consignmentContract.FileName) && consignmentContract.StatusID != (int) Consts.ConsignmentContractStatus.NotGenerated)
  { %>
  <div>
    <span>Contract:</span>
    <a id="showContract" style="margin-left: 15px;">View</a>
    <a id="toggleContract" style="margin-left: 15px; display: none;">Hide</a>
    <a style="margin-left: 15px;" href="<%= Url.Action("DownloadConsignmentContract", "Account", new {id = consignment.ID}) %>">Download</a>
<% if (Model.Any() && consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.Unsigned)
   { %>
    <a id="signContract" style="margin-left: 15px;">Sign Contract</a>
  <div class="sigPad" style="display: none; margin-top: 5px;">
    <ul class="sigNav">
      <li class="drawIt"><a href="#draw-it">Draw Signature</a></li>
      <li class="clearButton"><a href="#clear">Clear</a></li>
    </ul>
    <div class="sig sigWrapper">
    <div class="typed"></div>
    <canvas class="pad" width="198" height="55"></canvas>
    <input type="hidden" name="output" id="sellerSignature" />
    </div>
  </div>
<a id="confirmContract" style="margin-top: 5px; display: none;">Save</a>
   <% } %>
  </div>
<div>
  <object id="conscontract" type="application/pdf" style="width: 100%; margin-top: 10px; display: none;" height="600">
    </object>
</div>
  <br /> 
<% }
  else
  {%>
    <br /><br /> 
 <%  } 
  
   if (Model.Any())
   { %>
    <table border="0" cellpadding="0" cellspacing="0"  width="100%" > 
      <tr style="background-color: #D2D2D2" class="bordered">
        <th style="padding-left:10px; width:30px">
          Lot
        </th>
        <th style="width:550px">
          Title
        </th>
        <th style="width: 100px">
          Hammer Price
        </th>
        <th style="width: 200px">
          Rate
        </th>
        <th style="width: 100px">
          Cost
        </th>
      </tr>
      <% bool line = true;
         foreach (ConsignmentDetail item in Model)
         { %>      
      <tr <%= ((line) ? " style='background-color:#EFEFEF' " : String.Empty) %> class="bordered">
      <td style="padding-left: 5px;">        
        <%= Html.ActionLink(item.LinkParams.Lot.ToString(), "AuctionDetail", new {controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl}, new {@style = "font-size:12px; color:#000"}) %>
      </td>
       <td style="padding-left: 5px;">
         <%= Html.ActionLink(item.LinkParams.Title, "AuctionDetail", new {controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl}, new {@style = "font-size:12px; color:#000"}) %>
      </td>
      <td style="padding-left: 5px;">
        <%= item.Amount.GetCurrency(false) %>
      </td>
      <td style="padding-left: 5px;">
        <%= item.CommissionRate %>
      </td>
      <td style="padding-left: 5px;">
        <%= item.Cost.GetCurrency(false) %>
      </td>
     </tr>
      <% line = !line;
         } %>
      <% UICInvoice totals = ViewData["ConsignmentTotals"] as UICInvoice ?? new UICInvoice(); %>
      <tr>
        <td colspan="4" style="text-align:right; font-weight:bold">Consignment Total:&nbsp;</td><td><%= totals.TotalCost.GetCurrency(false) %></td>
      </tr>
      <tr>
        <td colspan="4" style="text-align:right; font-weight:bold">Payment to Date:&nbsp;</td><td><%= totals.AmountPaid.GetCurrency(false) %></td>
      </tr>
      <tr>
        <td colspan="4" style="text-align:right; font-weight:bold">Amount Due:&nbsp;</td><td><%= totals.AmountDue <= 0 ? "$0.00" : totals.AmountDue.GetCurrency(false) %></td>
      </tr>      
    </table>    
   <% } %>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
 
<script type="text/javascript" src="/public/scripts/plugins/jquery.signaturepad.min.js"></script>
<script type="text/javascript" src="/public/scripts/plugins/json2.min.js"></script>
<script type="text/javascript">
  $(document).ready(function () {
     <%
    Consignment consignment = ViewData["ConsignmentStatement"] as Consignment;
    ConsignmentContract consignmentContract = ViewData["ConsignmentContract"] as ConsignmentContract;
    if (consignment != null && consignmentContract != null && consignmentContract.StatusID == (int) Consts.ConsignmentContractStatus.Unsigned)
    {
  %>
    $('.sigPad').signaturePad({ output: "#sellerSignature" });
    
    $("#signContract").click(function () {
      $(this).hide();
      $(".drawIt a").click();
      $('.sigPad').show();
      $("#confirmContract").show();
    });
    
    $("#confirmContract").click(function () {
      if (confirm("Do you really want to sign this contract?")) {
        $.post('/Account/SignConsignmentContract', { id: '<% = consignment.ID %>', sellerSignature: $("#sellerSignature").val() }, function (data) {
          if (data.success) {
            window.location.reload();
          } else {
            alert(data.Message);
          }
        }, 'json');
      }
    });
    <% } %>

    $("#toggleContract").click(function () {
      $("#conscontract").toggle();
      if ($(this).html() == "Hide") {
        $(this).html('Show');
      } else {
        $(this).html('Hide');
      }
    });
    
    <% if (consignment != null) { %>
    $("#showContract").click(function () {
      $(this).hide();
      $("#conscontract, #toggleContract").show();
      $("#conscontract").attr('data', '<%= Url.Action("ShowConsignmentContract", "Account", new {id = consignment.ID}) %>');
    });
    <%}%>
  });
</script>
</asp:Content>