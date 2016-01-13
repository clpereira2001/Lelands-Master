<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ResendConfirmationCode>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Resend Confirmation Code < <%:Consts.CompanyTitleName %></title>
  <script src="<%=AppHelper.ScriptUrl("hashtable.js") %>" type="text/javascript"></script>
  <script src="<%=AppHelper.ScriptUrl("validation.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column" style="width: 180px"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content" style="padding-left: 10px;">
    <div class="page_title">Resend Confirmation Code</div>
    <% using (Html.BeginForm()){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
    Enter your <u>registered</u> e-mail address below.
    <br />
    <br />
    Email<em>*</em>&nbsp;&nbsp;&nbsp;
    <%= Html.TextBox("Email", "", new { @size = "30", @style = "border:1px solid #6990AF" })%>&nbsp;&nbsp;
    <%= Html.SubmitWithClientValidation("Send")%>
    <br />
    <font style="color: #FA3232">
      <%= Html.ValidationMessageArea("Email")%>
    </font>
    <% } %>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_0.jpg") %>');
    });
  </script>
</asp:Content>
