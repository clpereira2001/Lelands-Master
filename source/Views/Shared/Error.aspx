<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Error - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column"><img alt="" width="181" height="461" src="<%=isIE6?AppHelper.CompressImage("left_side_banner_0.jpg"):"" %>" /></div>
</asp:Content>
<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title"><%=ViewData["Title"]!=null?ViewData["Title"]:"Sorry, an error occurred while processing your request." %></div>
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