<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="form_auction_bidding" title="Bids information" class="dialog_form">
  <%=Html.Hidden("form_auction_bidding_Auction_ID") %>
  <div id="form_auction_bidding_tabs" style="margin:0px">
    <ul>
      <li><a href="#form_auction_bidding_tabs-1">Bids</a></li>
      <li><a href="#form_auction_bidding_tabs-2">Bids Log</a></li>      
    </ul>
    <!-- Bids -->
    <div id="form_auction_bidding_tabs-1">
      <table id="form_auction_bidding_bids_list"></table>
      <div id="form_auction_bidding_bids_pager"></div>
   </div>
   <!-- Bids Log -->
    <div id="form_auction_bidding_tabs-2">
      <table id="form_auction_bidding_bidslog_list"></table>
      <div id="form_auction_bidding_bidslog_pager"></div>
    </div>   
  </div>
</div>