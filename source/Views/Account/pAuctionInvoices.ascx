<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<LinkParams>>" %>

<% if (Model == null || Model.Count() == 0)
  Response.Write("You do not have any winning invoices from our past auction(s).");
else
{ %>
    Below is a list of auctions you have participated in. Auctions are listed in reverse order by begining date.<br />
    <div id="account_common" class="control" style="float: left">
      <div class="blue_box">
        <ul>
          <% foreach (LinkParams item in Model){%>
          <li>
            <%=Html.ActionLink(item.EventTitle, "InvoiceDetailed", new { controller = "Account", action = "InvoiceDetailed", id = item.ID, evnt=item.EventUrl }, new { @style = "font-size:14px" })%>
          </li>
          <% }%>
        </ul>
      </div>
    </div>
<%} %>