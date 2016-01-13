<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionShort>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<% AuctionShort categoryItem = ViewData.Model as AuctionShort;
    bool IsShownOpenBidOne = (ViewData["IsShownOpenBidOne"] != null) ? Convert.ToBoolean(ViewData["IsShownOpenBidOne"]) : false;
    bool IsUserRegisteredForEvent = (ViewData["IsUserRegisteredForEvent"] != null && Convert.ToBoolean(ViewData["IsUserRegisteredForEvent"]));
    GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
    SessionUser cuser = AppHelper.CurrentUser;
    string strPrice = ((IsShownOpenBidOne) ? ((cuser==null || !IsUserRegisteredForEvent) ? "$1.00" : (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *"))) : (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *")));  
%>
  
  <div style="width:185px; text-align:center;">
  <%  string address = AppHelper.AuctionImage(categoryItem.LinkParams.ID, categoryItem.ThumbnailPath);
    if (System.IO.File.Exists(Server.MapPath(address)) && (Convert.ToBoolean(filterParams.ImageViewMode)) && (!categoryItem.PulledOut))
    {%>
    <a href="<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", categoryItem.LinkParams.ID).Collect("page", "ViewMode"))%>">
      <img src="<%=AppHelper.CompressImagePath(address) %>" alt="<%=String.Format("Lot{0}~{1}", categoryItem.Lot, categoryItem.Title.Replace("\"", "\'")) %>" />
    </a>
    <%} %>
  <%--<br />
  <% if (!categoryItem.PulledOut)
     {%>
     <div id="add_item" class="custom_button" style="width:80px;//width:100px;margin-left:50px;//margin-left:0px">
        <button type="button" onclick="javascript:location='<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", categoryItem.LinkParams.ID).Collect("page", "ViewMode"))%>'"><%=(categoryItem.Status == (byte)Consts.AuctionStatus.Open) ? "Bid Now" : "Preview"%></button>
    </div>
     <%} %>--%>
  </div>
  
	<div id="table_grid_div_description">  
	<%=(categoryItem.IsBold)? "<b>":String.Empty %>
      <%=Html.ActionLink("Lot " + categoryItem.Lot.ToString() + ". " + categoryItem.Title.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = categoryItem.LinkParams.ID, evnt = categoryItem.LinkParams.EventUrl, maincat = categoryItem.LinkParams.MainCategoryUrl, cat = categoryItem.LinkParams.CategoryUrl, lot = categoryItem.LinkParams.GetLotTitleUrl(categoryItem.Lot, categoryItem.Title) })%>
  <%=(categoryItem.IsBold)? "</b>":String.Empty %>
	<br class="br_clear" />	
  <div class="cv_gv_panel" id="cv_gv_<%=categoryItem.LinkParams.ID %>" style='font-weight:bold'>
	<% if (!categoryItem.PulledOut)
    {%>	  
	  <%=(categoryItem.Status == (byte)Consts.AuctionStatus.Open) ? ((IsUserRegisteredForEvent && categoryItem.Bids > 0) ? "Current Bids:" : "Opening Bids:") : "Price Realized:"%> <%= (IsUserRegisteredForEvent) ? ((categoryItem.Bids == 0) ? (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *")) : (!categoryItem.IsUnsoldOrPulledOut) ? categoryItem.CurrentBid.GetCurrency() : "UNSOLD") : strPrice%>
	  <br class="br_clear" />
  <% if (categoryItem.Bids > 0)
     {%>
	    Bids: <span id="cv_b_<%=categoryItem.LinkParams.ID %>"><%=categoryItem.Bids%></span>
	<%} %>
	  <%} else { %>
	    <%=categoryItem.UnsoldOrPulledOut %>
	  <%} %>
    </div>
    <%=Html.Hidden("cv_cb_"+categoryItem.LinkParams.ID.ToString(), !categoryItem.IsUnsoldOrPulledOut ? ((categoryItem.Bids == 0) ? (categoryItem.Price.GetCurrency() + ((String.IsNullOrEmpty(categoryItem.Estimate)) ? "" : " *")) : Convert.ToDecimal(categoryItem.CurrentBid).GetCurrency()) : categoryItem.UnsoldOrPulledOut)%>	
	</div>
