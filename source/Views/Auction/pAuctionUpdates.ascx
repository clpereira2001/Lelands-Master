<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<AuctionUpdate>>" %>
<% 
if (Model == null || Model.Count == 0)
  Response.Write("There are no updates for this auction");
else
{%>
  <table style="table-layout: fixed;margin-left:0px;font-size:14px">
    <colgroup>
      <col width="50px" />
      <col width="350px" />
      <col width="520px" />      
    </colgroup>
    <tr style="background-color:#D2D2D2" class="bordered">
      <th style="padding-left:5px;">Lot</th>
      <th style="padding-left:5px;">Title</th>
      <th style="padding-left:5px;">Addendum</th>      
   </tr>    
      <%        
      bool line = true;
      foreach (AuctionUpdate item in Model)
      { %>
        <%=(line) ? "<tr class=\"bordered\" style=\"background-color:#EFEFEF;\">" : "<tr class=\"bordered\">"%>
        <td style="padding-left:5px;"><%=item.Lot%></td>
        <td>
        <%=(item.IsPulledOut)?item.Title : Html.ActionLink(item.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title) }, new { @style = "color:black;font-size:14px" }).ToHtmlString() %>
        </td>
        <td style="color:#FA3232;"><%=(item.IsPulledOut) ? "THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION <br/>"+item.Addendum : item.Addendum %></td>
        <%line = !line;
      }    
        %>
    </table>
<%} %>