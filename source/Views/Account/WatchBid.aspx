<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<UserBidWatch>>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Watch List - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div style="padding:10px;font-size:12px">
    <div class="page_title">Watch List</div>
   <%
      if (Model == null || Model.Count() < 1)
      { %>      
        There are currently no items in your Watch List. <br />      
        If you would like to add items to your Watch List, view an item and press the "Add this item to your Watch List" button or place a bid.
    <%} else { %>
    Your watch list shows items you are watching.<br />
    <table cellspacing="0" cellpadding="3" border="0">
      <tbody>
        <tr>
          <td align="left" style="width:50px">
            <font color="black">Black</font>
          </td>
          <td>
            - you are watching this item
          </td>
        </tr>
        <tr>
          <td align="left">
            <font color="red">Red</font>
          </td>
          <td>
            - you are not currently the high bidder
          </td>
        </tr>
        <tr>
          <td align="left">
            <font color="green">Green</font>
          </td>
          <td>
            - you are currently the high bidder
          </td>
        </tr>
      </tbody>
    </table>
    
    <div style="font-size:12px;">
    <table id="tblWatchList" style="table-layout: fixed" cellpadding="0" cellspacing="0">
      <colgroup>
        <col width="50px" />
        <col width="210px" />
        <col width="80px" />        
        <col width="80px" />
        <col width="80px" />
        <col width="80px" />        
        <col width="50px" />  
      </colgroup>
      <tr style="background-color: #D2D2D2" class="bordered">
        <th style="padding-left: 5px;">
          Lot
        </th>
        <th style="padding-left: 5px;">
          Title
        </th>     
        <th style="padding-left: 5px;">
          Current Bid
        </th>
        <th style="padding-left: 5px;">
          Bids
        </th>
        <th style="padding-left: 5px;">
          Your Bid
        </th>
        <th style="padding-left: 5px;">
          Your Max Bid
        </th>               
        <th style="padding-left: 5px;">
          &nbsp;
        </th>
      </tr>
      <%      
      foreach (UserBidWatch item in Model)
      {
        %>
      <tr auction_id="<%=item.LinkParams.ID %>">
          <%
              if (item.AuctionStatus != 5)
              { %>
        <td style="padding-left: 5px;">
        <%= Html.ActionLink(item.LinkParams.Lot.ToString(), "AuctionDetail", new {controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl}) %>
      </td>
      <td>
        <%= Html.ActionLink(item.LinkParams.Title, "AuctionDetail", new {controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, maincat = item.LinkParams.MainCategoryUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl}) %>
      </td>    
      <%
          switch (item.Option)
          {
              case 0:
                  Response.Write("<td style=\"color:red\">" + item.CurrentBid.GetCurrency() + "</td>");
                  break;
              case 1:
                  Response.Write("<td style=\"color:green\">" + item.CurrentBid.GetCurrency() + "</td>");
                  break;
              default:
                  Response.Write("<td>" + (!item.HasBid ? String.Empty : item.CurrentBid.GetCurrency()) + "</td>");
                  break;
          }
      %>
         <%
          switch (item.Option)
          {
              case 0:
                  Response.Write("<td style=\"color:red\">" + item.Bids.ToString() + "</td>");
                  break;
              case 1:
                  Response.Write("<td style=\"color:green\">" + item.Bids.ToString() + "</td>");
                  break;
              default:
                  Response.Write("<td>" + (!item.HasBid ? String.Empty : item.Bids.ToString()) + "</td>");
                  break;
          }
         %>
       <%
          switch (item.Option)
          {
              case 0:
                  Response.Write("<td style=\"color:red\">" + item.Amount.GetCurrency() + "</td>");
                  break;
              case 1:
                  Response.Write("<td style=\"color:green\">" + item.Amount.GetCurrency() + "</td>");
                  break;
              default:
                  Response.Write("<td>&nbsp;</td>");
                  break;
          }
       %>    
        <%
          switch (item.Option)
          {
              case 0:
                  Response.Write("<td style=\"color:red\">" + item.MaxBid.GetCurrency() + "</td>");
                  break;
              case 1:
                  Response.Write("<td style=\"color:green\">" + item.MaxBid.GetCurrency() + "</td>");
                  break;
              default:
                  Response.Write("<td>&nbsp;</td>");
                  break;
          }
        %>
      <td>        
        <%= (item.Option != 2) ? "&nbsp;" : "<span auction_id='" + item.LinkParams.ID + "' class=\"span_link\" >remove</span>" %>
      </td>
                   <%
              }
              else
              {
                 %>
        <td style="padding-left: 5px;"><%=item.LinkParams.Lot %></td>
     <td><%=item.LinkParams.Title %></td>
<td colspan="4">
    THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION
</td>
<td>        
        <span auction_id='<% = item.LinkParams.ID %>' class="span_link">remove</span>
      </td>
      <% 
              }
 }%>
      </tr>
    </table>
    </div>
  <% }%>
</div>
</asp:Content>

<asp:Content ID="jsC" runat="server" ContentPlaceHolderID="cphJS">
<script type="text/javascript">
  var removed_id_row = null;
  $(document).ready(function () {
    $("#bids_table tr:even").css("background-color", "#d9e9f0");
    $(".span_link").click(function () {
      removed_id_row = $("#tblWatchList tr[auction_id=" + parseInt($(this).attr("auction_id")) + "]");
      if (confirm('Do you really want to remove this item from your Watch list?')) {
        $.post('/Account/RemoveBid', { id: parseInt($(this).attr("auction_id")) }, function (data) {          
          if (data == "1")
            removed_id_row.remove();
          removed_id_row = null;
        }, 'json');
      }
    });
  });
</script>
</asp:Content>