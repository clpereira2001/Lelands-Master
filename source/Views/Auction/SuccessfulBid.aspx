<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Successful bid - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<div id="left_column">
  <%--<%Html.RenderPartial("~/Views/Auction/_CategoryList.ascx"); %>--%>
  <% Html.RenderAction("pCategoryTree", "Auction", new { event_id=Model.LinkParams.Event_ID, maincategory_id = Model.LinkParams.MainCategory_ID });%>
</div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Congratulations <%=AppHelper.CurrentUser.Login %>!</div>
      You are currently the high bidder for <b><%= Html.ActionLink("Lot#" +Model.LinkParams.Lot+". "+ Model.LinkParams.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl }, new { @style = "font-size:14px" })%></b>.
      Should you be outbid or win the lot, you will be notified via e-mail.
      <br /><br />
    <center>
      <%= Html.ActionLink("Back to Lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl }, new { @style = "font-size:14px" })%>
      |
      <%= Html.ActionLink("Back to Category", "CategoryView", new { controller = "Auction", action = "CategoryView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl }, new { @style = "font-size:14px" })%>
    </center>
    <% if (Model.PrevAuction!=null || Model.NextAuction!=null) { %><br class="clear" /> <br class="clear" /><%} %>
    <center>
    <% if (Model.PrevAuction!=null){ %>
      <%=Html.ActionLink("< previous lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl })%>
    <%} %>
    <%= (Model.PrevAuction!=null && Model.NextAuction!=null)?"&nbsp;|&nbsp":String.Empty %>
    <% if (Model.NextAuction!=null){ %>
      <%=Html.ActionLink("next lot >", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl })%>
    <%} %>
    </center>
  </div>
</asp:Content>