<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Vauction.Models.Image>>" %>
<%@ Import Namespace="System.Drawing" %>

<% 
    string address;    
    List<Vauction.Models.Image> list = new List<Vauction.Models.Image>();
    foreach (Vauction.Models.Image image in Model)
    {
      address = AppHelper.AuctionImage(image.Auction_ID, image.PicturePath);
      if (!System.IO.File.Exists(Server.MapPath(address))) continue;
      address = AppHelper.AuctionImage(image.Auction_ID,  image.ThumbNailPath);
      if (!System.IO.File.Exists(Server.MapPath(address))) continue;
      list.Add(image);
    }
    if (list.Count > 0)
    {
      Bitmap img;
      int maxheight = 0;
      var addr1 = new string[list.Count];
      var addr2 = new string[list.Count];
      var addr3 = new string[list.Count];
      var height = new int[list.Count];
      for (int i = 0; i < list.Count; i++)
      {
        addr1[i] = AppHelper.AuctionImage(list[i].Auction_ID, list[i].ThumbNailPath);
        addr2[i] = AppHelper.AuctionImage(list[i].Auction_ID, list[i].PicturePath);
        addr3[i] = String.IsNullOrEmpty(list[i].LargePath) ? addr2[i] : AppHelper.AuctionImage(list[i].Auction_ID, list[i].LargePath);

        img = new Bitmap(Server.MapPath(addr1[i]));
        maxheight = Math.Max(maxheight, img.Height);
        height[i] = img.Height;


      }%>
      <div id="gallery-1" class="royalSlider rsMinW rsHor">
      <% for (int i = 0; i < list.Count; i++)
         {%>
          <a class="rsImg" data-rsbigimg="<%= AppHelper.CompressImagePath(addr3[i]) %>" href="<%= AppHelper.CompressImagePath(addr2[i]) %>" >
            <img class="rsTmb" src="<%= AppHelper.CompressImagePath(addr1[i]) %>" style="margin:<%=(maxheight-height[i])/2 %>px 0" height="<%=height[i] %>" />
          </a>
          <%--data-rsw="<%=img2.Width %>" data-rsh="<%=img2.Height%>" --%>
          <%-- height="<%=img.Height %>" width="<%=img.Width %>"--%>
       <%} %>
      </div>
     <script type="text/javascript">
      $(document).ready(function () {
        $('#gallery-1').royalSlider({
          fullscreen: {
            enabled: true,
            nativeFS: true
          },
          controlNavigation: 'thumbnails',
          autoScaleSlider: true,
          autoScaleSliderWidth: 500,
          autoScaleSliderHeight: 600,
          loop: true,
          imageScaleMode: 'fit-if-smaller',
          navigateByClick: true,
          numImagesToPreload: 4,
          arrowsNavAutoHide: false,
          //arrowsNavHideOnTouch: true,
          keyboardNavEnabled: true,
          fadeinLoadedSlide: true,
          //globalCaption: true,
          //globalCaptionInside: false,
          thumbs: {
            appendSpan: true,
            firstMargin: true,
            paddingBottom: 4
          }
        });

        $(".rsOverflow").after("<div class='sliderpanel' enlarge='true'>* CLICK HERE TO ENLARGE THE IMAGE</div>");
        $(".sliderpanel").click(function () {
          var t = $(this);
          if (t.attr("enlarge") == 'true') {
            $("#gallery-1").data('royalSlider').enterFullscreen();
            t.html("CLICK HERE TO RETURN TO LOT DETAIL");
            t.attr("enlarge", "false");
          } else {
            $("#gallery-1").data('royalSlider').exitFullscreen();
            t.html("CLICK HERE TO ENLARGE THE IMAGE");
            t.attr("enlarge", "true");
          }
        });
      });
    </script>
  <% }
    else
    { %>
      <img src="<%=AppHelper.CompressImage("blank.jpg") %>" alt="" />
<% }%>