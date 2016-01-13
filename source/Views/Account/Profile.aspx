<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RegisterInfo>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>
<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Personal Information - <%=Consts.CompanyTitleName %></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column" style="width:180px"><img <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_3.jpg")+"'" : String.Empty %> alt="" width="180" height="600" /></div>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div id="registration" class="control">    
	  <div class="page_title">Personal Information</div>
    <% if (!Convert.ToBoolean(Model.IsModifyed))
    {%>
        <div class="pay_notice">Please verify your personal information and update it. If your Bidder ID is an Email address, please change it and update your personal information, because you can not use Email address as your Bidder ID.</div><br />
    <% }%>
    <span class="info_msg">Items with <strong style="color:#FA3232">*</strong> are required.</span>    
    <% using (Html.BeginForm()){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
   <div class="left_column">
        <fieldset class="blue_box"  style="margin: 0; padding: 0px;">
            <p>            
                <%= Html.ValidationMessageArea("Login")%>
                <label for="username"><strong>Bidder ID<em>*</em></strong></label>
                <%= Html.TextBox("Login", Model.Login, new { @size = "40" })%>
                <br />                                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("Email")%>                 
                <label for="username"><strong>Email<em>*</em></strong></label>
                <%= Html.TextBox("Email", Model.Email, new { @size = "40" })%>
                <br />                           
            </p>
            
            <p>            
                <%= Html.ValidationMessageArea("ConfirmEmail")%> 
                <label for="username"><strong>Confirm Email<em>*</em></strong></label>
                <%= Html.TextBox("ConfirmEmail", Model.ConfirmEmail, new { @size = "40" })%>
                <br />                               
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("Password")%>
                <label for="username">Password<em>*</em></label>                
                <%= Html.Password("Password", Model.Password, new { @size = "30"})%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ConfirmPassword")%>
                <label for="username">Confirm password<em>*</em></label>
                <%= Html.Password("ConfirmPassword", Model.ConfirmPassword , new { @size = "30" })%>                
            </p>         
         </fieldset> 
       </div>
       
       <div class="right_column">         
         <fieldset id="fsPhones" class="blue_box" style="margin: 0; padding: 0">
            <p>
                <%= Html.ValidationMessageArea("DayPhone")%>
                <label><strong>Phone (1)<em>*</em></strong></label>
                <%= Html.TextBox("DayPhone", Model.DayPhone, new { @size = "30" })%>
                <ul class="registration_phone_input" id="hDayPhone"><li>Formats for US phone numbers:<br /><b>'XXX-XXX-XXXX'</b> or <b>'XXX-XXX-XXXX EXTXXXX'</b><br />example: 631-244-0077</li><li>Format for the other countries phone numbers: <br /><b>'XXXXXXXXXXXXXX'</b>, example: 0123456789</li></ul>
                <br id="hbr1" />
            </p>   
            
            <p>
                <%= Html.ValidationMessageArea("EveningPhone")%>
                <label><strong>Phone (2)</strong></label>
                <%= Html.TextBox("EveningPhone", Model.EveningPhone, new { @size = "30" })%>                
                <br />     
            </p> 
            <p>
                <%= Html.ValidationMessageArea("MobilePhone")%>
                <label>Mobile Phone</label>
                <%= Html.TextBox("MobilePhone", Model.MobilePhone, new { @size = "30" })%>                
                <br />
            </p> 
            <p>
                <%= Html.ValidationMessageArea("Fax")%>
                <label>Fax</label>
                <%= Html.TextBox("Fax", Model.Fax, new {  @size = "30" })%>                
                <br />
            </p>

            <p>
                <%= Html.ValidationMessageArea("TaxpayerID")%>
                <label>Taxpayer ID</label>
                <%= Html.TextBox("TaxpayerID", Model.TaxpayerID, new { @size = "30" })%>
                <br />                
            </p>
        </fieldset>
      </div>
    <br clear=all />
      
      <div class="left_column">  
        <fieldset id="billing_info" class="blue_box">
            <h3>Billing information</h3>            
            
            <p class="left">
                Please enter your billing address exactly as it appears on your credit card statement
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingFirstName")%>
                <label><strong>First Name<em>*</em></strong></label>
                <%= Html.TextBox("BillingFirstName", Model.BillingFirstName)%>
                <br />                
            </p>
                        
            <p>
                <%= Html.ValidationMessageArea("BillingLastName")%>
                <label><strong>Last Name<em>*</em></strong></label>
                <%= Html.TextBox("BillingLastName", Model.BillingLastName)%>
                <br />                
            </p>
            <p>
                <%= Html.ValidationMessageArea("BillingCompany")%>
                <label>Company</label>
                <%= Html.TextBox("BillingCompany", Model.BillingCompany)%>
                <br />               
            </p>
                        
            <p>
                <%= Html.ValidationMessageArea("BillingAddress1")%>
                <label><strong>Address (1)<em>*</em></strong></label>
                <%= Html.TextBox("BillingAddress1", Model.BillingAddress1, new { @size = "60" })%>
                <br />                
            </p>                         
            
            <p>
                <%= Html.ValidationMessageArea("BillingAddress2")%>
                <label>Address (2)</label>
                <%= Html.TextBox("BillingAddress2", Model.BillingAddress2, new { @size = "60" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingCity")%>
                <label><strong>City<em>*</em></strong></label>
                <%= Html.TextBox("BillingCity", Model.BillingCity, new { @size = "60" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingState")%>
                <label><strong>State/Province<em>*</em></strong></label>
                
                <%= Html.DropDownList("BillingState", (IEnumerable<SelectListItem>)ViewData["States"])%>
                <br />                
            </p>

            <p>
                <%= Html.ValidationMessageArea("BillingInternationalState")%>
                <label>International State</label>
                <%= Html.TextBox("BillingInternationalState", Model.BillingInternationalState)%>
                <br />                
            </p>
            
            <p>                                         
                <%= Html.ValidationMessageArea("BillingZip")%>
                <label><strong>Zip<em>*</em></strong></label>
                <%= Html.TextBox("BillingZip", Model.BillingZip, new { @size = "11" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("BillingCountry")%>
                <label><strong>Country<em>*</em></strong></label>
                <%= Html.DropDownList("BillingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"])%>
                <br />               
            </p>
        </fieldset>
      </div>

      <div class="right_column">  
        <fieldset id="shipping_info" class="blue_box">
            <h3>Shipping information</h3>            
            
            <p class="left">
                <%= Html.CheckBox("BillingLikeShipping", new { @class = "chk"}) %> <%--, @onclick = "CheckShippingInformation()" --%>
                &nbsp;&nbsp;&nbsp;My billing and shipping addresses are identical
            </p>
            <br />
            <p>
                <%= Html.ValidationMessageArea("ShippingAddress1")%>
                <label><strong>Address (1)<em>*</em></strong></label>
                <%= Html.TextBox("ShippingAddress1", Model.ShippingAddress1, new { @size = "60" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingAddress2")%>
                <label>Address (2)</label>
                <%= Html.TextBox("ShippingAddress2", Model.ShippingAddress2, new { @size = "60" })%>
                <br />                
            </p>                            
            
            <p>
                <%= Html.ValidationMessageArea("ShippingCity")%>
                <label><strong>City<em>*</em></strong></label>
                <%= Html.TextBox("ShippingCity", Model.ShippingCity, new { @size = "60" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingState")%>
                <label><strong>State/Province<em>*</em></strong></label>
                <%= Html.DropDownList("ShippingState", (IEnumerable<SelectListItem>)ViewData["States"])%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingInternationalState")%>
                <label>International State</label>
                <%= Html.TextBox("ShippingInternationalState", Model.ShippingInternationalState)%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingZip")%>
                <label><strong>Zip<em>*</em></strong></label>
                <%= Html.TextBox("ShippingZip", Model.ShippingZip, new { @size = "11" })%>
                <br />                
            </p>
            
            <p>
                <%= Html.ValidationMessageArea("ShippingCountry")%>
                <label><strong>Country<em>*</em></strong></label>
                <%= Html.DropDownList("ShippingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"])%>
                <br />                
            </p>
                        
        </fieldset>
      </div> 
    <br clear=all />
       <div class="text_center">
            <%= Html.SubmitWithClientValidation("Update", "register")%>
       </div>
    <% } %>
</div>    
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
  <script type="text/javascript">
    /*function IsShipping(isfirstload) {
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
        if (!isfirstload) {
          $("#ShippingAddress1, #ShippingAddress2, #ShippingCity, #ShippingZip, #ShippingInternationalState").attr("value", "");
          $("#ShippingState").val(0);
          $("#ShippingCountry").val(parseInt("<%=Model.ShippingCountry%>"));
        }
      }
      $("#BillingLikeShipping").removeAttr("disabled");
    }*/

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

    $(document).ready(function() {
      $("#fsPhones #DayPhone").focus(function () { $("#hDayPhone").show(); $("#hbr1").hide(); }).blur(function () { $("#hDayPhone").hide(); $("#hbr1").show(); });
      $("#BillingLikeShipping").click(function () {
        BillingLikeShipping();
      });
      $("#BillingCountry").val(parseInt("<%=Model.BillingCountry%>"))
      BillingLikeShipping();
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