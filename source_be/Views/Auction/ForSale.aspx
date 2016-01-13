<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Items For Sale - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Items For Sale</h2>
  <table id="d_list"></table>
  <div id="d_pager"></div>
  <% Html.RenderPartial("~/Views/Auction/DOWForm.ascx"); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jsContent" runat="server">
  <script type="text/javascript">
    var auction_table_statuses = "<%=ViewData["AuctionStatusesList"] %>";
    var auction_table_commissionrate = "<%=ViewData["CommissionRateList"] %>";
    //var payment_types_for_table = '<%=ViewData["PaymentTypes"] %>';  
  </script>  
  <script src="<%=this.ResolveUrl("~/Scripts/VAuction/DOW.js") %>" type="text/javascript"></script>
  <script src="<%=this.ResolveUrl("~/Scripts/VAuction/DOWForm.js") %>" type="text/javascript"></script>
</asp:Content>
