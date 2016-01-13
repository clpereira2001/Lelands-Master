<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<AuctionShort>>" %>
<% if (Model.Count() == 0)
   {%>
<div>
  No auction found matching your criteria.</div>
<% }
   else
   {
%>
<table id="table_grid" cellpadding="0" cellspacing="0">
  <colgroup>
    <col width="185px" />
    <col width="185px" />
    <col width="185px" />
    <col width="185px" />
  </colgroup>
  <%     
    int i = 0;
    foreach (AuctionShort item in ViewData.Model)
    {
      if (i % 4 == 0)
        Response.Write("<tr>");
      i++;
      Response.Write("<td style=\"vertical-align:bottom;//padding:0px;\">");
  %>
  <% Html.RenderPartial("~/Views/Auction/_ItemView.ascx", item); %>
  <%      
    Response.Write("</td>");
    if (i % 4 == 4)
    {
      Response.Write("</tr>");
      i = 0;
    }
    } %>
</table>
<% }%>