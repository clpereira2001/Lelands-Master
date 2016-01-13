<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>
<asp:Content ID="cH" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%:String.Format("Lot {0}. {1} ~ {2} < {3} < {4} - {5}", Model.Lot, Model.Title, Model.LinkParams.CategoryTitle, Model.LinkParams.MainCategoryTitle, Model.LinkParams.EventTitle, Consts.CompanyTitleName)%></title>
  <% Html.Style("jQueryControls.css"); %>
  <% Html.Style("jquery.lightbox-0.5.css"); %>
  <% Html.Script("jquery.lightbox-0.5.min.js"); %>
  <% Html.Style("cloud-zoom.1.0.2_custom.css"); %>
  <% Html.Script("cloud-zoom.1.0.2_1_custom.js"); %>  
</asp:Content>
<asp:Content ID="cL" ContentPlaceHolderID="leftImg" runat="server">
  <script type="text/javascript" >
  $(function () {
    $("#fragment-a").lightBox({
      overlayOpacity: 0.6,
      imageLoading: '<%=AppHelper.CompressImage("lightbox-ico-loading.gif") %>',
      imageBtnClose: '<%=AppHelper.CompressImage("lightbox-btn-close.gif") %>',
      containerResizeSpeed: 350
    });
  });
  </script>
 <div id="left_column">
  <% if (!Model.IsCurrentEvent){ %> 
      <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");   %>
      <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
      <% if (!isIE6)
         { %>
      <script type="text/javascript">
        $(document).ready(function () {
          $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_2.jpg") %>');
        });
  </script>
  <%} %>
  <%} else { %>    
    <% Html.RenderAction("pCategoryTree", "Auction", new { event_id=Model.LinkParams.Event_ID, maincategory_id =Model.LinkParams.MainCategory_ID });%>  
  <% } %>
 </div>
</asp:Content>

