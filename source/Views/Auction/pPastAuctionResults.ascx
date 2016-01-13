<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Event>>" %>
  <% if (Model == null || Model.Count==0){ %>
      There is no past auctions
      <%}
       else
       {
         int i = 0;
     %>
    <table border="0" cellpadding="0" cellspacing="0">      
    <% int count = Model.Count;
       foreach (Event evnt in Model){                
       if (i%3 == 0)
         Response.Write("<tr>");
        i++;
         Response.Write((i%2 == 0)?"<td style=\"padding:10px 5px 10px 5px\" class=\"bordered\">" : "<td style=\"padding:10px 5px 10px 5px; background-color:#EFEFEF\" class=\"bordered\">");
      %>
      <span>
         <%=(count--) %>.          
         <%=(evnt.IsClickable) ? Html.ActionLink(evnt.Title, "AuctionResults", new { controller = "Auction", action = "PastAuctionResults", id = evnt.ID, evnt = evnt.UrlTitle }, new { @style = "font-size:14px; font-weight:normal" }).ToHtmlString() : "<b>" + evnt.Title + "</b>"%>    
       </span>
       <%=((String.IsNullOrEmpty(evnt.Description))?"":"<br />"+evnt.Description) %>
      <%      
      Response.Write("</td>");
      if (i%3 == 3){
        Response.Write("</tr>");
        i=0;       
      }
      } %>
    </table>
    <%} %>