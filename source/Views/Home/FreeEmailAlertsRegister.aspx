<%@ Page Language="C#"  MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Resiter for free email alerts - <%=Consts.CompanyTitleName %></title>  
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
<% decimal version; bool isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%> 
  <div id="left_column">    
    <img alt="" width="173" height="574" <%=isIE6?"src='"+AppHelper.CompressImage("left_side_banner_1.jpg")+"'" : String.Empty %> />
  </div>
</asp:Content>
<asp:Content ID="indexContent2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="registration" class="control">
	    <div class="page_title">Register for free email alerts</div>
      <span class="info_msg">Items with <strong style="color:#FA3232">*</strong> are required.</span>      
        <% using (Html.BeginForm("FreeEmailAlertsRegistrationSuccess", "Home", FormMethod.Post)) { %>
        <%=Html.AntiForgeryToken(Consts.AntiForgeryToken) %>
        <%:Html.Hidden("IsActive", "False") %>
        <div class="left_column" style="width:375px">
        <fieldset class="blue_box" style="margin: 0; padding: 0px;">
            <p>
                <label>First Name<em>*</em></label>
                <%= Html.TextBox("FirstName", "", new { @size = "50" })%>
                <br />
                <font style="font-size:12px">
                <%= Html.ValidationMessageArea("FirstName")%>
                </font>
            </p>
            <p>
                <label>Last Name<em>*</em></label>
                <%= Html.TextBox("LastName", "", new { @size = "50" })%>
                <br />
                <font style="font-size:12px">
                <%= Html.ValidationMessageArea("LastName")%>
                </font>
            </p>
            <p>            
                <label>Email:<em>*</em></label>
                <%= Html.TextBox("Email", "", new { @size = "50" })%>
                <br />
                 <font style="font-size:12px">
                <%= Html.ValidationMessageArea("Email")%>
                </font>
            </p>
             <p>            
                <label>Confirm email:<em>*</em></label>
                <%= Html.TextBox("EmailConfirm", "", new { @size = "50" })%>
                <br />
                <font style="font-size:12px">
                <%= Html.ValidationMessageArea("EmailConfirm")%>
                </font>
            </p>            
             <p class="left">
                <%= Html.CheckBox("IsRecievingWeeklySpecials", false, new { @class = "chk" })%>&nbsp;&nbsp;Please add me to the <strong><span>private</span></strong> mailing list for auction alerts
            </p>
                <p class="left">
                <%= Html.CheckBox("IsRecievingUpdates", false, new { @class = "chk" })%>&nbsp;&nbsp; Please add me to the <strong><span>private</span></strong> mailing list for news and updates<br />
                <font style="font-size:12px">
                <%= Html.ValidationMessageArea("IsRecievingUpdates")%>  
                </font>
            </p>
            <br />
            <div class="custom_button" style="float:left">
                <button type="submit">Send</button>
            </div>
        </fieldset>        
      </div>
      <% }%>
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