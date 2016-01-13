<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<PriceRealized>>" %>
<table style="table-layout: fixed; font-size:12px;">
      <colgroup>
        <col width="80px" />
        <col width="80px" />
        <col width="80px" />
        <col width="80px" />
        <col width="80px" />
        <col width="80px" />
      </colgroup>
      <%
        int elem_number = 0;
        foreach (PriceRealized item in Model)
        {
          if (elem_number == 0) Response.Write("<tr>");%>
          <% if (elem_number % 2 == 0)
             { %>
             <td class="bordered" style="background-color:#EFEFEF;">
          <%}
             else{ %>
          <td class="bordered">
          <%} %>
        <%=item.Lot %>. <%=Html.ActionLink(item.Price.GetCurrency(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }) %>         
          </td>
          <%
            elem_number++;
            if (elem_number >= 6) { Response.Write("</tr>"); elem_number = 0; }
        }
        if (elem_number > 0)
        {
          for (int j = elem_number; j< 6; j++) Response.Write("<td>&nbsp;</td>");
          Response.Write("</tr>");
        }       
        %>        
</table>