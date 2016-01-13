<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PreviewBid>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Preview Bid - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server"> 
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content">    
    <% bool yes = true; %>
    <div class="page_title">Preview Bid for Lot #<%=Model.LinkParams.Lot %>: <%=Model.LinkParams.Title %></div>    
    <% using (Html.BeginForm("PlaceBid", "Auction", FormMethod.Post)){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
    <table cellpadding="3" border="0" id="bids_table">
      <colgroup><col width="400px" /><col width="175px" /></colgroup>
      <tbody>
        <tr>
          <td class="bordered">
            Your Maximum Bid:
          </td>
          <td class="bordered">
            <%=Model.Amount.GetCurrency()%>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Bid Amount:
          </td>
          <td class="bordered">
            <%=Model.RealAmount.GetCurrency()%>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Quantity:
          </td>
          <td class="bordered">
            <%=Model.Quantity%>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Use Proxy Bidding:
          </td>
          <td class="bordered">
            <%=Model.IsProxy ? "Yes" : "No"%>
          </td>
        </tr>
        <tr style="background-color: #cccccc;">
          <td class="bordered">
            <b>Total Bid Amount:</b>
          </td>
          <td class="bordered">
            <b><%=Model.TotalRealAmount.GetCurrency()%></b>
          </td>
        </tr>
      </tbody>
    </table>
    <% =Html.Hidden("BidAmount", Model.Amount)%>
    <% =Html.Hidden("id", Model.LinkParams.ID)%>
    <% =Html.Hidden("ProxyBidding", Model.IsProxy)%>
    <% =Html.Hidden("Quantity", Model.Quantity)%>    
    <% =Html.Hidden("RealBidAmount", Model.RealAmount)%>

    <% if (Model.Amount<Model.RealAmount || Model.IsOutBid)
       { %>
       Sorry, but you are not allowed to make this bid, because the minimum bid amount is greater than yours bid amount. <br /><br />
    <%}
       else
       {%>    
      <% if (!Model.IsUpdate)
         {%>
           If the above information is incorrect, use the "CANCEL BID" button to return to the item and correct your bid. Once you hit "PLACE BID" the bid is final and you cannot retract your bid
            <br /><br />
      <% }
         else
         {
      %>
      You are currently the high bidder for this item with:
      <br />
      <br />
      Your Current Bid: <%=Model.PreviousAmount.GetCurrency()%>
      <% if (Model.PreviousMaxBid > 0)
         { %>
      <br />
            Maximum bid of <%=Model.PreviousMaxBid.GetCurrency()%>
      <% }
         else
         {%>
      <br />
            Maximum bid of <%=Model.PreviousAmount.GetCurrency()%>
      <%} %>
      <br />
      <br />
      <% if ((Model.Amount != Model.PreviousMaxBid || Model.RealAmount != Model.PreviousAmount))
         {%>       
          <% if (Model.Amount > Model.PreviousMaxBid)
             {%> Please confirm if you would like to raise your maximum bid to <%=Model.Amount.GetCurrency()%><%}
             else
             { %> 
             <b>Note: </b>Placing this bid does not change your maximum bid of <%=Model.PreviousMaxBid.GetCurrency()%>, it will only increase your current bid from <%=Model.PreviousAmount.GetCurrency()%> to <%=Model.RealAmount.GetCurrency()%>. 
             <br /><br /> Please confirm if you would like to raise your bid amount to <%=Model.RealAmount.GetCurrency()%><%} %>          
       <%}
         else
         {
           yes = false; %> Sorry, but you are not allowed to make this bid, because it will not raise your maximum bid or bid amount. <%}
         } %>
         <br />
         <% if (yes)
            { %>
       <button type="button" class="cssbutton small white place_bid" id="btnPlace">
        <span>Place Bid</span>
      </button>
     
      <input type="submit" id="btnSubmit" style="display:none" />      
      <div id="dvAnimation" style="color:rgb(180,0,0);text-align:center">
        <br />
        <strong>Please wait while your request is processed.</strong>
        <br /><br />
        <img src="<%=AppHelper.CompressImage("ajax-loader.gif") %>" alt="" />
      </div>
      <script type="text/javascript">$("#dvAnimation").hide();</script>
      <%}
       }%>
      <button type="button" class="cssbutton small white" style="//width:150px" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl })%>'">
      <span>Cancel Bid</span>
    </button>
    <% } %>
  </div>
  <script type="text/javascript">    
    $("#btnPlace").click(function () {
      $(".cssbutton").hide();
      $("#dvAnimation").show();
      $("#btnSubmit").click();
    });    
  </script>  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="cphJS" runat="server">  
  <%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_2.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>