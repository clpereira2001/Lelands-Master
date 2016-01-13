<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Welcome to Lelands.com - The Industry Leader in Sports and Americana Memorabilia</title>
  <% Html.Script("s3Slider.js"); %>
  <style type="text/css">
    #events_box {
      background-color: #fff;
      color: #555;
      font-size: 13px;
      height: 96px;
      margin: 0 10px;
      text-align: justify;
      width: 305px;
    }

    #events_box div a {
      font-size: 12px;
      font-weight: bold;
    }

    #btn_events {
      color: #444;
      display: block;
      float: left;
      font-size: 10px;
      font-weight: bold;
      margin-left: 8px;
      margin-top: 2px;
    }

    #btn_events li {
      border: 1px solid #888;
      cursor: pointer;
      display: inline;
      line-height: 20px;
      padding: 2px 5px 2px 5px;
      /*//margin-right:3px;
      //text-align:center;*/
    }

    #btn_events li.event_button_active {
      background-color: #c61a0c;
      color: #fff;
    }

    #content { background-color: black; }
  </style>

  <style type="text/css" media="screen">
  #slider1
  {
    width: 980px; /* important to be same as image width */
    height: 375px; /* important to be same as image height */
    position: relative; /* important */
    overflow: hidden; /* important */
  }
  #slider1Content
  {
    width: 980px; /* important to be same as image width or wider */
    position: absolute;
    top: 0;
    margin-left: 0;
    list-style-type: none
  }
  .slider1Image
  {
    float: left;
    position: relative;
    display: none
  }
  .slider1Image span
  {
    position: absolute;
    font: 10px/15px Arial, Helvetica, sans-serif;
    padding: 10px 13px;
    width: 375px;
    background-color: #000;
    filter: alpha(opacity=70);
    -moz-opacity: 0.7;
    -khtml-opacity: 0.7;
    opacity: 0.7;
    color: #fff;
    display: none
  }
  .slider1Image span strong
  {
    font-size: 14px
  }
  .left
  {
    top: 0;
    left: 0;
    width: 150px !important;
    height: 365px;
  }
  .right
  {
    right: 0;
    bottom: 0;
    width: 150px !important;
    height: 365px;
  }
  .top
  {
    top: 0;
    left: 0;
  }
  .bottom
  {
    bottom: 0;
    left: 0;
  }
