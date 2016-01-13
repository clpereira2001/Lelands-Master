<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<InvoiceDetail>>" %>

 <% 
   if (Model == null || Model.Count == 0)
      Response.Write("There are currently no invoices.");
  else
  { 
     UserInvoice ui = ViewData["UserInvoice"] as UserInvoice;
    %>
    Invoice ID:&nbsp;&nbsp;<u>&nbsp;<%=Model[0].UserInvoice_ID %>&nbsp;</u>&nbsp;&nbsp;&nbsp;
    Invoice Date:&nbsp;&nbsp;<u>&nbsp;<%=Model[0].SaleDate.ToShortDateString() %>&nbsp;</u>
    <br /><br />      
    <table border="0" cellpadding="0" cellspacing="0"  width="100%" >
      <tr style="background-color: #D2D2D2" class="bordered">
        <th style="padding-left:10px; width:30px">
          Lot
        </th>
        <th style="width:600px">
          Title
        </th>
        <th style="width: 100px">
          Cost
        </th>        
      </tr>
      <% bool line = true;
         decimal sumcost = 0;
         decimal sumshipping = 0;
         decimal sumtax = 0;
         decimal sumfees = 0;         
         decimal tmp = 0;
         bool isnotshipment = false;
         tmp = Convert.ToDecimal(ViewData["AmountPaid"]);
         foreach (InvoiceDetail item in Model)
         {
           if (item.Shipping == 0) isnotshipment = true;           
           %>
      <%=((line) ? "<tr style=\"background-color:#EFEFEF\" class=\"bordered\">" : "<tr class=\"bordered\">")%>
      <td style="padding-left: 5px;">        
        <%=Html.ActionLink(item.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.Auction_ID, evnt =   item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }, new { @style = "font-size:14px; color:#000" })%>
      </td>
       <td style="padding-left: 5px;">
         <%=Html.ActionLink(item.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.Auction_ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }, new { @style = "font-size:14px; color:#000" })%>
      </td>
      <td style="padding-left: 5px;">
        <%=item.Cost.GetCurrency()%>
      </td>
     </tr>
      <% line = !line;
         sumcost += item.Cost;
         sumshipping += item.Shipping;
         sumtax += item.Tax;
         sumfees += item.LateFee;
       }%>
    <tr>
      <td colspan="2" style="text-align:right; font-weight:bold">Net Cost*:&nbsp;</td><td><%=sumcost.GetCurrency()%></td>
    </tr>
    <tr>
      <td colspan="2" style="text-align:right; font-weight:bold">Shipping, Handiling & Insurance:&nbsp;</td><td><%=(sumshipping == 0 || isnotshipment) ? "not calculated yet" : sumshipping.GetCurrency()%></td>
    </tr>
    <tr>
      <td colspan="2" style="text-align:right; font-weight:bold">Sales Tax:&nbsp;</td><td><%=(sumtax==0)?"$0.00":sumtax.GetCurrency()%></td>
    </tr>
    <tr>
      <td colspan="2" style="text-align:right; font-weight:bold">Late Fees:&nbsp;</td><td><%=(sumfees == 0) ? "$0.00" : sumfees.GetCurrency()%></td>
    </tr>
    <tr>
      <td colspan="2" style="text-align:right; font-weight:bold">Amount Paid:&nbsp;</td><td><%=(tmp == 0) ? "$0.00" : tmp.GetCurrency()%></td>
    </tr>
    <tr>
      <% tmp = sumcost + sumfees + sumshipping + sumtax - tmp; %>
      <td colspan="2" style="text-align:right; font-weight:bold">Amount Due:&nbsp;</td><td><%=(tmp < 0 || (sumshipping == 0 || isnotshipment)) ? "not calculated yet" : tmp.GetCurrency(false) %></td>
    </tr>
    <tr>
      <td colspan="3">* Auction items include premium</td>      
    </tr>    
    </table>
    
    <%--<% if (Model.First().LinkParams.EventUrl=="November-2010-Catalog") { %>
    <b>Due to the Christmas Holiday - shipping will not begin until December 27th.</b> <br /><br />
    <%} %>--%>

    We accept personal checks, money orders, cash, and wire transfers. WE DO NOT ACCEPT CREDIT CARDS. If you have any questions, please contact customer service at 631-244-0077

<br/><br/>
<strong>Make Check Payment out to Lelands Collectibles</strong>
    <% }%>