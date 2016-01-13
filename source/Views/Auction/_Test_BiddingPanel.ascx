<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
          
<% using (Html.BeginForm("PlaceBidTest", "Auction")){ %>
  <%=Html.Hidden("id", Model.LinkParams.ID) %>
  <input type="submit" value="Place Bid" id="btnPlaceBid" />
<%} %>