<asp:Content ID="cM" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content">
<% SessionUser cuser = AppHelper.CurrentUser; 
  bool test = false;
    if (test)
    {
      Html.RenderPartial("~/Views/Auction/_Test_BiddingPanel.ascx", Model);
    }
    else
    { %>
  <span class="addthis_toolbox addthis_default_style" style="float: right; padding-top: 20px">
    <a href="http://www.addthis.com/bookmark.php?v=250&amp;username=clpereira" class="addthis_button_compact"></a>          
    <a class="addthis_button_facebook"></a>
    <a class="addthis_button_email"></a>
    <a class="addthis_button_favorites"></a>
    <a class="addthis_button_print"></a>
  </span>
  <script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#username=clpereira"></script>
  <%--<% Html.ScriptUrl("http://s7.addthis.com/js/250/addthis_widget.js#username=clpereira"); %>  --%>
  <br />

  <span>
    <%= Html.ActionLink(Model.LinkParams.EventTitle, Model.IsCurrentEvent ? "Category" : "AuctionResults", new { controller = "Auction", action = Model.IsCurrentEvent ? "Category" : "AuctionResults", id = Model.LinkParams.Event_ID, evnt = Model.LinkParams.EventUrl }, new { @style = "font-size:14px" })%>
    &nbsp;>&nbsp;
    <%=(Model.IsCurrentEvent) ? Html.ActionLink(Model.LinkParams.MainCategoryTitle, "CategoryView", new { controller = "Auction", action = "CategoryView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl }, new { @style = "font-size:14px" }).ToHtmlString() : Model.LinkParams.MainCategoryTitle%>
    &nbsp;>&nbsp;
    <%=Html.ActionLink(Model.LinkParams.CategoryTitle, Model.IsCurrentEvent ? "CategoryView" : "PastCategoriesView", new { controller = "Auction", action = Model.IsCurrentEvent ? "CategoryView" : "PastCategoriesView", id = Model.LinkParams.EventCategory_ID, evnt = Model.LinkParams.EventUrl, maincat = Model.LinkParams.MainCategoryUrl, cat = Model.LinkParams.CategoryUrl }, new { @style = "font-size:14px" })%>
  </span>

  <br class="clear" /><br class="clear" />

  <% if (Model.PrevAuction != null)
     { %>
  <%=Html.ActionLink("< previous lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.PrevAuction.ID, evnt = Model.PrevAuction.EventUrl, maincat = Model.PrevAuction.MainCategoryUrl, cat = Model.PrevAuction.CategoryUrl, lot = Model.PrevAuction.LotTitleUrl })%>
  <%} %>
  <%= (Model.PrevAuction != null && Model.NextAuction != null) ? "&nbsp;|&nbsp" : String.Empty%>
  <% if (Model.NextAuction != null)
     { %>
    <%=Html.ActionLink("next lot >", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.NextAuction.ID, evnt = Model.NextAuction.EventUrl, maincat = Model.NextAuction.MainCategoryUrl, cat = Model.NextAuction.CategoryUrl, lot = Model.NextAuction.LotTitleUrl })%>
  <%} %>
  <% if (Model.PrevAuction != null || Model.NextAuction != null)
     { %>
    <br class="clear" /> <br class="clear" />
  <%} %>

  <span class="lot_title" id="spLotTitle">Lot <%= Model.Lot%>: <span style="text-transform: uppercase"><%= Model.Title%></span></span>  
  <br class="clear" /><br class="clear" />

  <% if (Model.IsPulledOut) { %>
     <div class="pay_notice">NOTE:THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION</div>
    <%} else {
        string address = AppHelper.AuctionImage(Model.LinkParams.ID, Model.DefaultImage);
        bool isImage = System.IO.File.Exists(Server.MapPath(address));
       %>   
    <h4>ITEM DESCRIPTION</h4>
    <table id="auctiondetail_uppertable" cellpadding="0" cellspacing="0">
      <colgroup><col width="502px" />
      <%=(Request.Browser.Browser == "IE") ? String.Empty : "<col width='1px' />"%>
      <col width="240px" /></colgroup>
      <tr>
        <td>
          <div id="featured">
            <% if (isImage) { %>                
                <div id="fragment-img" class="ui-tabs-panel" style="display:table; margin-left:-4px;//vertical-align:middle;//text-align:center">
                  <a id="fragment-a" style="display:table-cell;vertical-align:middle;text-align:center" <%= (!(Request.Browser.Browser == "IE" && Request.Browser.MajorVersion<7)) ? "class=\"cloud-zoom\" rel=\"position:'inside',adjustX:-1,adjustY:-2\"" : String.Empty %>>  
                    <img id="fragment-img-big" alt="" /> <%--src="<%=AppHelper.CompressImage("big-roller.gif") %>" --%>
                  </a>
                 </div>
                 <% if (Request.Browser.Browser != "IE") {%>
                 <span id="spEnlarge" style="cursor:pointer;color:#444;font-size:11px;font-weight:bolder">* CLICK HERE TO ENLARGE THE IMAGE</span>
                 <%} %>
             <%}
               else
               {%>
                <div class="ui-tabs-panel-blank" style="display:table;vertical-align:middle;padding:0;padding-top:2px;text-align:center;width:500px;">
                  <img src="<%=AppHelper.CompressImage("blank.jpg") %>" alt="" />
               </div>
             <%} %>
          </div>
        </td>
        <%=(Request.Browser.Browser == "IE") ? String.Empty : "<td style='background-color:#818181'>&nbsp;</td>"%>
        <td style="//padding:0;//margin-left:2px">
          <div id="dvAD_rightpanel_top">
          <%if (Model.Status == Consts.AuctionStatus.Closed)
              Html.RenderAction("pAuctionDetailPast", "Auction", new { auction_id = Model.LinkParams.ID });
            else
              Html.RenderPartial("~/Views/Auction/pAuctionDetailCurrent.ascx", Model);%>          
          </div>
          <% Html.RenderAction("pAuctionDetailImages", "Auction", new { auction_id = Model.LinkParams.ID });%>          
        </td>
      </tr>
      <% if (Request.Browser.Browser == "IE" && isImage) {%>
        <tr><td colspan="2"><span id="spEnlarge" style="cursor:pointer;color:#444;font-size:11px;font-weight:bolder">* CLICK HERE TO ENLARGE THE IMAGE</span></td></tr>          
       <%} %>  
    </table>
    
    <% if (!String.IsNullOrEmpty(Model.Addendum))
       { %>
       <br /><div class="pay_notice"><strong>UPDATE:</strong>&nbsp;<%=Model.Addendum%></div>
    <%}
       else
       { %>
       <br />
    <%} %>

    <% Html.RenderPartial("~/Views/Auction/pAuctionDetailAdds.ascx", Model); %>
    
    <div class="lot_description"><%=Model.Description%></div>

    <% if (Model.Status != Consts.AuctionStatus.Locked && Model.Status != Consts.AuctionStatus.PulledOut)
       { %>
       <% AuctionUserInfo aui = (ViewData["AuctionUserInfo"] as AuctionUserInfo) ?? new AuctionUserInfo(); %>
       <br class="clear" />
       <h4>BIDDING</h4>
       <table id="tbl_ad_bidding" cellpadding="0" cellspacing="0" border="1" width="530px">    
        <%if (Model.Status == Consts.AuctionStatus.Pending)
          {%>        
          <tr>
            <td class="bordered">
              This is only an auction preview. The bidding for this auction begins on <%=Model.EventDateStart.ToString("g")%> EST. <br class="clear" />
            </td>
          </tr>
          <% if (cuser == null)
             {
               Response.Write("<tr><td class=\"bordered\">" + Html.ActionLink("Log In", "LogOn", new { controller = "Account", action = "LogOn", @returnurl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }).ToHtmlString() + "</td></tr>");
             }
             else
             {
               if (!aui.IsRegisterForEvent)
               { %>
          <tr>
            <td class="bordered">              
              You must be registered for this event before you can bid.<br />
              <%= Html.ActionLink("Click here to register for this event", "Register", "Event", new { controller = "Event", action = "Register", id = Model.LinkParams.ID }, new { @style = "font-weight:bold;font-size:14px" })%>              
            </td>
          </tr>
          <%}
               else
               {%>
          <tr>
            <td class="bordered">              
              <% Response.Write(!aui.IsInWatchList ? Html.ActionLink("Add this item to your Watch List", "AddItemToWatchList", "Account", new { controller = "Account", action = "AddItemToWatchList", id = Model.LinkParams.ID }, new { @style = "font-weight:bold;font-size:14px" }).ToHtmlString() : ("You are currenlty watching this lot in your " + Html.ActionLink("WATCH LIST", "WatchBid", "Account", new { controller = "Account", action = "WatchBid" }, new { @style = "font-weight:bold;font-size:14px;text-decoration:underline" }).ToHtmlString()));%>
            </td>
          </tr>
          <%}
             }%>
        <%}
          else
          {
            if (Model.Status == Consts.AuctionStatus.Closed)
              Response.Write("<tr><td class=\"bordered\">The bidding for this lot has ended.</td></tr>");
            else
            {
              if (cuser == null)
              {
                Response.Write("<tr><td class=\"bordered\">" + Html.ActionLink("Log In", "LogOn", new { controller = "Account", action = "LogOn", @returnurl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }).ToHtmlString() + "</td></tr>");
              }
              else
              {
                bool UserHasRightsToBid = Model.Status == Consts.AuctionStatus.Open && (Model.CloseStep == 0 || (Model.CloseStep == 1 && (ViewData["IsUserHasRightsToBid"] != null && Convert.ToBoolean(ViewData["IsUserHasRightsToBid"]))));
                if (!aui.IsRegisterForEvent)
                  Response.Write("<tr><td class=\"bordered\">You must be registered for this event before you can bid.<br />" + Html.ActionLink("Click here to register for this event", "Register", "Event", new { controller = "Event", action = "Register", id = Model.LinkParams.ID }, new { @style = "font-weight:bold;font-size:14px" }).ToHtmlString() + "</td></tr>");
                else
                {%>
                <% if (UserHasRightsToBid)
                   { %>
              <tr>
                <td class="bordered">Contact Lelands if you have any questions before bidding. By bidding you agree to all Terms and Conditions and are entering into a legally binding Contract.<br />
                  <span class="bid_info"><b>19.5% buyers premium will be added to all winning bids for this auction. Please bid accordingly.</b></span>
                </td>
              </tr>
              <%} %>              
              <tr>
                <td class="bordered">
                 <% if (UserHasRightsToBid)
                      Html.RenderPartial("~/Views/Auction/pBiddingPanel.ascx", Model);
                    else Response.Write("You can not participate in bidding on this lot. The bidding for this lot has ended.");%>
                </td>
              </tr>
            <%}
              }
            }
          }
        %>
        </table>
      <%} %>

    <% if (Model.PrevAuction != null || Model.NextAuction != null)
       { %><br class="clear" /> <br class="clear" /><%} %>
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
  <%} %>

  <%} %>
 </div> 
</asp:Content>
<asp:Content ContentPlaceHolderID="cphJS" ID="cJ" runat="server">
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
  <%
    string address = AppHelper.AuctionImage(Model.LinkParams.ID, Model.DefaultImage);
    string path = Server.MapPath(address);
    System.Drawing.Bitmap img = null;
    try
    {
      img = System.IO.File.Exists(path)? new System.Drawing.Bitmap(path) : null;
    }
    catch
    {
      img = null;
    }
    if ( img != null ) {
    int imgH = 0;    
    imgH = Math.Min(img.Height, 502);
   %>
  <script type="text/javascript">
    $(document).ready(function () {
      var a = parseInt($("#dvAD_rightpanel_top").height());
      var b = parseInt($("#dvAD_rightpanel_bottom").attr("value"));
      var c = parseInt('<%=imgH %>');
      var d = parseInt($("#dvAD_rightpanel_bottom_maximage").attr("value"));
      $("#featured, #fragment-img").css("height", Math.min(Math.max(a + b, Math.max(c, d)), 502) + "px");
            
      $("#spEnlarge").click(function () {
        $("#fragment-a").click();
      });

      $.each($(".postloadimages"), function (i, item) {
        $(item).attr("src", $(item).attr("limg"));
      });
      
      selectimg_detail($("#nav-fragment-1"));
    });
  </script>
    <% if (Request.Browser.Browser == "IE") {%>
     <script type="text/javascript">
       $(document).ready(function () {
         $("#featured").css("line-height", ($("#featured").height() - 1) + "px");
       });
    </script>
    <%} %>
  <%} %>
 
</asp:Content>