<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Consignors - <%=Consts.CompanyTitleName %></title>
  <% Html.Script("maxlength.js"); %>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Consignor Auction Listing</h2>
  <table>
    <tr>
      <td>
        <table id="c_list"></table>
        <div id="c_pager"></div>
      </td>
    </tr>
    <tr>
      <td>
        <table id="a_list"></table>
        <div id="a_pager"></div>
      </td>
    </tr>
  </table>
  <% Html.RenderPartial("~/Views/Auction/ConsignorForm.ascx"); %>  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <script type="text/javascript">    
    var consignment_table_events = "<%=ViewData["EventsList"] %>";
    var consignment_table_paymenttypes = "<%=ViewData["PaymentTypes"] %>";
    //    var consignment_table_committionrates = "<%=ViewData["CommissionRateTypes"] %>";
  </script>  
  <%Html.ScriptV("Consignors.js"); %>
  <%--<%Html.ScriptV("ConsignorForm.js"); %>--%>
  <script src="/Scripts/Vauction/ConsignorForm.js" type="text/javascript"></script>
  <%Html.ScriptV("UserForm.js"); %>  
</asp:Content>
