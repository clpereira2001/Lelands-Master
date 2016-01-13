<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="_TableViewPast.ascx.cs" Inherits="Vauction.Views.Auction.TableView" %>
<% IEnumerable<AuctionShort> categoryItems = ViewData.Model as IEnumerable<AuctionShort>; %>
<%
  bool IsShownOpenBidOne = (ViewData["IsShownOpenBidOne"] != null) ? Convert.ToBoolean(ViewData["IsShownOpenBidOne"]) : true;
  bool IsUserRegisteredForEvent = (ViewData["IsUserRegisteredForEvent"] != null && Convert.ToBoolean(ViewData["IsUserRegisteredForEvent"]));
  GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
  if (categoryItems == null)
  {%>
<div>
  No auction found matching your criteria.</div>
<% }
  else
  {%>
<table id="item_view" border="0" cellpadding="0" cellspacing="0" width="100%">  
  <tr style="background-color: #D2D2D2">
    <th style="padding-left: 10px; width: 30px">
      <%= Html.ActionLink("Lot", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Lot))%>
    </th>
    <th style="width: 510px">
      <%= Html.ActionLink("Auction Title", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Title))%>
    </th>
    <th style="width: 90px">
      <%= Html.ActionLink("Winning Bid", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.CurrentBid))%>
    </th>
    <th style="width: 90px">
      <%= Html.ActionLink("Price Realized", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.CurrentBid))%>
    </th>
    <th style="width: 30px">
      <%= Html.ActionLink("Bids", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Bids))%>
    </th>
  </tr>
  <% 
    string address;
    string strPrice;
    int count = 0;
    foreach (AuctionShort item in categoryItems)
    {
      count++;      
      strPrice = (IsShownOpenBidOne) ? ((AppHelper.CurrentUser==null || !IsUserRegisteredForEvent) ? "$1.00" : (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *"))) : (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *"));
  %>
  <%=((count%2==1)?"<tr style=\"background-color:#EFEFEF\">":"<tr>") %>  
  <td style="padding-left: 10px;">
    <%=item.IsBold ? "<b>" : ""%>
     <%=Html.ActionLink(item.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) })%>
    <%=item.IsBold ? "</b>" : ""%>
  </td>
  <td>
    <%=item.IsBold ? "<b>" : ""%>
    <%=Html.ActionLink(item.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) })%>
    <%=item.IsBold ? "</b>" : ""%>
  </td>
  <td>
    <%=item.IsBold ? "<b>" : ""%><%= (IsUserRegisteredForEvent) ? (((item.Bids == 0 && item.CurrentBid == 0) || item.IsUnsoldOrPulledOut) ? "" : item.CurrentBid.GetCurrency()) : strPrice%><%=item.IsBold ? "</b>" : ""%>
  </td>
  <td>
    <%=item.IsBold ? "<b>" : ""%><%= (!item.IsUnsoldOrPulledOut)?((IsUserRegisteredForEvent) ?((item.Bids == 0 && item.CurrentBid==0) ? (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *")) : item.PriceRealized.GetCurrency()) : strPrice) : item.UnsoldOrPulledOut %>
    <%=item.IsBold ? "</b>" : ""%>
  </td>
  <td>
    <% if (item.Bids > 0)
       { %>
    <%=item.IsBold ? "<b>" : ""%>
    <%=item.Bids%>
    <%=item.IsBold ? "</b>" : ""%>
    <%} %>
  </td>
  </tr>
  <% 
    if (!Convert.ToBoolean(filterParams.ImageViewMode) || item.PulledOut) continue;
    address = AppHelper.AuctionImage(item.LinkParams.ID, item.ThumbnailPath);
    if (!System.IO.File.Exists(Server.MapPath(address))) continue;
  %>
  <%=((count % 2 == 1) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr>")%>
  <td colspan="5">
    <a href="<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.MainCategoryUrl, item.LinkParams.CategoryUrl, item.LinkParams.GetLotTitleUrl(item.Lot, item.Title)).Collect("page", "ViewMode"))%>">
      <img src="<%=AppHelper.CompressImagePath(address) %>" title="<%=String.Format("Lot{0}~{1}", item.Lot, item.Title.Replace("\"", "\'")) %>" alt="" /></a>
  </td>
  </tr>
  <% } %>
  </table>
<% }%>