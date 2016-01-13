<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="top_menu">
    <ul id="menu" style="margin-left:30px">  <%--style="margin-left:6px"--%>
  <li><span>&nbsp;</span></li>
  <li>
    <%= Html.ActionLink("HOME", "Index", "Home", new { controller = "Home", action = "Index" }, new { @id = "aHome" })%> 
  </li>
  <li><span>&nbsp;</span></li>
  <li>
		<% Html.RenderAction("pCategoryMenu", "Auction");%> 
  </li>
  <li><span>&nbsp;</span></li>
  <li>
    <%= Html.ActionLink("WEB RULES/TERMS", "FAQs", "Home", new { controller = "Home", action = "FAQs" }, new { @id = "aFAQ" })%> 
  </li> 
  <li><span>&nbsp;</span></li>
  <li>    
    <%= Html.ActionLink("CONSIGN TO AUCTION", "Consign", "Home", new { controller = "Home", action = "Consign" }, new { @id = "aConsign" })%> 
  </li>   
  <li><span>&nbsp;</span></li>
  <li>
    <%= Html.ActionLink("ABOUT US", "About", "Home", new { controller = "Home", action = "About" }, new { @id = "aAbout" })%> 
  </li> 
  <li><span>&nbsp;</span></li>
  <li>
    <%= Html.ActionLink("PAST AUCTION RESULTS", "PastAuctionResults", new { controller = "Auction", action = "PastAuctionResults" }, new { @id = "aPast" })%>
  </li>
  <li><span>&nbsp;</span></li>
  <% if (AppHelper.CurrentUser == null) { %>    
    <li>
     <%= Html.ActionLink("REGISTER", "Register", "Account", new { controller = "Account", action = "Register" }, new { @id = "aRegistration" })%>
    </li>   
  <% } else { %>  
    <li>          
      <%= Html.ActionLink("MY ACCOUNT", "MyAccount", "Account", new { controller = "Account", action = "MyAccount" }, new { @id = "aAccount" })%> 
    </li>   
  <% }  %>
 <%-- <li><span>&nbsp;</span></li>
  <li>
    <a class="store_link" href="http://store.lelands.com" style="background:url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -60px;color:#fff">WEB STORE</a>
  </li>--%>
  <li><span>&nbsp;</span></li>
</ul>
</div>

<%
  string idLink = "#aHome";
  string[] path = Page.Request.FilePath.Split(new string[] { "/", ".aspx", ".aspx/" }, StringSplitOptions.RemoveEmptyEntries);
  if (path.Length > 0)
  {
    if (path[0].CompareTo("Home")==0 && path.Count()>1)
    {      
      switch (path[1])
      {
        case "FAQs": idLink = "#aFAQ"; break;
        case "Consign": idLink = "#aConsign"; break;
        case "About": idLink = "#aAbout"; break;
        case "Search": idLink = "#aCurrentAuction"; break;
        case "AdvancedSearch": idLink = "#aCurrentAuction"; break;
        case "AdvancedSearchResult": idLink = "#aCurrentAuction"; break;
        default: idLink = "#aHome";  break;
      }
    }
    else if (path[0].CompareTo("Auction") == 0 && path.Count() > 1)
    {
      switch (path[1])
      {
        case "PastAuctionResults": idLink = "#aPast"; break;
        case "AuctionResults": idLink = "#aPast"; break;
        case "PriceRealized": idLink = "#aPast"; break;
        case "MyBids": idLink = "#aPast"; break;
        case "PastAuctionUpdates": idLink = "#aPast"; break;
        case "PastCategoriesView": idLink = "#aPast"; break;
        default: idLink = "#aCurrentAuction"; break;
      }
    }
    else if (path[0].CompareTo("Account") == 0 && path.Count() > 1)
    {
      switch (path[1])
      {        
        case "Register": idLink = "#aRegistration"; break;
        case "MyAccount": idLink = "#aAccount"; break;
        case "RegisterConfirmSuccess": idLink = "#aRegistration"; break;
        case "RegisterConfirm": idLink = "#aRegistration"; break;
        case "ForgotPassword": idLink = "#aRegistration"; break;
        case "ForgotPasswordUpdate": idLink = "#aRegistration"; break;
        case "Profile": idLink = "#aAccount"; break;
        case "ProfileSaveMessage" : idLink = "#aAccount"; break;
        case "WatchBid": idLink = "#aAccount"; break;
        case "EditMailSettings": idLink = "#aAccount"; break;
        case "AuctionsParticipated" : idLink = "#aAccount"; break;
        case "PastAuction": idLink = "#aAccount"; break;
        case "AuctionInvoices": idLink = "#aAccount"; break;
        default: idLink = "#aLogin"; break;
      }
    }
    else if (path[0].CompareTo("Event") == 0 && path.Count() > 1)
    {
      switch (path[1])
      {
        case "Register": idLink = "#aCurrentAuction"; break;
        default: idLink = "#aCurrentAuction"; break;
      }
    }
    else if (path[0].CompareTo("Sales") == 0)
    {
      idLink = "#aCurrentSales";
    }
  }
