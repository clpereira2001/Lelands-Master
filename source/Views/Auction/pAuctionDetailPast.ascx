<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionShort>" %>
<div class="details">
  <div class="leftdetails">
    <% 
     if (Model.Price < Model.PriceRealized || Model.PriceRealized == 0)
     { %>
        <div>
          <div class="span2">Reserve:</div>
          <div class="span1"><%=Model.Price.GetCurrency()%></div>
        </div>
        <div class="clear"></div>
       <%-- <% if (est > 0 && Model.CurrentBid > est) {%>
            <div>
                <div class="span2">
                    Estimate:
                </div>
                <div class="span1">
                    <%=est.GetCurrency() %>
                </div>     
            </div>
            <div class="clear"></div>
        <% } %>--%>
      <% } %>
    <div>
      <div class="span2">Winning Bid:</div>
      <div class="span1"><%=(!Model.IsUnsoldOrPulledOut) ? (Model.CurrentBid>0?Model.CurrentBid.GetCurrency():"&nbsp;"):"UNSOLD"%></div>
    </div>
    <div class="clear"></div>
    
    <% if (!Model.IsUnsoldOrPulledOut && Model.PriceRealized>0){ %>
      <div>
        <div class="span2">Price Realized:</div>
        <div class="span1"><%=(Math.Abs(Model.CurrentBid.GetValueOrDefault(0) * 1.195m - Model.PriceRealized)<1) ? Model.PriceRealized.GetCurrency() : (Model.CurrentBid * 1.195m).GetCurrency()%></div>
      </div>
      <div class="clear"></div>
    <%} %>
    <% if (Model.Bids>0){ %>
      <div>
        <div class="span2">Number Of Bids:</div>
        <div class="span1"><%=Model.Bids.ToString() %></div>
      </div>
      <div class="clear"></div>
    <%} %>
     <%
         decimal est;
         if (decimal.TryParse(Model.Estimate ?? string.Empty, out est)) {%>
        <div style="color: #C00000;">
            <%=((est>0 && Model.CurrentBid > 0 && Model.CurrentBid < est) ? "This item has not met reserve" : "Reserve has been met")%>
        </div>
        <div class="clear"></div>
   <%} %>
  </div>
</div>
<div class="dateAuction">
	Auction Date:<%=Model.EndDate.ToString("g")%> EST
</div>