<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionShort>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<% AuctionShort categoryItem = ViewData.Model as AuctionShort;
   bool IsShownOpenBidOne = (ViewData["IsShownOpenBidOne"] != null) ? Convert.ToBoolean(ViewData["IsShownOpenBidOne"]) : false;
   bool IsUserRegisteredForEvent = (ViewData["IsUserRegisteredForEvent"] != null && Convert.ToBoolean(ViewData["IsUserRegisteredForEvent"]));
   GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
   SessionUser cuser = AppHelper.CurrentUser;
   string strPrice = ((IsShownOpenBidOne) ? ((cuser==null || !IsUserRegisteredForEvent) ? "$1.00" : (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *"))) : (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *")));  
%>
<div style="width: 185px; text-align: center;">
  <%  string address = AppHelper.AuctionImage(categoryItem.LinkParams.ID, categoryItem.ThumbnailPath);
    if (System.IO.File.Exists(Server.MapPath(address)) && (Convert.ToBoolean(filterParams.ImageViewMode)) && (!categoryItem.PulledOut))
    {%>
  <a href="<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", categoryItem.LinkParams.ID, categoryItem.LinkParams.EventUrl, categoryItem.LinkParams.MainCategoryUrl, categoryItem.LinkParams.CategoryUrl, categoryItem.LinkParams.GetLotTitleUrl(categoryItem.Lot, categoryItem.Title)).Collect("page", "ViewMode"))%>">
    <img src="<%=AppHelper.CompressImagePath(address) %>" title="<%= String.Format("Lot{0}~{1}", categoryItem.Lot, categoryItem.Title.Replace("\"", "\'")) %>" alt="" />
  </a>
  <%} %>
<%--   <br />
 <% if (!categoryItem.PulledOut)
     {%>
  <div id="add_item" class="custom_button" style="width: 80px; //width: 100px; margin-left: 50px;
    //margin-left: 0px;">
    <button type="button" onclick="javascript:location='<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", categoryItem.LinkParams.ID, categoryItem.LinkParams.EventUrl, categoryItem.LinkParams.MainCategoryUrl, categoryItem.LinkParams.CategoryUrl, categoryItem.LinkParams.GetLotTitleUrl(categoryItem.Lot, categoryItem.Title)).Collect("page", "ViewMode"))%>'">
      <%= (cuser!=null && categoryItem.Status == (byte)Consts.AuctionStatus.Open) ? "Bid Now" : "Preview"%></button>
  </div>
  <%} %>--%>
</div>
<div id="table_grid_div_description">
  <%=(categoryItem.IsBold)? "<b>":String.Empty %>
  <%=Html.ActionLink("Lot " + categoryItem.Lot.ToString() + ". " + categoryItem.Title.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = categoryItem.LinkParams.ID, evnt = categoryItem.LinkParams.EventUrl, maincat = categoryItem.LinkParams.MainCategoryUrl, cat = categoryItem.LinkParams.CategoryUrl, lot = categoryItem.LinkParams.GetLotTitleUrl(categoryItem.Lot, categoryItem.Title) })%>
  <%=(categoryItem.IsBold)? "</b>":String.Empty %>
  <br class="br_clear" />
  <span>
    <% if (!categoryItem.IsUnsoldOrPulledOut)
       { %>
       <b>
       <%=(categoryItem.CurrentBid>0)?"Winning Bid: "+categoryItem.CurrentBid.GetCurrency()+"<br />":String.Empty %>      
      <%=(categoryItem.Status==(byte)Consts.AuctionStatus.Open)?((IsUserRegisteredForEvent && categoryItem.Bids>0) ? "Current Bids:" : "Opening Bids:"):(categoryItem.PriceRealized>0?"Price Realized: ":"Reserve: ")%>      
      <%= (IsUserRegisteredForEvent) ? (((categoryItem.Bids == 0 && categoryItem.CurrentBid==0) || categoryItem.IsUnsoldOrPulledOut) ? (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *")) : categoryItem.PriceRealized.GetCurrency()) : strPrice %>
      </b>
      <br class="br_clear" />
      <% if (categoryItem.Bids > 0)
      {%>
      Bids:
      <%=categoryItem.Bids%>
      <%} %>
    <%} else { %>
	    <%:categoryItem.UnsoldOrPulledOut%>
	  <%} %>
  </span>
</div>
