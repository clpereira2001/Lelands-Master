<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
<% 
  AuctionUserInfo aui = (ViewData["AuctionUserInfo"] as AuctionUserInfo) ?? new AuctionUserInfo();   
  AuctionShort ashort = ViewData["AuctionShort"] as AuctionShort;
  bool IsUserHasRightsToBid = (Model.Status == Consts.AuctionStatus.Open && (Model.CloseStep == 0 || (Model.CloseStep == 1 && (ViewData["IsUserHasRightsToBid"] != null && Convert.ToBoolean(ViewData["IsUserHasRightsToBid"])))));    
%>

<div class="details">
  <div class="leftdetails <%=AppHelper.CurrentUser != null && aui.IsRegisterForEvent ? "bdr" : "" %>">
    <div>
      <div class="span2">Reserve:</div>
      <div class="span1"><%=ashort.Price.GetCurrency()%></div>
    </div>
    <div class="clear"></div>

<%--<% 
    decimal est;
    decimal.TryParse(ashort.Estimate ?? string.Empty, out est);
    if (est > 0 && ashort.CurrentBid>=est)
    {%>
    <div>
        <div class="span2">
            Estimate:
        </div>
        <div class="span1">
            <%=est.GetCurrency() %>
        </div>     
    </div>
    <div class="clear"></div>
<%  } %>--%>
    
    <div>
      <div class="span2">Current Bid:</div>
      <div class="span1" id="tbCurrentBid"><%=ashort.CurrentBid.GetCurrency()%></div>
    </div>
    <div class="clear"></div>
    
    <div>
      <div class="span2">Number Of Bids:</div>
      <div class="span1" id="tbBids"><%=ashort.Bids>0?ashort.Bids.ToString():String.Empty%></div>
    </div>
    <div class="clear"></div>

<%  decimal est;
    if (decimal.TryParse(ashort.Estimate ?? string.Empty, out est))
    {%>
        <div style="color: #C00000;">
            <%=(est>0 && ashort.CurrentBid>0 && ashort.CurrentBid<est) ? "This item has not met reserve" : "Reserve has been met"%>
        </div>
        <div class="clear"></div>
    <% } %>
    
    <% if (IsUserHasRightsToBid && aui.IsRegisterForEvent){%>
      <span id="lnkRBR"><img src="<%=AppHelper.CompressImage("refresh.gif") %>" />refresh current bid</span>
      <span id="lnkRBR_loading"><img src="<%=AppHelper.CompressImage("ajax-loader_small.gif") %>" alt="" />&nbsp;refreshing current bid</span>
    
    <script type="text/javascript">$("#lnkRBR_loading").hide();</script>
  <%} %>
  </div>
  <div class="rightdetails <%=AppHelper.CurrentUser == null || aui.IsRegisterForEvent ? "" : "bdr" %>">
      <% if (AppHelper.CurrentUser == null) { %>
      <div class="custom_button" style="float:right">
        <button type="button" onclick="window.location='<%= Url.Action ("LogOn", "Account", new { controller = "Account", action = "LogOn" , returnurl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) } )%>'">Log In</button>
      </div>
    <% } else {    
    if (!aui.IsRegisterForEvent) {%>
          <div style="margin-left:20%">
            <a id="btn_ad_register" href="/Event/Register/<%=Model.LinkParams.ID %>" title="Click here to Enable Bidding"></a>
          </div>
          <p>
            <span class="ad_notif">Enable Bidding by clicking here. In order to place bids in this auction you have to enable your bidding.</span>
          </p>
    <%} else {
        if (IsUserHasRightsToBid)
        {%>
            <div style="margin-left:20%;margin-bottom:10px"><a id="btn_ad_placebid" href="#tbl_ad_bidding" title="Place bid"></a></div>
    
          <% if (!aui.IsInWatchList){ %>        
          <div style="margin-left:20%"><a id="btn_ad_watchlist" href="/Account/AddItemToWatchList/<%=Model.LinkParams.ID %>" title="Add this item to your Watch List"></a></div>
        <%} else Response.Write("<span class=\"ad_notif\">You are currenlty watching this lot in your</span> " + Html.ActionLink("Watch List", "WatchBid", "Account", new { controller = "Account", action = "WatchBid" }, new { @style = "font-weight:bold;color:#004;text-decoration:underline" }).ToHtmlString());%>
        <%}%>
        
   <% } %>
   <%} %> 
  </div>
</div>

<% if (Model.Status == Consts.AuctionStatus.Pending)
   { %>
      <div class="dateAuction">
        This is only an auction preview.<br/>The bidding for this auction begins on <%= Model.EventDateStart.ToString("g") %> EST.
      </div>
 <% }
   else
   { %>
<div class="dateAuction">
	Auction Date:<%= ashort.EndDate.ToString("g") %> EST
</div>
<% } %>
