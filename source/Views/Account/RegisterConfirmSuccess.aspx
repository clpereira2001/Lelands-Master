<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Successful registration < <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
  <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Registration success</div>
    <strong>Congratulations.</strong>
    <br />
    Your email address has been confirmed. You will be notified by email as soon as
    you have been approved. Approval can take 24-48 hours.
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