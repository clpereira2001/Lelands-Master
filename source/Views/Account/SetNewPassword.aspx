<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RegisterInfo>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<%@ Import Namespace="Vauction.Utils.Perfomance" %>
<asp:Content ID="resetpasswHead" ContentPlaceHolderID="head" runat="server">
  <title>Reset Password < <%=Consts.CompanyTitleName %></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
  <div id="left_column">
    <img alt="" width="173" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_2.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="resetpasswContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">
      Set New Password</div>
    <div id="registration" class="control">
      <% using (Html.BeginForm()){ %>
      <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
      Enter your new password and confirm it to reset your current password.
      <br />
      <%=Html.Hidden("user_id", Model.ID) %>
      <fieldset id="info" class="blue_box">
        <p>
          <%= Html.ValidationMessageArea("Password")%>
          <label for="username">
            Password<em>*</em></label>
          <%= Html.Password("Password", Model.Password, new { @size = "30"})%>
          <br />
        </p>
        <p>
          <%= Html.ValidationMessageArea("ConfirmPassword")%>
          <label for="username">
            Confirm password<em>*</em></label>
          <%= Html.Password("ConfirmPassword", Model.ConfirmPassword , new { @size = "30" })%>
          <br />
        </p>
      </fieldset>
      <div class="text_center">
        <%= Html.SubmitWithClientValidation("Save")%>
      </div>
      <% } %>
    </div>
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
