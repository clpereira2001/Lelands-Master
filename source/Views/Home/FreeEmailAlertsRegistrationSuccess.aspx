<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Successful resitration for free email alerts - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Registration Confirm</div>
    We are sending a confirmation e-mail to you. Once you have received your confirmation
    message, you must confirm your email address and after this your registration will be approved.
    <br />
    Thank you for registering!
    <div class="back_link"><%=Html.ActionLink("Return to the Home page", "Index", new { controller = "Home", action = "Index" })%></div>
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>