</style>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <% var images = ViewData["BigImages"] as List<HomepageImage>; %>

  <div class="main_img">
    <%
      if (images == null || images.Count() == 0)
        Response.Write("&nbsp;");
      else if (images.Count() == 1)
      { %>
      <a href="<%= String.IsNullOrEmpty(images[0].Link) ? "#" : images[0].Link %>" title="<%= images[0].LinkTitle %>">
        <img alt="" src="<%= AppHelper.CompressImagePath("/public/images/homepage/" + images[0].ImgFileName) %>" />
      </a>
    <% }
      else
      {
        for (var i = 0; i < images.Count(); i++)
        { %>
        <a id="img<%= i %>" href="<%= String.IsNullOrEmpty(images[i].Link) ? "#" : images[i].Link %>" title="<%= images[i].LinkTitle %>" <%= i != 0 ? "style='display:none'" : "" %>>
          <img alt="" src="<%= AppHelper.CompressImagePath("/public/images/homepage/" + images[i].ImgFileName) %>" />
        </a>
      <% } %>
      <div class="slideshow">
        <ul class="buttons">
          <% for (var i = 0; i < images.Count(); i++)
             { %>
            <li id="btn<%= i %>" <%= (i == 0) ? "class=\"active\"" : "" %>><a href="#" title=""><%= (i + 1) %></a></li>
          <% } %>
        </ul>
      </div>
    <% } %>

    <a id="img0" href="#" title="The Auction Is Now Closed">
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_ended.jpg") %>" />
      </a>
      <a id="img1" href="/Auction/CategoryView/3442/November-2010-Catalog/Sports/The-Sal-LaRocca-Collection" title="The Sal LaRocca Collection" >
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_sallarocca.jpg") %>" />
      </a>
      <a id="img2" href="/Auction/CategoryView/3419/November-2010-Catalog/Sports/The-Fred-Budde-Collection" title="The Fred Budde Collection"  style="display:none">
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_fredbudde.jpg") %>" />
      </a>
      <a id="img3" href="/Auction/CategoryView/3444/November-2010-Catalog/Sports/The-Mark-Mausner-Collection" title="The Mark Mausner Boxing Collection" style="display:none">
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_markmausner.jpg") %>" />
      </a>
      <a id="img4" href="/Auction/CategoryView/3422/November-2010-Catalog/Photo-Collection/Babe-Ruth-and-Lou-Gehrig" title="Vintage Photography" style="display:none">
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_vintagephoto.jpg") %>" />
      </a>
      <a id="img5" href="/Auction/CategoryView/3439/November-2010-Catalog/Rock-n-Roll/Rock-n-Roll" title="Rock'N'Roll ~ The Rick Rosen Beatles Collection ~ 	The Famous Rock Photographers" style="display:none">
        <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_rockroll.jpg") %>" />
      </a>      
     <div class="slideshow">
        <ul class="buttons">
          <li id="btn0" class="active" ><a href="#" title="">1</a></li>
          <li id="btn1"><a href="#" title="">2</a></li>
          <li id="btn2"><a href="#" title="">3</a></li>
          <li id="btn3"><a href="#" title="">4</a></li>
          <li id="btn4"><a href="#" title="">5</a></li>
          <li id="btn5"><a href="#" title="">6</a></li>
        </ul>
     </div>
  </div>


  
      <div id="slider9">
        <ul id="slider1Content">
            <li class="slider1Image">
                <a href=""><img src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_markmausner.jpg") %>" alt="1" /></a>
                <span class="left"><strong>Title text 1</strong><br />Content text...</span></li>
            <li class="slider1Image">
                <a href=""><img src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_vintagephoto.jpg") %>" alt="2" /></a>
                <span class="right"><strong>Title text 2</strong><br />Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...</span></li>
            <li class="slider1Image">
                <a href=""><img src="<%=AppHelper.CompressImagePath("/public/images/homepage/912_ended.jpg") %>" alt="2" /></a>
                <span class="bottom"><strong>Title text 2</strong><br />Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...Content text...</span></li>
            <div class="clear slider1Image"></div>
        </ul>
      </div>

  <%
      if (images==null || images.Count()==0)
        Response.Write("&nbsp;");
      else if (images.Count()==1) {%>
        <a href="<%=String.IsNullOrEmpty(images[0].Link)?"#":images[0].Link %>" title="<%=images[0].LinkTitle %>">
          <img alt="" src="<%=AppHelper.CompressImagePath("/public/images/homepage/"+images[0].ImgFileName) %>" />
        </a>
      <%} else
      {%>
        <div id="slider1">
          <ul id="slider1Content1">
          <% for (int i=0; i<images.Count(); i++){ %>
            <li class="slider1Image">
              <a href="<%=String.IsNullOrEmpty(images[i].Link)?"#":images[i].Link %>" title="<%=images[i].LinkTitle %>">
                <img src="<%=AppHelper.CompressImagePath("/public/images/homepage/"+images[i].ImgFileName) %>" alt="<%=images[i].LinkTitle %>" />                
              </a>
              <span class="left">
                <strong>Title text <%=(i+1) %></strong>
                <br />Content text...
              </span>
            </li>            
          <%}%>
          <div class="clear slider1Image"></div>
          </ul>
        </div>
      <%}%>


  <img src="/public/images/15-min-ext.jpg" alt="" style='border:2px solid brown;margin-left:-2px' />
  <img src="/public/images/30-min-ext.jpg" alt=""  style='border:2px solid brown;margin-left:-2px' />
  <img src="/public/images/homepage/rule_10min.jpg" alt="" />

  <% images = ViewData["StripeImages"] as List<HomepageImage>;
     foreach (var homepageImage in images ?? new List<HomepageImage>())
     { %>
    <img alt="" src="<%= AppHelper.CompressImagePath("/public/images/homepage/" + homepageImage.ImgFileName) %>" /><br />
  <% } %>

  <% var buzz_list = new List<HotNew>(); //ViewData["BuzzArray"] as List<HotNew> ?? new List<HotNew>();
     if (buzz_list.Count > 0)
     { %>
    <div style='background-color: #F6F6F6; border: 1px solid #aaa; color: #565656; cursor: default; float: left; font-family: Arial, Helvetica, sans-serif; font-size: 14px; font-weight: bold; margin: 5px; padding: 5px; width: 960px;'>
      <a href='/Home/Buzz' style='color: #A80101; cursor: pointer; float: left; font-size: 14px; font-weight: bold; padding-left: 20px; padding-right: 20px;' title='See all Lelands latest news'>Lelands Buzz</a>
      <div style="float: left; text-align: right;">|</div>
      <div style="cursor: pointer; float: left; padding-left: 30px; width: 775px" id="dvNews" title='See next Lelands buzz'></div>
    </div>
  <% } %>

  <div style="margin: 0 5px">
    <img src="<%= AppHelper.CompressImagePath("/public/images/homepage/Show-Calendar-Banner-2.jpg") %>" alt="" />
  </div>

  <map name="banner2">
    <area shape="poly" alt="Josh Evans" coords="455,5, 550,5, 550,141, 455,141" href="mailto:&#106;&#111;&#115;&#104;&#101;&#118;&#097;&#110;&#115;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;" title="Josh Evans" />
    <area shape="poly" alt="Mike Heffner" coords="565,5, 655,5, 655,141, 565,141" href="mailto:&#109;&#105;&#107;&#101;&#104;&#101;&#102;&#102;&#110;&#101;&#114;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;" title="Mike Heffner" />
  </map>

  <div style="margin: 0 5px">
    <img style="border: 1px solid #555" src="<%= AppHelper.CompressImagePath("/public/images/homepage/Appraisal-Banner-2.jpg") %>" alt="" usemap="#banner2"/>
  </div>

  <div class="image_container">
