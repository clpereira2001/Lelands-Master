<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%=String.Format("Updates - {0} < Action Results - {1}", (ViewData["Event"] as Event).Title, Consts.CompanyTitleName)%></title>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <div style="padding:10px;">     
    <% Event evnt = ViewData["Event"] as Event;
       if (evnt == null)
         Response.Write("There are no updates for this auction");
       else
       {    
    %>  
        <div class="page_title"><%=evnt.Title%> > Updates</div>
        <% Html.RenderAction("pPastAuctionMenu", "Auction", new { event_id=evnt.ID, event_title=evnt.UrlTitle });%>
        <br /><br />
        <% Html.RenderAction("pPastAuctionUpdates", "Auction", new { event_id = evnt.ID });%>
    <%} %>
  </div>
</asp:Content>