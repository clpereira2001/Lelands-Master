<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <% EventCategory evCat = ViewData["EventCategory"] as EventCategory; %>
  <title><%=ViewData["Title"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
 <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <% Event evnt = ViewData["Event"] as Event;
     if (evnt == null)
     {
       Response.Write("There are no results for this auction");
     } else {
       EventCategoryDetail evCat = ViewData["EventCategory"] as EventCategoryDetail;
  %>  
  <div class="center_content"> 
    <div class="page_title"><%=evnt.Title %> > <%=evCat.LinkParams.MainCategoryTitle %> > <%=evCat.LinkParams.CategoryTitle %></div>
    <% Html.RenderAction("pPastAuctionMenu", "Auction", new { event_id=evnt.ID, event_title=evnt.UrlTitle });%>
    <br /><br />
    <% CategoryFilterParams filterParams = ViewData["filterParam"] as CategoryFilterParams;%>
     <% Html.RenderAction("pPastCategoriesView", "Auction", new { event_id = evnt.ID, param = filterParams, page = filterParams.page, viewmode=filterParams.ViewMode, imageviewmode=filterParams.ImageViewMode });%>    
  </div>
  <%} %>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
 <%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_2.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>