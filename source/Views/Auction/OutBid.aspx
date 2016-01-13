<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PreviewBid>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Sorry, you have been outbid- <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server"> 
<% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Sorry, you have been outbid.</div>
    <%  decimal StartBid = Model.Amount;        
        bool line=true;
        List<SelectListItem> BidAmountItems = new List<SelectListItem>();
        for (int i = 0; i < Consts.DropDownSize; i++)
        {          
          StartBid += Consts.GetIncrement(StartBid, false);
          if (!Model.IsProxy || (Consts.IsPriceBeforeLevel(StartBid) || line))
            BidAmountItems.Add(new SelectListItem { Value=StartBid.GetPrice().ToString(), Text = StartBid.GetCurrency() });
          line = Consts.IsPriceBeforeLevel(StartBid) ? false : !line;
        } 
    %>
    <% using (Html.BeginForm("AuctionDetail", "Auction")){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
    Sorry, You've been outbid. This is due to someone placing a higher maximum bid than
    yours, or a prior bid of the same amount, which takes precedence. If you would like
    to bid again, please use the form below:
    <br /><br />
    <table cellpadding="3" border="0" id="bids_table">
      <tr>
        <td class="bordered">
          Current Bid:          
        </td>
        <td class="bordered">
          <%=Model.Amount.GetCurrency() %>
        </td>
      </tr>      
      <tbody>
        <tr>
          <td class="bordered">
            Your New Bid:
          </td>
          <td class="bordered">
            <%=Html.DropDownList("BidAmount", BidAmountItems, new { @style = "width:150px" })%>
          </td>
        </tr>
         <tr>
          <td class="bordered">
            Proxy Bidding:
          </td>
          <td class="bordered">
            <%:Model.IsProxy?"Yes":"No" %>
          </td>
        </tr>
      </tbody>
    </table>
    <% =Html.Hidden("id",Model.LinkParams.ID) %>
    <% =Html.Hidden("ProxyBidding", Model.IsProxy) %>
    <% =Html.Hidden("Quantity", Model.Quantity )%>
    <% =Html.Hidden("OutBidAmount", Model.Amount )%>
    <div class="custom_button" style="width:100px; float:left;//padding-right:0px">
        <button type="submit" style="padding-left:0px">Preview Bid</button>
      </div>
      <div style="float:left; width:30px;//width:50px;">&nbsp;</div>    
      <div class="custom_button" style="width:100px; float:left;">
        <button type="button" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl })%>'">Cancel Bid</button>
      </div>
      
    <br /><br /><br />
    <center>
      <%= Html.ActionLink("Back to Lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl })%>
      |
      <%= Html.ActionLink("Back to Category", "CategoryView", new { controller = "Auction", action = "CategoryView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl })%>
    </center>    
    <%
      }            
    %>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>
