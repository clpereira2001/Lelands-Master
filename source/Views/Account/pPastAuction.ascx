<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Event>>" %>

<% if (Model == null || Model.Count == 0)
  Response.Write("We show no bidding history for you at this time.");
else
{ %>
  Below is a list of auctions you have participated in. Auctions are listed in reverse
  order by begining date.
  <br />
  <div id="account_common" class="control" style="float: left">
    <div class="blue_box">
      <ul>
        <% foreach (Event item in Model) {%>
        <li>
          <%=Html.ActionLink(item.Title, "AuctionsParticipated", new { controller = "Account", action = "AuctionsParticipated", id = item.ID, evnt=item.UrlTitle }, new { @style = "font-size:14px" })%>
        </li>
        <% 
         }%>
      </ul>
    </div>
  </div>
<%} %>

<%:Html.Hidden("cch_PastAuction", DateTime.Now) %>