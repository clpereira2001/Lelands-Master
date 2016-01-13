<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RegisterInfo>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Registration < <%=Consts.CompanyTitleName %></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column" style="width:180px"><img <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_3.jpg")+"'" : String.Empty %> alt="" width="180" height="600" /></div>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="registration" class="control">    
	    <div class="page_title">Registration</div>
	    Please enter your information below. You will be notified by email as soon as you have been approved. <br /> Approval for bidding can take from 24-48 hours.<br /><br />	    
	    <span class="info_msg">Items with <strong style="color:#FA3232">*</strong> are required.</span>	    
	    <br />
        <% using (Html.BeginForm()) { %>
        <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
        <div>
  <div class="left_column">
    <fieldset class="blue_box" style="margin: 0; padding: 0px;">
      <p>  
          <font style="font-size:12px">
          <%= Html.ValidationMessageArea("Login")%>
          </font>

          <label for="username" >Create Bidder ID<em>*</em></label>
          <%= Html.TextBox("Login", Model.Login, new { @size = "50" })%>
          <br />          
      </p>
      <p>
          <%= Html.ValidationMessageArea("Email")%> 
          <label for="username" >Email<em>*</em></label>
          <%= Html.TextBox("Email", Model.Email, new { @size = "50" })%>
          <br />                      
      </p>
      <p>  
          <%= Html.ValidationMessageArea("ConfirmEmail")%>             
          <label for="username">Confirm email<em>*</em></label>                
          
            <%= Html.TextBox("ConfirmEmail", Model.ConfirmEmail, new { @size = "50" }) %>
          <br />
       
      </p>
      <p>
          <%= Html.ValidationMessageArea("Password")%>
          <label for="username">Create password<em>*</em></label>                
          <%= Html.Password("Password", Model.Password, new { @size = "30"})%>
          <br />
         
      </p>
      <p>
          <%= Html.ValidationMessageArea("ConfirmPassword")%>
          <label for="username">Confirm password<em>*</em></label>
          <%= Html.Password("ConfirmPassword", Model.ConfirmPassword, new { @size = "30" })%>
          <br />                
      </p>
   </fieldset>
  </div>
    <div class="right_column" >
        <fieldset id="fsPhones" class="blue_box" style="margin: 0; padding: 0">
        <p>
            <%= Html.ValidationMessageArea("DayPhone")%>
            <label>Phone (1)<em>*</em></label>
            <%= Html.TextBox("DayPhone", Model.DayPhone, new { @size = "22" })%>
            <ul class="registration_phone_input" id="hDayPhone"><li>Formats for US phone numbers:<br /><b>'XXX-XXX-XXXX'</b> or <b>'XXX-XXX-XXXX EXTXXXX'</b><br />example: 631-244-0077</li><li>Format for the other countries phone numbers: <br /><b>'XXXXXXXXXXXXXX'</b>, example: 0123456789</li></ul>
            <br id="hbr1" />
        </p>       
        <p>
            <%= Html.ValidationMessageArea("EveningPhone")%>
            <label>Phone (2)</label>
            <%= Html.TextBox("EveningPhone", Model.EveningPhone, new { @size = "22" })%>            
            <br /> 
        </p>           
        <p>
            <%= Html.ValidationMessageArea("MobilePhone")%>
            <label>Mobile Phone</label>
            <%= Html.TextBox("MobilePhone", Model.MobilePhone, new { @size = "22" })%>            
            <br />
        </p>           
        <p>
           <%= Html.ValidationMessageArea("Fax")%>
            <label>Fax</label>
            <%= Html.TextBox("Fax", Model.Fax, new {  @size = "22" })%>           
            <br />
        </p>
        <p>
            <%= Html.ValidationMessageArea("TaxpayerID")%>
            <label>Taxpayer ID</label>
            <%= Html.TextBox("TaxpayerID", Model.TaxpayerID, new { @size = "20" })%>
            <br />               
        </p>
    </fieldset>
    </div>
</div>        
      <br clear="all">
      <div class="left_column">
        <fieldset id="billing_info" class="blue_box">
            <h3>Billing information</h3>
            <p class="left">
                Please enter your billing address exactly as it appears on your<br /> credit card statement
            </p>
            <p>
                <%= Html.ValidationMessageArea("BillingFirstName")%>
                <label>First Name<em>*</em></label>
                <%= Html.TextBox("BillingFirstName", Model.BillingFirstName, new { @size = "50" })%>
                <br />
            </p>        
            <p>
                <%= Html.ValidationMessageArea("BillingLastName")%>
                <label>Last Name<em>*</em></label>
                <%= Html.TextBox("BillingLastName", Model.BillingLastName, new { @size = "50" })%>
                <br />
            </p>
            <p>
                <%= Html.ValidationMessageArea("BillingCompany")%>
                <label>Company</label>
                <%= Html.TextBox("BillingCompany", Model.BillingCompany, new { @size = "60" })%>
                <br />                
            </p>
                        
            <p>
                <%= Html.ValidationMessageArea("BillingAddress1")%>
                <label>Address (1)<em>*</em></label>
                
                <%= Html.TextBox("BillingAddress1", Model.BillingAddress1, new { @size = "65" })%>
                <br />                
            </p>                         
            
            <p>
                <%= Html.ValidationMessageArea("BillingAddress2")%>
                <label>Address (2)</label>
                <%= Html.TextBox("BillingAddress2", Model.BillingAddress2, new { @size = "65" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingCity")%>
                <label>City<em>*</em></label>
              
                <%= Html.TextBox("BillingCity", Model.BillingCity, new {  @size = "30" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingState")%>
                <label>State/Province<em>*</em></label>
                
                <%= Html.DropDownList("BillingState", (IEnumerable<SelectListItem>)ViewData["States"])%>
                <br />                
            </p>

            <p>
                <%= Html.ValidationMessageArea("BillingInternationalState")%>
                <label>International State</label>
                <%= Html.TextBox("BillingInternationalState", Model.BillingInternationalState, new { @size = "30" })%>
                <br />
                
            </p>
            
            <p>                                         
                <%= Html.ValidationMessageArea("BillingZip")%>
                <label>Zip<em>*</em></label>
                
                <%= Html.TextBox("BillingZip", Model.BillingZip, new {  @size = "6" })%>
                <br />
                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingCountry")%>
                <label>Country<em>*</em></label>
                
                <%= Html.DropDownList("BillingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"])%>
                <br />
                
            </p>
        </fieldset>
      </div>
      <div class="right_column">
        <fieldset id="shipping_info" class="blue_box">
            <h3>Shipping information</h3>
            <p class="left">
                <%= Html.CheckBox("BillingLikeShipping", Model.BillingLikeShipping, new { @class = "chk" })%> <%--, @onclick = "CheckShippingInformation()" --%>
               &nbsp &nbsp My billing and shipping addresses are identical
            </p>
            <br />
            <p>
                <%= Html.ValidationMessageArea("ShippingAddress1")%>
                <label>Address (1)<em>*</em></label>                
                <%= Html.TextBox("ShippingAddress1", Model.ShippingAddress1, new { @size = "65" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingAddress2")%>
                <label>Address (2)</label>
                <%= Html.TextBox("ShippingAddress2", Model.ShippingAddress2, new {  @size = "65" })%>
                <br />
                
            </p>                            
            
            <p>
                <%= Html.ValidationMessageArea("ShippingCity")%>
                <label>City<em>*</em></label>                
                <%= Html.TextBox("ShippingCity", Model.ShippingCity, new { @size = "30" })%>
                <br />
                
            </p>

            <p>
                <%= Html.ValidationMessageArea("ShippingState")%>
                <label>State/Province<em>*</em></label>                
                <%= Html.DropDownList("ShippingState", (IEnumerable<SelectListItem>)ViewData["States"])%>
                <br />
                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingInternationalState")%>
                <label>International State</label>
                <%= Html.TextBox("ShippingInternationalState", Model.ShippingInternationalState, new { @size = "30" })%>
                <br />
                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingZip")%>
                <label>Zip<em>*</em></label>                
                <%= Html.TextBox("ShippingZip", Model.ShippingZip, new {  @size = "6" })%>
                <br />
                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingCountry")%>
                <label>Country<em>*</em></label>                
                <%= Html.DropDownList("ShippingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"])%>
                <br />
                
            </p>
                        
        </fieldset>
       </div>
       <br clear="all" /> 
       <span class="info_msg">Please list references to past auction houses that you have bid in</span>       
       <br clear="all" />
       <div class="left_column" style="font-size:12px"> 
        <%= Html.CheckBox("NoReferencesAvailable", Model.NoReferencesAvailable)%>&nbsp;&nbsp;No references available.<br /><span style="color:Maroon;">Check box if you don't have references.</span>
       </div>
        <div class="right_column" style="font-size:12px">          
          <%= Html.CheckBox("RecievingOutBidNotice", Model.RecievingOutBidNotice)%>&nbsp;&nbsp;I want to recieve OutBid notices
        </div>
       <br clear="all" id="brL1" />
       <div class="left_column" id="dRef1">
        <fieldset id="fsRef1" class="blue_box">
            <h3>Reference 1</h3>
            <p>
                <%= Html.ValidationMessageArea("Reference1AuctionHouse")%>
                <label>Auction House</label>
                <%= Html.TextBox("Reference1AuctionHouse", Model.Reference1AuctionHouse, new { @size = "70" })%>
                <br />
                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("Reference1PhoneNumber")%>
                <label>Phone Number</label>
                <%= Html.TextBox("Reference1PhoneNumber", Model.Reference1PhoneNumber, new { @size = "60" })%>                
                <br />
            </p>

            <p>
                <%= Html.ValidationMessageArea("Reference1LastBidPlaced")%>
                <label>Last Bid Date</label>
                <%= Html.TextBox("Reference1LastBidPlaced", Model.Reference1LastBidPlaced, new { @size = "10" })%>
                <br />
            </p>                                  
                      
        </fieldset>
       </div>
       <div class="right_column" id="dRef2">
        <fieldset id="fsRef2" class="blue_box">
            <h3>Reference 2</h3>            
            
            <p>
                <%= Html.ValidationMessageArea("Reference2AuctionHouse")%>
                <label>Auction House</label>
                <%= Html.TextBox("Reference2AuctionHouse", Model.Reference2AuctionHouse, new { @size = "70" })%>
                <br />
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("Reference2PhoneNumber")%>
                <label>Phone Number</label>
                <%= Html.TextBox("Reference2PhoneNumber", Model.Reference2PhoneNumber, new { @size = "60" })%>
                <br />
            </p>

            <p>
                <%= Html.ValidationMessageArea("Reference2LastBidPlaced")%>
                <label>Last Bid Date</label>
                <%= Html.TextBox("Reference2LastBidPlaced", Model.Reference2LastBidPlaced, new { @size = "10" })%>
                <br />                
            </p>                                  
                      
        </fieldset>
       </div>
       
       <br cleaner=all id="brL2" />    
       <div class="left_column"  id="dEbay"> 
        <fieldset id="ebuy_info" class="blue_box">
            <h3>Ebay Reference Information</h3>            
            
            <p>
                <%= Html.ValidationMessageArea("EbayID")%>
                <label>Bidder ID</label>
                <%= Html.TextBox("EbayID", Model.EbayID, new { @size = "20" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("EbayFeedback")%>
                <label>Feedback</label>
                <%= Html.TextBox("EbayFeedback", Model.EbayFeedback, new { @size = "300" })%>
                <br />
            </p>        
        </fieldset>
        </div>       
        <br clear=all /><br clear=all />
        <div >
        <div style="vertical-align:middle !important;font-size:14px;">                              
                   <%=Html.CheckBox("Agree", new { @class = "chk", @onclick = "EnableDisableButton()" })%>&nbsp;I agree with the <%= Html.ActionLink("Terms and Conditions", "FAQs", "Home", new { controller = "Home", action = "FAQs" }, new { style="font-size:14px;"})%>
                   <%= Html.ValidationMessageArea("Agree")%>   
                </div>                           
        <div class="text_center">        
            <%= Html.SubmitWithClientValidation("Register", "register")%>
        </div>
       <% } %>
    </div>    
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
  <script type="text/javascript">
    function EnableDisableButton() {
      var check = document.getElementById("Agree").checked;
      document.getElementById("btSubmit").disabled = !check;
    }

    function BillingLikeShipping() {
      if ($("#BillingLikeShipping").attr("checked")) {
        $("#ShippingAddress1,#ShippingAddress2,#ShippingCity,#ShippingZip,#ShippingInternationalState,#ShippingState,#ShippingCountry").attr("disabled", "true");
        $("#ShippingAddress1").attr("value", $("#BillingAddress1").attr("value"));
        $("#ShippingAddress2").attr("value", $("#BillingAddress2").attr("value"));
        $("#ShippingCity").attr("value", $("#BillingCity").attr("value"));
        $("#ShippingZip").attr("value", $("#BillingZip").attr("value"));
        $("#ShippingInternationalState").attr("value", $("#BillingInternationalState").attr("value"));
        $("#ShippingState").val($("#BillingState").val());
        $("#ShippingCountry").val($("#BillingCountry").val());
      } else {
        $("#ShippingAddress1,#ShippingAddress2,#ShippingCity,#ShippingZip,#ShippingInternationalState,#ShippingState,#ShippingCountry").removeAttr("disabled");
        $("#ShippingAddress1").attr("value", "<%=Model.ShippingAddress1 %>");
        $("#ShippingAddress2").attr("value", "<%=Model.ShippingAddress2 %>");
        $("#ShippingCity").attr("value", "<%=Model.ShippingCity %>");
        $("#ShippingZip").attr("value", "<%=Model.ShippingZip %>");
        $("#ShippingInternationalState").attr("value", "<%=Model.ShippingInternationalState %>");
        $("#ShippingCountry").val(parseInt("<%=Model.ShippingCountry%>"));
      }
    }

    function NoReferenceAvailable() {
      if ($("#NoReferencesAvailable").attr("checked")) {
        $("#dRef1, #dRef2, #dEbay, #brL1, #brL2").hide();
      } else {
        $("#dRef1, #dRef2, #dEbay, #brL1, #brL2").show();
      }
    }

    $(document).ready(function () {
      $("#fsPhones #DayPhone").focus(function () { $("#hDayPhone").show(); $("#hbr1").hide(); }).blur(function () { $("#hDayPhone").hide(); $("#hbr1").show(); });

      $("#NoReferencesAvailable").click(function () {
        NoReferenceAvailable();
      });

      $("#BillingLikeShipping").click(function () {
        BillingLikeShipping();
      });

      $("#BillingCountry").val(parseInt("<%=Model.BillingCountry%>"))
      BillingLikeShipping();
      NoReferenceAvailable();

      EnableDisableButton();
    });
  </script>
  <%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_3.jpg") %>');     
    });
  </script>
  <%} %>
</asp:Content>