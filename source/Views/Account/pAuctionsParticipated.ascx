<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IPagedList<MyBid>>" %>

<%@ Import Namespace="Vauction.Utils.Paging" %>

Note: If the price is <span style="color: green; font-weight: bold">green</span> then you are the high bidder of this item.
<br /><br />
<% GeneralFilterParamsEx filterParams = (GeneralFilterParamsEx)ViewData["filterParamEx"];%>
<div style="vertical-align: middle; font-size: 12px; float: right">Show image(s):
  <% if (Convert.ToBoolean(filterParams.ImageViewMode)){ %>
      <input type="checkbox" id="cbImages" checked />
  <%} else { %> 
       <input type="checkbox" id="cbImages" />
  <%} %>
</div>
<div class="pager_pos" style="vertical-align: middle">
  <%if (Model.TotalItemCount > Model.PageSize)
    { %>
  <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
  <%} %>
</div>
<br />

<%  string address;
    int count = 0;
  if (Model ==null || !Model.Any())
    Response.Write("You have no bids in this auction.");
  else{%>
  <table id="item_view" border="0" cellpadding="0" cellspacing="0"  width="100%" style="table-layout:fixed" >
    <colgroup>
        <col width="30px" />
        <col width="250px" />
        <col width="120px" />
        <col width="90px" />
        <col width="90px" />        
        <col width="90px" />
        <col width="90px" />
        <col width="30px" />
      </colgroup>
    <tr style="background-color:#D2D2D2">
      <th style="padding-left:5px;">Lot</th>
      <th>Title</th>
      <th>Date</th>
      <th>Bid Amount</th>
      <th>Max. Bid</th>      
      <th>Winning Bid</th>
      <th>Price Realized</th>  
      <th>Bids</th>       
    </tr>
   <%
    foreach (var item in Model)
      {
        count++;          
    %>
    <%=((count % 2 == 1) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr>")%>
<% if (item.AuctionStatus != 5)
   {
      %>
        <td>
        <%=Html.ActionLink(item.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.ID, evnt = item.PriceRealized.LinkParams.EventUrl, maincat = item.PriceRealized.LinkParams.MainCategoryUrl, cat = item.PriceRealized.LinkParams.CategoryUrl, lot = item.PriceRealized.LinkParams.GetLotTitleUrl(item.Lot, item.Title) })%>
        </td>
        <td>        
        <%=Html.ActionLink(item.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.ID, evnt = item.PriceRealized.LinkParams.EventUrl, maincat = item.PriceRealized.LinkParams.MainCategoryUrl, cat = item.PriceRealized.LinkParams.CategoryUrl, lot = item.PriceRealized.LinkParams.GetLotTitleUrl(item.Lot, item.Title) })%>
        </td>        
        <%
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.DateMade));    
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.Amount.GetCurrency()));
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.MaxBid.GetCurrency()));          
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.WinBid.GetCurrency()));          
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.IsUnsold ? "UNSOLD" : item.Cost.GetCurrency()));
          Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.BidsCount>0?item.BidsCount.ToString():string.Empty));
   }
   else
   {
       %>
<td><%=item.Lot%></td>
<td><%=item.Title %></td>
<td colspan="6">THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION</td>
    <% 
   } %>
    </tr>    
    <%    
      if (item.AuctionStatus==5 || !Convert.ToBoolean(filterParams.ImageViewMode)) continue;
      address = AppHelper.AuctionImage(item.ID, item.ThubnailImage);
      if (!System.IO.File.Exists(Server.MapPath(address))) continue;
    %>    
    <%=((count % 2 == 1) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr>")%>
      <td colspan="8">
        <a href="<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", item.ID).Collect("page", "ViewMode"))%>">
          <img src="<%=AppHelper.CompressImagePath(address) %>" alt="" />
        </a>
      </td>      
    </tr>
    <%     
    }
   }
   %>
</table>

<%if (Model.TotalItemCount > Model.PageSize)
  { %>
    <div class="pager_pos" style="vertical-align: middle">
      <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
    </div>
<%} %>

<script src="<%=AppHelper.ScriptUrl("jquery.cookie.js") %>" type="text/javascript"></script>
<script type="text/javascript">
  $(document).ready(function () {
    $("select.span-3.viewselect").change(function (e) {
      $.cookie("ViewMode", this.value, { path: "/" })
      location.reload();
    })
    $("#cbImages").click(function (e) {
      $.cookie("ImageViewMode", ($(this).attr("checked")) ? 1 : 0, { path: "/" })
      location.reload();
    })
  });
</script>