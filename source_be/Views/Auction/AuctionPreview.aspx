<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Import Namespace="Vauction.Utils" %>
<%@ Import Namespace="Vauction.Models" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
  <% Html.StyleUrl("/content/redmond/jquery-ui-1.7.1.custom.css"); %>
  <% Html.Style("frontend.css"); %>
  <% Html.Style("jquery.lightbox-0.5.css"); %>

  <% Html.Script("jquery-1.4.1.min.js"); %>
  <% Html.Script("jquery-ui-1.7.2.custom.min.js"); %>
  <% Html.Script("jquery.lightbox-0.5.min.js"); %>

  <%= Html.CompressJs(Url) %>
  <%= Html.CompressCss(Url) %>
  <% Html.Clear(); %>

  <title>Auction preview</title>
  <%--<script type="text/javascript" >
    $(function() {
      $("#fragment-a").lightBox({
        overlayOpacity: 0.6,
        imageLoading: '<%=this.ResolveUrl("~/public/images/lightbox-ico-loading.gif") %>',
        imageBtnClose: '<%=this.ResolveUrl("~/public/images/lightbox-btn-close.gif") %>',
        containerResizeSpeed: 350
      });
    });
  </script> --%> 
</head>
<body>
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
 <div class="center_content">
  <%
      Auction currentAuction = ViewData["CurrentAuction"] as Auction;
      List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
      int height = 0;
      int height_small = 0;
      int BidsCount = 0;
  %>
      <span class="lot_title">Lot <%= currentAuction.Lot%>: <span style="text-transform: uppercase"><%= currentAuction.Title%></span></span>
      <br class="clear" /><br class="clear" />
 <% if (!String.IsNullOrEmpty(currentAuction.Addendum) || currentAuction.PulledOut.Value)
         { %>
      <div class="pay_notice">
        <strong>UPDATE:</strong>
        <%=(!String.IsNullOrEmpty(currentAuction.Addendum)) ? currentAuction.Addendum : "NOTE:THIS LOT HAS BEEN WITHDRAWN FROM OUR AUCTION"%>
      </div>
      <br class="clear" />
      <%}
       if (!currentAuction.PulledOut.Value)
       {%>
      <h4>ITEM DESCRIPTION</h4>      
      <table id="auction_detailed" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;table-layout: fixed">
        <colgroup>
          <col width="150px" />
          <col width="600px" />
        </colgroup>
        <tr>
          <td class="bordered">
            Event:
          </td>
          <td class="bordered">
            <a href="#"><%=currentAuction.Event.Title %></a>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Category:
          </td>
          <td class="bordered">
            <a href="#"><%=currentAuction.EventCategory.MainCategory.Title %></a>
            &nbsp;>&nbsp;
            <a href="#"><%=currentAuction.EventCategory.Category.Title %></a>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Reserve:
          </td>
          <td class="bordered">
            <%=(currentAuction.Price.GetCurrency() + ((String.IsNullOrEmpty(currentAuction.Estimate)) ? "" : " * ("+currentAuction.Estimate+")"))%>
          </td>
        </tr>
        <tr>
          <td class="bordered">
            Auction:
          </td>
          <td class="bordered">
            <%=currentAuction.Event.DateStart%>&nbsp;EST -
            <%=currentAuction.Event.DateEnd%>&nbsp;EST
          </td>
        </tr>
         <% if (currentAuction.Event.DateEnd<DateTime.Now) {%>
        <tr>
          <td class="bordered">
            Winning Bid:
          </td>
          <td class="bordered"><%=(BidsCount == 0) ? "&nbsp;" : (!currentAuction.IsUnsold)?Convert.ToDecimal(0).GetCurrency():String.Empty %></td>
        </tr>
        <%} %>
         <tr>
          <td class="bordered">
            <%=(currentAuction.Status == (byte)Consts.AuctionStatus.Closed) ? "Price Realized:" : "Current Bid:"%>
          </td>
          <td class="bordered"><%=(BidsCount == 0) ? "&nbsp;" : (!currentAuction.IsUnsold) ? ((currentAuction.Status == (byte)Consts.AuctionStatus.Closed) ? (Convert.ToDecimal(0)*(1+currentAuction.Event.BuyerFee/100)).GetCurrency() : Convert.ToDecimal(0).GetCurrency()) : "UNSOLD"%></td>
        </tr>
        <tr>
          <td class="bordered">
            Number of Bids:
          </td>
          <td class="bordered"><%=(BidsCount == 0) ? "&nbsp;" : BidsCount.ToString()%></td>
        </tr>
      </table>
      
      <br class="clear" />      
      <div>
        <% if (currentAuction.Description != "")
           { %>
        <div class="lot_description">
          <%= currentAuction.Description%>
        </div>
        <br />
        <% } %>
        <% 
           string address;
           StringBuilder sb = new StringBuilder();
           StringBuilder sbThumbnail = new StringBuilder();
           string siteUrl = ConfigurationManager.AppSettings["siteURL"];          
           foreach (var Image in currentAuctionImages)
           {
             address = Consts.GetVauctionFrontendDir+@"\public\AuctionImages\" + (string.Format("{0}\\{1}\\{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "\\" + Image.PicturePath;
             if (!System.IO.File.Exists(address)) continue;
             sb.Append(address + "~");

             address = Consts.GetVauctionFrontendDir+@"\public\AuctionImages\" + (string.Format("{0}\\{1}\\{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "\\" + Image.ThumbNailPath;
             if (!System.IO.File.Exists(address)) continue;
             sbThumbnail.Append(address + "~");
           }
           if (sb.Length > 0 && sbThumbnail.Length > 0)
           {
             sb.Remove(sb.Length - 1, 1);
             sbThumbnail.Remove(sbThumbnail.Length - 1, 1);             
             %>
             <span id="spEnlarge" style="text-decoration:underline;cursor:pointer;color:Navy;">Click on image to enlarge it</span>                      
          <div id="featured">
          <ul class="ui-tabs-nav" id="img_list" >
            <% int imgCount = 0;               
               System.Drawing.Bitmap img;               
               System.Drawing.Size size;
               height_small = currentAuctionImages.Count() * 10;
               string address2, address3;
               foreach (var Image in currentAuctionImages)
               {
                 address = Consts.GetVauctionFrontendDir + @"\public\Auctionimages\" + (string.Format("{0}\\{1}\\{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "\\" + Image.ThumbNailPath;
                 if (!System.IO.File.Exists(address)) continue;
                 img = new System.Drawing.Bitmap(address);
                 if (img != null)
                 {
                   size = Vauction.Utils.Graphics.ImageActions.ImageResizing(img, Math.Min(100, img.Width), false);
                   height_small += size.Height;
                 }

                 address2 = Consts.GetVauctionFrontendDir + @"\public\Auctionimages\" + (string.Format("{0}\\{1}\\{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "\\" + Image.PicturePath;
                 if (!System.IO.File.Exists(address2)) continue;
                 img = new System.Drawing.Bitmap(address2);
                 if (img != null)
                 {
                   size = Vauction.Utils.Graphics.ImageActions.ImageResizing(img, Math.Min(600, img.Width), false);
                   height = Math.Max(size.Height, height);
                 }

                 address3 = Consts.GetVauctionFrontendDir + @"\public\Auctionimages\" + (string.Format("{0}\\{1}\\{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "\\" + Image.LargePath;
                 if (!System.IO.File.Exists(address3) && !String.IsNullOrEmpty(address2)) address3=address2;
                 
                 ++imgCount;
                 address = siteUrl + "/public/Auctionimages/" + (string.Format("{0}/{1}/{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "/" + Image.ThumbNailPath;
                 address2 = siteUrl + "/public/Auctionimages/" + (string.Format("{0}/{1}/{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "/" + Image.PicturePath;
                 address3 = siteUrl + "/public/Auctionimages/" + (string.Format("{0}/{1}/{2}", currentAuction.ID / 1000000, currentAuction.ID / 1000, currentAuction.ID)) + "/" + Image.LargePath;
            %>            
            <li class="ui-tabs-nav-item" id="nav-fragment-<%=imgCount %>" imgaddr="<%=address2 %>" imgaddr2="<%=address3 %>">
            <a>
              <img src="<%=address%>" alt="" />
            </a>
            </li>
            <% } %>
          </ul>
          <div id="fragment-img" class="ui-tabs-panel" style="display:table">            
            <a id="fragment-a" style="display: table-cell; vertical-align: middle">
              <img id="fragment-img-big"  alt=""/>
            </a>
          </div>        
        </div>
        <% } %>
      </div>
      <%=(height > 0) ? "<br />" : ""%>
      <% } %>
  </div>
  
  <% height = Math.Max(Math.Max(height, 500), height_small); %>
 
  <script type="text/javascript">
    function selectimg(li) {
      $("#featured > ul > li").removeClass("ui-tabs-selected");
      li.addClass("ui-tabs-selected");
      $("#fragment-img-big").attr("src", li.attr("imgaddr"));
      $("#fragment-a").attr("href", li.attr("imgaddr2"));
    }
    $(document).ready(function() {
      $("#featured").css("height", "<%=height %>px");
      $("#fragment-img").css("height", "<%=height %>px");
     
      selectimg($("#nav-fragment-1"));
      $(".ui-tabs-nav-item > a").click(function() {
        selectimg($(this).parent("li:first"));
      });
      $("#spEnlarge").click(function() {
        $("#fragment-a").click();
      });
    });
  </script>
</body>
</html>
