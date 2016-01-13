<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Resend Confirmation Code Success - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column" style="width: 180px"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content" style="padding-left:10px">
	<div class="page_title">Resend Confirmation Code Success</div>
    An e-mail with your confirmation information has been sent.
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