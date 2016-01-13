<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<AuctionSales>>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="System.IO" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%= ((String.IsNullOrEmpty(ViewData["Title"] as string)) ? Consts.CompanyTitleName : ViewData["Title"] as string) %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
  <div id="left_column">
    <img alt="" width="173" height="574" src='<% = AppHelper.CompressImage("left_side_banner_1.jpg") %>' />
  </div>
</asp:Content>
<asp:Content ID="main" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <% var evnt = ViewData["Event"] as Event ?? new Event(); %>
    <div class="page_title" style="margin-bottom: 0;"><%= evnt.Title %> &gt;  Sales</div>
    <span style="font-size: 12px">All auctions end <b><%= evnt.DateEnd.ToString("G") %> EST</b></span>
    <br />
    <br />
    Page description
    <br />
    <br />
    <% if (!Model.Any())
       { %>
      <div>No auction found.</div>
    <% }
       else
       { %>
      <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <% var count = 0;
           foreach (var item in Model.OrderBy(t => t.Lot))
           {
             count++;
             var address = AppHelper.AuctionImage(item.LinkParams.ID, item.ThumbnailPath);
        %>
          <%= ((count%2 == 1) ? "<tr class='tbl_res_row' defcolor='#EFEFEF' defchanged='#FFA' style=\"background-color:#EFEFEF\" id='tbl_res_row_" + item.LinkParams.ID + "'>" : "<tr class='tbl_res_row' defcolor='#fff' defchanged='#FFA' id='tbl_res_row_" + item.LinkParams.ID + "'>") %>
          <td>
            <%
              if (File.Exists(Server.MapPath(address)))
              {
            %>
              <a style="float: left" href="<%= Url.Action("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.MainCategoryUrl, item.LinkParams.CategoryUrl, item.LinkParams.GetLotTitleUrl(item.Lot, item.Title)).Collect("page", "ViewMode")) %>">
                <img src="<%= AppHelper.CompressImagePath(address) %>" title="<%= String.Format("Lot{0}~{1}", item.Lot, item.Title.Replace("\"", "\'")) %>" alt="" /></a>
              <%= Html.ActionLink(item.Title, "AuctionDetail", new {controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.GetLotTitleUrl(item.Lot, item.Title)}, new {@class = "salesLink"}) %>
              <div class="salesDescription">
                <%= item.Description %>  
              </div>
            <%
              }
            %>
          </td>
          </tr>
        <% } %>
      </table>
    <% } %> 
  </div>
</asp:Content>