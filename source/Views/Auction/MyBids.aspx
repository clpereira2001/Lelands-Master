<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%=String.Format("My Bids - {0} < Action Results - {1}", (ViewData["Event"] as Event).Title, Consts.CompanyTitleName)%></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% Event evnt = ViewData["Event"] as Event;
  if (evnt == null){%>
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
  <%}%>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <% Event evnt = ViewData["Event"] as Event;
     if (evnt == null)
       Response.Write("There are no results for this auction");
     else
     {    
  %>  
  <div style="text-align:justify; padding:0px 20px 20px 20px; font-size:12px;">
    <div class="page_title"><%=evnt.Title%> > My Bids</div>
    <% Html.RenderAction("pPastAuctionMenu", "Auction", new { event_id=evnt.ID, event_title=evnt.UrlTitle });%>
    <br /><br />
    <% Html.RenderAction("pMyBids", "Auction", new { event_id = evnt.ID, user_id = AppHelper.CurrentUser.ID });%>
  </div>
  <%} %>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_0.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>