<div class="small_img" style='margin-left: 5px; margin-right: 2px; width: 320px; _margin-left: 0px; _margin-right: 0px'>
<a href="/Home/Consign" target="_blank" title="Consign to Lelands">
<img src="<%=AppHelper.CompressImagePath("/public/images/homepage/smallbanner_freeappraisals.jpg") %>" alt="" /></a><br />
</div>
<div class="small_img" style='margin-left: 3px; margin-right: 2px; padding: 0px; width: 320px'>
<a href="http://www.bloomberg.com/video/93207117-lelands-s-heffner-on-auction-of-babe-ruth-jersey.html" target="_blank" title="Lelands's Heffner on Auction of Babe Ruth Jersey ">
<img src="<%=AppHelper.CompressImagePath("/public/images/homepage/smallruth.jpg") %>" alt="Lelands's Heffner on Auction of Babe Ruth Jersey " /></a><br />
</div>

<div class="small_img" style='margin-left: 3px; margin-right: 2px; padding: 0px; width: 320px'>
<a href="http://espn.go.com/mlb/story/_/id/7953437/babe-ruth-jersey-sells-record-44-million" target="_blank" title="Babe Ruth jersey sells for $4.4 million">
<img src="<%=AppHelper.CompressImagePath("/public/images/homepage/BRRB2.jpg") %>" alt="Babe Ruth jersey sells for $4.4 million" /></a><br />
</div>
  <div class="small_img" style='margin-left:3px;margin-right:2px;padding:0px;width:320px'>
        <a href="http://stores.ebay.com/lelands-com-store?_rdc=1" target="_blank" title=""><img src="<%=AppHelper.CompressImagePath("/public/images/homepage/le_ebay.jpg") %>" alt="Lelands Web Store" /></a><br />  
      </div>

  
    <div class="small_img" style='margin-top:-1px;margin-left:0px;margin-right:2px;_margin-right:0px;width:326px;padding-top:2px;background:url("/public/images/homepage/upcomingE.jpg") no-repeat 0 0'>         <br />        
      <div id="events_box">
        <div id="event0">
          <span style='color:#222;font-weight:bold'>• Spring Catalog Auction</span><br />
          Starting May 18th - Ending June 15th<br />
        </div>
             
      </div>              
      <ul id='btn_events'>
        <li id='btn_ev0' class="event_button_active">1</li>
        <li id='btn_ev1'>2</li>
      </ul>
    </div>
  <%--</div>--%>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="cphJS" runat="server">
  <% var buzz_list = new List<HotNew>(); // ViewData["BuzzArray"] as List<HotNew> ?? new List<HotNew>();
     var images = ViewData["BigImages"] as List<HomepageImage>;
  %>

  <script type="text/javascript">
    $(document).ready(function() {
      //$('#slider1').s3Slider({timeOut: 3000 });

      $(function($) {
        <% if (images.Count() > 1)
           { %>
        var timer;
        var imgIndex = 0;
        var imgCount = parseInt("<%= images.Count() %>");

        function button_click(event) {
          $("#img" + imgIndex).hide();
          if (event != null && event.data != null)
            imgIndex = event.data.index;
          else
            imgIndex = (imgIndex + 1 >= imgCount) ? 0 : imgIndex + 1;
          $("#img" + imgIndex).css("opacity", "0");
          $("#img" + imgIndex).show();
          $("#img" + imgIndex).animate({ "opacity": 1 }, 300, "linear", null);
          $("ul.buttons li").removeClass("active");
          $("#btn" + imgIndex).addClass("active");
          clearTimeout(timer);
          timer = setTimeout(eval("button_click"), "6000");
        }

        <% for (var i = 0; i < images.Count(); i++)
           { %>
        $('#btn<%= i %>').bind('click', { index: <%= i %> }, button_click);
        <% } %>
        <% } %>

        var news_index = 0;
        var news = new Array(<%= buzz_list.Count %>);
        <% for (var i = 0; i < buzz_list.Count; i++)
           { %>
        news[<%= i %>] = "<%= buzz_list[i].Title %>";
        <% } %>
        var timer_news;
        $("#dvNews").text(news[news_index]);
        $("#dvNews").show("slow");

        function news_click() {
          $("#dvNews").hide();
          news_index = (news_index + 1 >= news.length) ? 0 : news_index + 1;
          $("#dvNews").text(news[news_index]);
          $("#dvNews").show("slow");
          clearTimeout(timer_news);
          timer_news = setTimeout(eval("news_click"), "6000");
        }

        $('#dvNews').bind('click', {}, news_click);

        var evntIndex = 0;
        var evntCount = 1;
        var timer_events;

        function events_click(event) {
          $("#events_box div").hide();
          if (event != null && event.data != null)
            evntIndex = event.data.index;
          else
            evntIndex = (evntIndex + 1 >= evntCount) ? 0 : evntIndex + 1;
          $("#event" + evntIndex).show("slow");
          $("#btn_events li").removeClass("event_button_active");
          $("#btn_ev" + evntIndex).addClass("event_button_active");
          clearTimeout(timer_events);
          timer_events = setTimeout(eval("events_click"), "6000");
        }

        $('#btn_ev0').bind('click', { index: 0 }, events_click);
        $('#btn_ev1').bind('click', { index: 1 }, events_click);
        $('#btn_ev2').bind('click', { index: 2 }, events_click);
        $('#btn_ev3').bind('click', { index: 3 }, events_click);
        $('#btn_ev4').bind('click', { index: 4 }, events_click);
        $('#btn_ev5').bind('click', { index: 5 }, events_click);
        $('#btn_ev6').bind('click', { index: 6 }, events_click);
        $('#btn_ev7').bind('click', { index: 7 }, events_click);

        function OnLoad(event) {
          <% if (images.Count() > 1)
             { %>
          clearTimeout(timer);
          timer = setTimeout(eval("button_click"), "6000");
          <% } %>
          clearTimeout(timer_news);
          timer_news = setTimeout(eval("news_click"), "6000");
          clearTimeout(timer_events);
          timer_events = setTimeout(eval("events_click"), "6000");
        }

        OnLoad();
      });
    });
  </script>
</asp:Content>