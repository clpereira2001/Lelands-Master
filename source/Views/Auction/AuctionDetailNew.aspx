<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>

<asp:Content ID="cH" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%:String.Format("Lot {0}. {1} ~ {2} < {3} < {4} - {5}", Model.Lot, Model.Title, Model.LinkParams.CategoryTitle, Model.LinkParams.MainCategoryTitle, Model.LinkParams.EventTitle, Consts.CompanyTitleName)%></title>
 <%-- <% Html.Style("jQueryControls.css"); %>
  <% Html.Style("jquery.lightbox-0.5.css"); %>
  <% Html.Script("jquery.lightbox-0.5.min.js"); %>
  <% Html.Style("cloud-zoom.1.0.2_custom.css"); %>
  <% Html.Script("cloud-zoom.1.0.2_1_custom.js"); %>  --%>
  <link href="/public/css/auctiondetail.css" rel="stylesheet" />
  <script src="/public/scripts/jquery-1.8.3.min.js" type="text/javascript"></script>
  <script src="/public/scripts/jquery.royalslider.min.js" type="text/javascript"></script>
  <link href="/public/css/royalslider/royalslider.css" rel="stylesheet" />
  <link href="/public/css/royalslider/skins/minimal-white/rs-minimal-white.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="cM" ContentPlaceHolderID="MainContent" runat="server">

  <% SessionUser cuser = AppHelper.CurrentUser;%>
  <div class="content_new">

    <div class="topPanel">
      <div class="breadcrumb">
        <%= Html.ActionLink(Model.LinkParams.EventTitle, Model.IsCurrentEvent ? "Category" : "AuctionResults", new { controller = "Auction", action = Model.IsCurrentEvent ? "Category" : "AuctionResults", id = Model.LinkParams.Event_ID, evnt = Model.LinkParams.EventUrl }, new { @style = "font-size:14px" })%>
    &nbsp;>&nbsp;
    <%=(Model.IsCurrentEvent) ? Html.ActionLink(Model.LinkParams.MainCategoryTitle, "CategoryView", new { controller = "Auction", action = "CategoryView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl }, new { @style = "font-size:14px" }).ToHtmlString() : Model.LinkParams.MainCategoryTitle%>
    &nbsp;>&nbsp;
    <%=Html.ActionLink(Model.LinkParams.CategoryTitle, Model.IsCurrentEvent ? "CategoryView" : "PastCategoriesView", new { controller = "Auction", action = Model.IsCurrentEvent ? "CategoryView" : "PastCategoriesView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl }, new { @style = "font-size:14px" })%>
      </div>
      <div class="navigation">
        <% if (Model.PrevAuction != null)
           { %>
        <%=Html.ActionLink("< previous lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl })%>
        <%} %>
        <%= (Model.PrevAuction != null && Model.NextAuction != null) ? "&nbsp;|&nbsp" : String.Empty%>
        <% if (Model.NextAuction != null)
           { %>
        <%=Html.ActionLink("next lot >", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl })%>
        <%} %>
      </div>
    </div>
    
    <div>
    <div class="leftcolumn">
      <% if (Model.IsPulledOut)
         { %>
            <img src="<%=AppHelper.CompressImage("withdrawn.jpg") %>" alt="" />
      <% }
         else
         { %>
          <% Html.RenderAction("pAuctionDetailImages", "Auction", new {auction_id = Model.LinkParams.ID}); %> 
      <% } %>
    </div>

    <div class="rightcolumn">
      <span class="lot_title">Lot <%= Model.Lot%>: <span style="text-transform: uppercase"><%= Model.Title%></span></span>

      <% if (Model.IsPulledOut) { %>
        <div class="pay_notice">NOTE:THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION</div>
        <div class="pay_notice"><%=Model.Addendum %></div>
      <% } else { %>

        <%if (Model.Status == Consts.AuctionStatus.Closed)
          Html.RenderAction("pAuctionDetailPast", "Auction", new { auction_id = Model.LinkParams.ID });
        else
          Html.RenderPartial("~/Views/Auction/pAuctionDetailCurrent.ascx", Model);%>
      
        <% foreach (var collection in Model.Collections)
        {
          %>
          <div class="itemdescription">
            <strong><%=collection.Title %></strong>
          </div>
          <div class="itemdescr">
            <%=collection.Description %>
          </div>
        <%
        } %>
        <div class="itemdescription">
	  		  <strong>Description</strong>
            <span class="productSocial addthis_toolbox addthis_default_style addthis_16x16_style">
              <a class="addthis_button_email"></a>
              <a class="addthis_button_facebook"></a>
              <a class="addthis_button_twitter"></a>
              <a class="addthis_button_print"></a>
              <a class="addthis_button_pinterest_pinit"></a>
            </span>
  		  </div>
      <% if (!String.IsNullOrEmpty(Model.Addendum))
         { %>
         <div class="pay_notice"><strong>UPDATE:</strong>&nbsp;<%=Model.Addendum%></div>
        <%}%>

        <div class="itemdescr">
        
          <% Html.RenderPartial("~/Views/Auction/pAuctionDetailAdds.ascx", Model); %>
          <%=Model.Description%>
        </div>
      <% } %>
    </div>
    
    </div>
    
    
    <div class="clear">
      
    <% if (!Model.IsPulledOut)
       { %>
    <!-- BIDDING -->
    <% if (Model.Status != Consts.AuctionStatus.Locked && Model.Status != Consts.AuctionStatus.PulledOut)
       { %>
       <% AuctionUserInfo aui = (ViewData["AuctionUserInfo"] as AuctionUserInfo) ?? new AuctionUserInfo(); %>
        <div class="bidding_title">
	  		  Bidding
        </div>
    <table id="tbl_ad_bidding" cellpadding="0" cellspacing="0">    
        <% if (Model.Status == Consts.AuctionStatus.Pending)
           { %>        
          <tr>
            <td class="bordered">
              This is only an auction preview. The bidding for this auction begins on <%= Model.EventDateStart.ToString("g") %> EST. <br class="clear" />
            </td>
          </tr>
          <% if (cuser == null)
             {
               Response.Write("<tr><td class=\"bordered\">" + Html.ActionLink("Log In", "LogOn", new {controller = "Account", action = "LogOn", @returnurl = Url.Action("AuctionDetail", "Auction", new {id = Model.LinkParams.ID})}).ToHtmlString() + "</td></tr>");
             }
             else
             {
               if (!aui.IsRegisterForEvent)
               { %>
          <tr>
            <td class="bordered">              
              You must be registered for this event before you can bid.<br />
              <%= Html.ActionLink("Click here to register for this event", "Register", "Event", new {controller = "Event", action = "Register", id = Model.LinkParams.ID}, new {@style = "font-weight:bold;font-size:14px"}) %>              
            </td>
          </tr>
          <% }
               else
               { %>
          <tr>
            <td class="bordered">              
              <% Response.Write(!aui.IsInWatchList ? Html.ActionLink("Add this item to your Watch List", "AddItemToWatchList", "Account", new {controller = "Account", action = "AddItemToWatchList", id = Model.LinkParams.ID}, new {@style = "font-weight:bold;font-size:14px"}).ToHtmlString() : ("You are currenlty watching this lot in your " + Html.ActionLink("WATCH LIST", "WatchBid", "Account", new {controller = "Account", action = "WatchBid"}, new {@style = "font-weight:bold;font-size:14px;text-decoration:underline"}).ToHtmlString())); %>
            </td>
          </tr>
          <% }
             } %>
        <% }
           else
           {
             if (Model.Status == Consts.AuctionStatus.Closed)
               Response.Write("<tr><td class=\"bordered\">The bidding for this lot has ended.</td></tr>");
             else
             {
               if (cuser == null)
               {
                 Response.Write("<tr><td class=\"bordered\">" + Html.ActionLink("Log In", "LogOn", new {controller = "Account", action = "LogOn", @returnurl = Url.Action("AuctionDetail", "Auction", new {id = Model.LinkParams.ID})}).ToHtmlString() + "</td></tr>");
               }
               else
               {
                 bool UserHasRightsToBid = Model.Status == Consts.AuctionStatus.Open && (Model.CloseStep == 0 || (Model.CloseStep == 1 && (ViewData["IsUserHasRightsToBid"] != null && Convert.ToBoolean(ViewData["IsUserHasRightsToBid"]))));
                 if (!aui.IsRegisterForEvent)
                   Response.Write("<tr><td class=\"bordered\">You must be registered for this event before you can bid.<br />" + Html.ActionLink("Click here to register for this event", "Register", "Event", new {controller = "Event", action = "Register", id = Model.LinkParams.ID}, new {@style = "font-weight:bold;font-size:14px"}).ToHtmlString() + "</td></tr>");
                 else
                 { %>
                <% if (UserHasRightsToBid)
                   { %>
              <tr>
                <td class="bordered">Contact Lelands if you have any questions before bidding. By bidding you agree to all Terms and Conditions and are entering into a legally binding Contract.<br />
                  <span class="bid_info"><b>19.5% buyers premium will be added to all winning bids for this auction. Please bid accordingly.</b></span>
                </td>
              </tr>
              <% } %>              
              <tr>
                <td class="bordered">
                 <% if (UserHasRightsToBid)
                      Html.RenderPartial("~/Views/Auction/pBiddingPanel.ascx", Model);
                    else Response.Write("You can not participate in bidding on this lot. The bidding for this lot has ended."); %>
                </td>
              </tr>
            <% }
               }
             }
           }
            %>
        </table>
      <% } %>
    </div>
    <% } %>
    

    <center>
    <% if (Model.PrevAuction != null)
       { %>
      <%=Html.ActionLink("< previous lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl })%>
    <%} %>
    <%= (Model.PrevAuction != null && Model.NextAuction != null) ? "&nbsp;|&nbsp" : String.Empty%>
    <% if (Model.NextAuction != null)
       { %>
      <%=Html.ActionLink("next lot >", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl })%>
    <%} %>
    </center> 
  </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphJS" ID="cJ" runat="server">
  <script type="text/javascript"> var addthis_config = { "data_track_addressbar": false }; </script>
  <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-51a5d4c65778a843"></script>
  <script type="text/javascript">    
    $(document).ready(function () {
      $("#lnkRBR").click(function () {        
        $("#lnkRBR").hide();
        $("#lnkRBR_loading").show();
        $.post('/Auction/UpdateAuctionResult', {id:<%=Model.LinkParams.ID %>}, function (data) {            
          $("#tbCurrentBid").html(data.currentbid);
          $("#tbBids").html(data.bids);
          $("#tbMinBid").html(data.minbid);
          $("#lnkRBR_loading").hide();
          $("#lnkRBR").show();
        }, 'json');        
      });
    });
  </script>
</asp:Content>