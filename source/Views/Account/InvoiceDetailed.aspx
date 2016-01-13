<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<IInvoice>>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%:ViewData["Event_Title"] %> < Your Auction Invoices - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column"><img  <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_3.jpg")+"'" : String.Empty %> alt="" width="180" height="600" /></div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title"><%= Html.ActionLink("Your Auction Invoices", "AuctionInvoices", "Account", new { controller = "Account", action = "AuctionInvoices" }, new { @style="font-size:16px" })%> > <span style="color:#000"><%:ViewData["Event_Title"]%></span></div>
    <% Html.RenderAction("pInvoiceDetailed", "Account", new { userinvoice_id = Convert.ToInt64(ViewData["Invoice_ID"]) });%>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_3.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>