%>

<script type="text/javascript">
  $(document).ready(function () {
      $("#menu li").hover(function () {
        $(this).children("a:first").children(".drop_down").css("background", "url('/Zip/Image?path=/public/images/menu/bg_drop_down_hover.png') no-repeat left");
        $(this).children("a:first").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -60px").css("color", "#FFF");
        $(this).children("ul").animate({ opacity: "show", top: "40" }, "fast");
        $(this).children("a.store_link:first").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -60px").css("color", "#FFF");
        if ($(this).children("a:first").hasClass("oneMainCategory")) {
          $(this).find('table').first().css('left', "0").css('top', "2px").show();
        }
      }, function () {
        $(this).children("a:first").children(".drop_down").css("background", "url('/Zip/Image?path=/public/images/menu/bg_drop_down.png') no-repeat left");
        $(this).children("a:first").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px 0px").css("color", "#FFF");
        $(this).children(".current").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -120px").css("color", "#3A0801").children(".drop_down").css("background", "url('/Zip/Image?path=/public/images/menu/bg_drop_down_current.png') no-repeat left");
        $(this).children("ul").animate({ opacity: "hide", top: "55" }, "fast");
        $(this).children("a.store_link:first").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -60px").css("color", "#FFF");
      });
      $("#menu li ul li").hover(function () {
        $(this).children("a:first").css("background", "url('/Zip/Image?path=/public/images/menu/bg_sumbenu_selected.png') repeat-x left").css("color", "#FFF").children("em").css("background", "url('/Zip/Image?path=/public/images/menu/bg_arrow_right_hover.png') no-repeat top left");
        $(this).children("table").show();
      }, function () {
        $(this).children("a:first").css("background", "#e0e0e0").css("color", "#222").children("em").css("background", "url('/Zip/Image?path=/public/images/menu/bg_arrow_right.png') no-repeat top left");
        $(this).children(".current").css("background", "url('/Zip/Image?path=/public/images/menu/bg.png') repeat-x 0px -120px").css("color", "#3A0801");
        $(this).children("table").hide();
      });      

    $("<%=idLink %>").addClass("current").children(".drop_down").css("background", "url('/Zip/Image?path=/public/images/menu/bg_drop_down.png') no-repeat left");
    if ('<%=idLink%>' == '#aCurrentAuction') {
      var elem = $("<%=idLink %>");
      if (!elem.hasClass("oneMainCategory")) {
        elem.children(".drop_down").css("background", "url('/Zip/Image?path=/public/images/menu/bg_drop_down_current.png') no-repeat left");
      }
    }
    $(".current").parent().prev().children("span").css("background", "none");
    $(".current").parent().next().children("span").css("background", "none");
  });   
</script>
