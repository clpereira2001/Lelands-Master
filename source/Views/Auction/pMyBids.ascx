<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<MyBid>>" %>
    <% if (Model == null || Model.Count() == 0)
         Response.Write("You did not place the bid at this auction");       
       else
       {%>    
    Note: If the record in the table is in <span style="color:green">green</span> that means you are the current high bidder<br />
    <table style="table-layout: fixed">
      <colgroup>
        <col width="50px" />
        <col width="270px" />
        <col width="120px" />
        <col width="90px" />
        <col width="90px" />        
        <col width="90px" />
        <col width="90px" />
      </colgroup>
      <tr style="background-color:#D2D2D2" class="bordered">
        <th style="padding-left:5px;">Lot</th>
        <th style="padding-left:5px;">Title</th>
        <th style="padding-left:5px;">Date</th>
        <th style="padding-left:5px;">Bid Amount</th>
        <th style="padding-left:5px;">Maximum Bid</th>        
        <th style="padding-left:5px;">Winning Bid</th>        
        <th style="padding-left:5px;">Realized Price</th>
     </tr>
    <% bool line = true;
       foreach (MyBid item in Model)
       {%>
      <%=((line) ? "<tr style=\"background-color:#EFEFEF\" class=\"bordered\">" : "<tr class=\"bordered\">")%>
      <td style="padding-left:5px;"><%=Html.ActionLink(item.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.ID, evnt = item.PriceRealized.LinkParams.EventUrl, maincat = item.PriceRealized.LinkParams.MainCategoryUrl, cat = item.PriceRealized.LinkParams.CategoryUrl, lot = item.PriceRealized.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }, new { @style = "color:black;font-size:14px" })%></td>
      <td><%=Html.ActionLink(item.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.ID, evnt = item.PriceRealized.LinkParams.EventUrl, maincat = item.PriceRealized.LinkParams.MainCategoryUrl, cat = item.PriceRealized.LinkParams.CategoryUrl, lot = item.PriceRealized.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }, new { @style = "color:black;font-size:14px" })%></td>
      <%
        Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.DateMade));
         Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner?"style=\"color:green\"":String.Empty,item.Amount.GetCurrency()));
         Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.MaxBid.GetCurrency()));         
         Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.WinBid.GetCurrency()));         
         Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.IsUnsold?"UNSOLD":item.Cost.GetCurrency()));
        line = !line;
       }%>
    </table>
    <%} %>