<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Account has been updated - <%=Consts.CompanyTitleName %></title>    
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content" style="text-align:justify; padding:10px">
	<div class="page_title">Personal Information</div>
    <b>Your account has been updated.</b>        
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