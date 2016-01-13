<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Registration Confirm - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">    
    <div class="page_title">Registration Confirm</div>
      <b>Congratulations, you are now registered with our site.</b><br /><br />
      We are sending a confirmation e-mail to you. Once you have received your confirmation
      message, you must confirm your email address and after this you will be approved. Approval can take 24-48 hours.      
      You will not be able to participate in Lelands Auctions until your account is approved.
      
      <br />
      If you don't receive your confirmation link, please <%=Html.ActionLink("click here", "ResendConfirmationCode", new {controller="Account", action="ResendConfirmationCode"}, new {@style="font-weight:bold;font-size:14px" }) %> to resend.
  </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
 <%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_0.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>