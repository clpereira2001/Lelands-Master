<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Vauction.Models.CustomModels.ConsignForm>" %>

<%@ Import Namespace="SSB.Web.Mvc.Html" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Consign To Auction - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
<div style="text-align:justify; padding:0px 20px 20px 20px;font-size:14px;">
	<div class="page_title">Consign to Lelands</div>
      Thinking about consigning…<br />
Lelands.com the first, foremost and #1 Sports Memorabilia and Americana Auction House is primed and ready to give you a free appraisal of your fine collectibles.  If you have memorabilia you wish to sell, let a Lelands specialist assist you through the whole process 
call 631-244-0077
<br /><br />
<h4><i>Why Choose Lelands Auctions:</i></h4>
    
    <h4>PRICES</h4>
    We will realize you the best prices in the field BAR NONE. Why sell blindly when you can maximize through consignment at auction. Yes, we may also buy outright if that is a must option for you. We get more so we can afford to pay you more. Call now and we will discuss both options. <br /><br />
    
    <h4>CASH ADVANCES</h4>
    Generous cash advances may be paid on quality auction consignments. Up to $1,000,000 is ready at all times.
    <br /><br />
    
    <h4>APPRAISALS</h4>
    Call today for a FREE and HONEST appraisal. Our founder is the first bonafide sports appraiser in America.
    <br /><br />
 
    <h4>THE FIRST</h4>
    We are THE ORIGINAL sports auction house in the world and reign supreme in our quality, service, attention and results. Our staff of experts boasts well over 100 years of experience in the field and is known for their honesty, trustworthiness and care.
    <br /><br />

    <h4>INTERNET</h4>
    Our website is THE JEWEL OF THE INDUSTRY. The most maneuverable site, we sport real time, on-line bidding. Our hits number the best in the biz and we are visited by not only the rich and famous, but the real collector as well. Please judge for yourself and come visit us at www.lelands.com.
    <br /><br />

    <h4>CATALOGUES</h4>
    Our AWARD WINNING, state-of-the-art, full color auction catalogues are a sight to behold. We present your collectibles in the best possible light which translates into more dollars in your pocket. We boast the largest mailing list in the industry which has been culled over the last 20 years and includes over 15,000 bonafide buyers.
    <br /><br />

    <h4>COMMISSIONS</h4>
    Our commissions are competitive and negotiable depending upon the size of your holdings. There are NO hidden fees and charges.
    <br /><br />

    <h4>AUCTIONS</h4>
    Our auctions sport the best of the best. We have sold such important collections as the Charlie Sheen Collection, The Mickey Mantle Collection, the Boston Garden Auction, and so many more. We have handled material from such important player estates as Lou Gehrig, Nellie Fox, Guy Lafleur, Sammy Baugh, Brad Park, Howie Morenz, Mickey Mantle, Pete Rose, Mario Lemieux, Jackie Robinson and many, many more .
    <br /><br />

    <h4>PUBLICITY</h4>
    We get the best publicity in the business by far. To name just a few we have appeared in Sports Illustrated 16 times, been the entire front page of the New York Daily News, appeared on ABC’s Nightline, and even been in The Tonight Show monologue for our famed selling of “The Buckner Ball.”<%-- Our PR is exclusively handled by Drotman Communications, specializing in sports marketing and public relations for nearly twenty years.--%><br /><br />

    <h4>CREDIBILITY</h4>
    Our reputation for authenticity and credibility is unparalleled. We are less interested in making the last dollar as doing the right thing. That is something few others can say.
    <br /><br />
    
    <h4>MAJOR LEAGUE BASEBALL®</h4>
    We were the official auctioneers for the 2004 Major League All-Star FanFest.
    <br /><br />
    
    <h4>PURCHASING</h4>
    Lelands.com will also buy your memorabilia or collections outright for cash versus consignment. Please contact us today for an offer.
    <br /><br />
    
    <h4 id="contactus">If you are interested in consigning to our upcoming auction please fill out our brief contact form with a short description about your fine collectible. Or, if it's easier you can call one of our Lelands specialists at 631-244-0077, 9 AM - 6 PM EST</h4>
    <form action="/Home/ContactUsMessage" method="post">    
    <div id="registration" class="control">
    <div class="left_column" style="width:350px">
        <fieldset class="blue_box" style="margin: 0; padding: 0px;">        
            <p>
                <label>First Name:<em>*</em></label>
                <%= Html.TextBox("FirstName", "", new { @size = "50" })%>
                <%= Html.ValidationMessageArea("FirstName")%>          
            </p><br/>
            <p>
                <label>Last Name:<em>*</em></label>
                <%= Html.TextBox("LastName", "", new { @size = "50" })%>
                <%= Html.ValidationMessageArea("LastName")%>  
            </p><br/>
            <p>            
                <label>Email:<em>*</em></label>
                <%= Html.TextBox("Email", "", new { @size = "50" })%>                
                <%= Html.ValidationMessageArea("Email")%>  
            </p><br/>
            <p>            
                <label>Phone:</label>
                <%= Html.TextBox("Phone", "", new { @size = "50" })%>  
                <%= Html.ValidationMessageArea("Phone")%>  
            </p>           
            <br />
             <p>            
                <label>Best time to call:</label>
                <%= Html.TextBox("BestTime", "", new { @size = "50" })%>                
            </p> 
            <br />
             <p >
                <label >Description of item(s):<em>*</em></label>
                <%= Html.TextArea("Description", "", new { @style = "width:330px;border:solid 1px #580B01" })%>                
                <%= Html.ValidationMessageArea("Description")%>  
            </p> <br/>
            <p>
              
              <%= Html.MvcCaptcha()%>
              <label for="captchaValue">Please enter the text shown on the picture: <em>*</em></label>
          <%= Html.TextBox("captchaValue", "")%><br/>
          <%= Html.ValidationMessageArea("captchaValue")%>  
          </p>
            <%= Html.SubmitWithClientValidation("SEND")%>
            <%--<div class="custom_button" id="btnSearchType1" style="width:65px">
              <button type="submit" >Send</button>
            </div>            --%>
        </fieldset>        
      </div>
    </div>    
    </form>
</div>    
</asp:Content>