<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Log On - <%: Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
  <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server"> 
  <div class="center_content"> 
    <div class="page_title">Log On</div>
    <b>You must login before bidding on lots in the auction</b>
    <table id="logon_table" cellpadding="10" cellspacing="10">
      <tr>
        <td rowspan="2" style="//padding:0px;">
          <div id="login_control" class="control blue_box" style="width:320px; padding-bottom:15px;">
          <h4>Already Registered?</h4>                
          <p>Enter your User ID and password below to sign-in</p>        
        <% using (Html.BeginForm()) { %>          
            <p class="p_t_10"> 
              <label for="login">User ID:</label><em>*</em><%= Html.TextBox("login", "", new { @style="//float:none;//margin-left:90px" })%>
              <br />
              <%= Html.ValidationMessage("login")%>
            </p>
            
            <p class="p_t_10">
                <label for="password">Password:</label><em>*</em><%= Html.Password("password", "", new { @style = "//float:none;//margin-left:73px" })%>
                <br />
                <%= Html.ValidationMessage("password") %>
            </p>
            <div style="vertical-align:bottom">
            <p class="p_t_10" style="float:left;">
                <span class="remember" for="rememberMe" style="//float:left;">Remember me</span><%= Html.CheckBox("rememberMe", false, new { @class = "remember chk", @style="//float:left" })%>                                
            </P>                            
            </div>
            <br />
            <div class="custom_button" style="float:right">
              <button type="submit" >Login</button>
            </div>
            <br />
            <p>
                <%= Html.ActionLink("Forgot your password?", "ForgotPassword")%> 
            </p>
        <% } %>        
    </div>
        
        </td>
        <td style="//padding:0px;" >
            <div id="login_control" class="control blue_box" style="width:365px;">
              <h4>New Customer?</h4>
              <p>Create an account to save your shipping and billing information</p>
              <div class="description description_text">If you are not sure if you have an account with us,  please call first before registering 631-244-0077
              </div>
              <br />
              <div class="custom_button" style="float:left;">
                <button type="button" onclick="window.location='<%= Url.Action("Register", "Account") %>'">Register</button>
              </div>
              <br />
              <p class="p_t_30"><%= Html.ActionLink("Resend Confirmation Code", "ResendConfirmationCode")%></p>
          </div>
        </td>
      </tr>
      <tr>
        <td style="//padding:0px;">
          <div id="login_control" style="width:365px;">          
              <h4>Join our mailing list?</h4>
             <div class="description">By joining our mailing you receive our Newsletters, as well as auction alerts and information on other important Lelands.com events!</div>
               <div class="custom_button" style="float:left;">
                  <button type="button" onclick="window.location='<%= Url.Action ("FreeEmailAlertsRegister", "Home")%>'">Register</button>
                </div>          
           </div>
        </td>
      </tr>
    </table>
</div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_2.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>