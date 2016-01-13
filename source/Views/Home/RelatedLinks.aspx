<%@ Page Language="C#"  MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Related Links - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_0.jpg")+"'" : String.Empty %> /></div>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Related Links</div>
    <ol>
    <li>
	<span style="font-size:10pt; color:#000046">WORLD'S LARGEST INFORMATION SOURCE FOR SPORTS DISPLAY ANTIQUES</span>
	<br />
	<a title="http://www.SportsAntiques.com" href="http://www.SportsAntiques.com" style="font-size:14px; font-weight:bold">www.SportsAntiques.com</a>
	</li>
	</ol>
	</div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", "/public/images/left_side_banner_0.jpg");
    });
  </script>
  <%} %>
</asp:Content>