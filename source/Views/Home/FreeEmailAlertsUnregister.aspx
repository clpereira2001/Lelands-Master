<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content7" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Unsubscribe from free email alerts - <%=Consts.CompanyTitleName %></title>  
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div id="registration" class="control">
	  <div class="page_title">Unsubscribe from free email alerts</div>
    Enter your e-mail address below.
    <% using (Html.BeginForm("FreeEmailAlertsUnsubscribeSuccess", "Home", FormMethod.Post)){ %>
    <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
      <p>
        <label style='font-size:14px'>Email Address:</label>
        <%= Html.TextBox("Email", "", new { @size = "50" })%>
        <br />
        <font style="font-size:12px"><%= Html.ValidationMessageArea("Email")%></font>
        </p>        
        <div class="custom_button" style="float:right">
          <button type="submit">Send</button>
        </div>        
      <% }%>    
  </div>
</asp:Content>

<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
  { %>
  <script type="text/javascript">
    $(document).ready(function () {
      $("#left_column img").attr("src", '<%=AppHelper.CompressImage("left_side_banner_1.jpg") %>');
    });
  </script>
  <%} %>
</asp:Content>
