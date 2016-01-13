<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="footer">
  <div class="footer_upper_border">&nbsp;</div>
  <div class="footer_wrapper">
    <div class="footer_item">
      <h4>My Account</h4>
      <ul>
        <li>
          <%= Html.ActionLink("Sign In/Register", "LogOn", "Account") %></li>
      </ul>
    </div>
    <div class="footer_item">
      <h4>Auction</h4>
      <ul>
        <li>
          <%= Html.ActionLink("Sports Memorabilia", "SubCategory", new { controller = "Auction", action = "SubCategory", Id = 1, evnt="Sport" })%></li>
        <%--<li>
           <%= Html.ActionLink("Best of the best", "Tag", new { controller = "Auction", action = "Tag", tagID = 1 })%>
        </li>--%>
        <%--<li>
          <%= Html.ActionLink("Photo Collection", "SubCategory", new { controller = "Auction", action = "SubCategory", Id = 3, evnt = "Photo Collection" })%></li>
        <li>
          <%= Html.ActionLink("Rock 'n' Roll", "SubCategory", new { controller = "Auction", action = "SubCategory", Id = 7, evnt = "Rock-n-Roll" })%></li>--%>            
        <li>
          <%= Html.ActionLink("Americana", "SubCategory", new { controller = "Auction", action = "SubCategory", Id = 2, evnt = "Americana" })%></li>            
        <li>
          <%= Html.ActionLink("Auction Updates", "AuctionUpdates", new { controller = "Auction", action = "AuctionUpdates"})%></li>
      </ul>
    </div>
    <div class="footer_item">
      <h4>About Us</h4>
      <ul>
        <li><%=Html.ActionLink("Lelands History", "About", new { controller = "Home", action = "About" })%></li>
        <li><a href="/Home/About#specialist">Lelands Specialist</a></li>
        <li><a href="http://blog.lelands.com" target="_blank">Lelands Buzz</a></li><%-- <%=Html.ActionLink("Lelands Buzz", "Buzz", new { controller = "Home", action = "Buzz" })%>--%>
      </ul>
    </div>
    <div class="footer_item">
      <h4>Help</h4>
      <ul>
        <li>
          <%= Html.ActionLink("Consign To Auction", "Consign", new { controller = "Home", action = "Consign" })%></li>
        <li>
          <%= Html.ActionLink("Web Rules/Terms", "FAQs", new { controller = "Home", action = "FAQs" })%></li>            
        <li>
          <%= Html.ActionLink("Contact Us", "ContactUs", new { controller = "Home", action = "ContactUs", page="" })%>
          </li>
        <li>
          <%= Html.ActionLink("Advanced Search", "AdvancedSearch", new { controller = "Home", action = "AdvancedSearch" })%></li>
      </ul>
    </div>
    <div class="footer_item">
      <h4>Extras</h4>
      <ul>
        <li><%= Html.ActionLink("Join Our Mailing List", "FreeEmailAlertsRegister", "Home")%></li>            
        <li><%= Html.ActionLink("Past Auction Results", "PastAuctionResults", new { controller = "Auction", action = "PastAuctionResults" })%></li>
        <li><%= Html.ActionLink("Related Links", "RelatedLinks", new { controller = "Home", action = "RelatedLinks" })%></li>
        <%--<li><%= Html.ActionLink("For Sale", "ForSale", new { controller = "Home", action = "ForSale" })%></li>--%>
        <li><%= Html.ActionLink("Unsubscribe From Free Email Alerts", "FreeEmailAlertsUnregister", "Home")%></li>
        <%--<li><a href="/ECatalog/912/Index.html" title="Online November 2010 Auction Catalogue" target="_blank">Online Catalogue</a></li>--%>
      </ul>
      <div style="float:left;margin:0;padding:0">        
        <a href="http://www.facebook.com/pages/Lelandscom-Auctions/222277494452270" target="_blank" style="border:none;text-decoration:none">
          <img src="<%=AppHelper.CompressImage("facebook.png") %>" title='Follow us on Facebook' alt='Follow us on Facebook' style="border:none" />
        </a>
        &nbsp;&nbsp;
        <a href="http://twitter.com/#!/lelandsdotcom" target="_blank" style="border:none;text-decoration:none">
          <img src='<%=AppHelper.CompressImage("twitter.png") %>' title='Follow us on Twitter' alt='Follow us on Twitter'  style="border:none" />
        </a> 
      </div>
    </div>
  </div>
   <div class="footer_lower_border">&nbsp;</div>
  <div class="footer_wrapper_address">
    Lelands.com | 130 Knickerbocker Ave ~ Suite E | Bohemia, NY 11716 | tel: 631.244.0077 | fax: 631.750.5216<br />
    <b>To Consign:</b> <a href="mailto:kevin@lelands.com">kevin@lelands.com</a> | <b>Customer Service</b>: <a href="mailto:laura@lelands.com">laura@lelands.com</a> | <b>General Questions</b>: <a href="mailto:info@lelands.com">info@lelands.com</a><br/>
    &#169; 1988 - <%=DateTime.Now.Year %> Lelands.com, All rights reserved.
  </div>
</div>