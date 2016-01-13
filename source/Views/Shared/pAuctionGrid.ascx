<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<AuctionShort>>" %>

<% if (ViewData["IsPastGrid"] != null && !Convert.ToBoolean(ViewData["IsPastGrid"]))
   { %>   
  <% Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", Model); %>
<%} else { %>
  <% Html.RenderPartial("~/Views/Auction/_AuctionGridPast.ascx", Model); %>
<%} %>