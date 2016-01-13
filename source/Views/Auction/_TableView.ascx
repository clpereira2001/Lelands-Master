<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="_TableView.ascx.cs" Inherits="Vauction.Views.Auction.TableView" %>
<% IEnumerable<AuctionShort> categoryItems = ViewData.Model as IEnumerable<AuctionShort>; %>
<%
  bool IsShownOpenBidOne = (ViewData["IsShownOpenBidOne"] != null) ? Convert.ToBoolean(ViewData["IsShownOpenBidOne"]) : true;
  bool IsUserRegisteredForEvent = (ViewData["IsUserRegisteredForEvent"] != null && Convert.ToBoolean(ViewData["IsUserRegisteredForEvent"]));
  GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
  if (categoryItems.Count() == 0)
   {%>
      <div>No auction found matching your criteria.</div>
<% }
   else
   {%>
  <table id="item_view" border="0" cellpadding="0" cellspacing="0"  width="100%" >
    <tr style="background-color:#D2D2D2">
        <th style="padding-left:10px; width:30px">
          <%= Html.ActionLink("Lot", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Lot))%>
        </th>
        <th style="width:560px">
          <%= Html.ActionLink("Auction Title", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Title))%>
        </th>
        <th style="width: 110px">
          <%= Html.ActionLink("Price", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.CurrentBid))%>
        </th>
        <th style="width:50px">
          <%= Html.ActionLink("Bids", ViewData["PageActionPath"].ToString(), GetSpesificParams(Consts.CategorySortFields.Bids))%>
        </th>
    </tr>
 <% string address;
    string strPrice;
    int count = 0;
    foreach (AuctionShort item in categoryItems)
    {
      count++;      
      strPrice = (IsShownOpenBidOne) ? ((AppHelper.CurrentUser==null || !IsUserRegisteredForEvent) ? "$1.00" : (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *"))) : (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *"));
  %>
  <%=((count % 2 == 1) ? "<tr class='tbl_res_row' defcolor='#EFEFEF' defchanged='#FFA' style=\"background-color:#EFEFEF\" id='tbl_res_row_" + item.LinkParams.ID + "'>" : "<tr class='tbl_res_row' defcolor='#fff' defchanged='#FFA' id='tbl_res_row_" + item.LinkParams.ID + "'>")%>
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
    <%=item.IsBold ? "<b>" : ""%>
    <span id="cv_cb_<%=item.LinkParams.ID %>"><%= !item.IsUnsoldOrPulledOut?((IsUserRegisteredForEvent) ? ((item.Bids == 0) ? (item.Price.GetCurrency() + ((String.IsNullOrEmpty(item.Estimate)) ? "" : " *")) : Convert.ToDecimal(item.CurrentBid).GetCurrency()) : strPrice):item.UnsoldOrPulledOut%></span>
    <%=item.IsBold ? "</b>" : ""%>
  </td>
  <td>
    <%=item.IsBold ? "<b>" : ""%>
    <span id="cv_b_<%=item.LinkParams.ID %>"><%=item.Bids>0?item.Bids.ToString():String.Empty%></span>
    <%=item.IsBold ? "</b>" : ""%>
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
<%}%>