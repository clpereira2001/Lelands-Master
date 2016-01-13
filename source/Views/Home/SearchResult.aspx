<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<%@ Import Namespace="Vauction.Models.CustomModels" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Search - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_3.jpg")+"'" : String.Empty %> alt="" width="180" height="600" /></div>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <% Event evnt = (ViewData["CurrentEvent"] as Event); %>
    <div class="center_content">
      <div class="page_title">Search Result</div>
      <% AuctionFilterParams filterParams = ViewData["filterParam"] as AuctionFilterParams;%>
      <% if (ViewData["ShowAdvancedForm"] != null && Convert.ToBoolean(ViewData["ShowAdvancedForm"])) {
           Html.RenderAction("pAdvancedSearch", "Home", new { eventid = evnt.ID, param = filterParams });
        }%>        
        <h4>Search by - <%: filterParams.ShortFilterString()%></h4>
        <% Html.RenderAction(((!evnt.IsCurrent && evnt.CloseStep==2) || (filterParams.Event_ID.HasValue && (filterParams.Event_ID.Value != evnt.ID || (filterParams.Event_ID.Value == evnt.ID && !evnt.IsCurrent && evnt.CloseStep==2)))) ? "pSearchResultPast" : "pSearchResultCurrent", "Home", new {iscurrent=evnt.IsCurrent, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, imageviewmode = filterParams.ImageViewMode});%>
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