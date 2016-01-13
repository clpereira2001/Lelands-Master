<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Advanced Search - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_3.jpg")+"'" : String.Empty %> alt="" width="180" height="600" /></div>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
   <div class="center_content">
      <div class="page_title">Advanced Search</div>
      <% Html.RenderAction("pAdvancedSearch", "Home", new { eventid = (ViewData["CurrentEvent"] as Event).ID, param = new Vauction.Models.CustomModels. AuctionFilterParams() }); %>
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