<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<AuctionSales>>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="System.IO" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%= ViewData["Title"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
    <div id="left_column">
        <img alt="" width="173" height="574" src='<% = AppHelper.CompressImage("left_side_banner_1.jpg") %>' />
    </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
    <% var evnt = ViewData["Event"] as Event; %>
    <div class="center_content">
        <div class="page_title" style="margin-bottom: 0;"> <% = evnt.Title %></div>
        <span style="font-size: 12px">
            <%= evnt.Description %>
        </span><br /><br />    
        <% if (Model == null || !Model.Any())
           { %>
            <div style="font-size: 12px">No auction found.</div>
        <% }
           else
           { %>
            <table id="item_view" border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr style="background-color: #D2D2D2">
                    <th style="padding-left: 10px; width: 30px">
                        Lot
                    </th>
                    <th>
                        Title
                    </th>
                    <th style="width: 120px">
                        Price
                    </th>
                </tr>
                <% var count = 0;
                   foreach (var item in Model.OrderBy(t => t.Lot))
                   {
                       count++;
                       var address = AppHelper.AuctionImage(item.LinkParams.ID, item.ThumbnailPath);
                %>
                    <%= ((count%2 == 1) ? "<tr class='tbl_res_row' defcolor='#EFEFEF' defchanged='#FFA' style=\"background-color:#EFEFEF\" id='tbl_res_row_" + item.LinkParams.ID + "'>" : "<tr class='tbl_res_row' defcolor='#fff' defchanged='#FFA' id='tbl_res_row_" + item.LinkParams.ID + "'>") %>
                    <td style="padding-left: 10px; width: 30px;">
                        <%= item.Lot %>
                    </td>
                    <td>
                        <%= Html.ActionLink(item.Title, "Details", new {controller = "Sales", action = "Details", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title)}) %>
                    </td>
                    <td>
                        <%
                            if (!string.IsNullOrWhiteSpace(item.Estimate))
                            {
                                decimal estimate;
                                decimal.TryParse(item.Estimate, out estimate);
                        %>
                            <%= estimate > 0 ? estimate.GetCurrency() : item.Estimate %>
                        <%
                            }
                            else
                            { %>
                            <%= item.Price.GetCurrency() %>
                        <% } %>
           
                    </td>
                    </tr>
                    <%
                        if (File.Exists(Server.MapPath(address)))
                        {
                    %>
                        <%= ((count%2 == 1) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr>") %>
                        <td colspan="3">
                            <a style="float: left" href="<%= Url.Action("Details", "Sales", new CollectParameters("Sales", "Details", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.MainCategoryUrl, item.LinkParams.CategoryUrl, item.LinkParams.GetLotTitleUrl(item.Lot, item.Title)).Collect("page", "ViewMode")) %>">
                                <img src="<%= AppHelper.CompressImagePath(address) %>" title="<%= String.Format("Lot{0}~{1}", item.Lot, item.Title.Replace("\"", "\'")) %>" alt="" /></a>
                        </td>
                        </tr>
                    <%
                        }
                    %>
                <% } %>
            </table>
        <% } %> 
    </div>
</asp:Content>