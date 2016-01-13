<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%
  SessionUser cuser = AppHelper.CurrentUser;
    if (cuser != null) {
%>
<p class="registered">
  Hello, <b><%=cuser.FirstName %></b>!
  <br /><%= Html.ActionLink("My Account", "MyAccount", "Account") %> | <%=Html.ActionLink("Watch list", "WatchBid", "Account")%> | <%= Html.ActionLink("Log Off", "LogOff", "Account") %>
  </p>
<% } else { %> 
  <%= Html.ActionLink("Register Now", "Register", "Account") %> or <%= Html.ActionLink("Sign In", "LogOn", "Account") %>
  <a href="/Home/FreeEmailAlertsRegister" style="margin:10px 0 0 0;display:block">Join Our Mailing List<img src="<%=AppHelper.CompressImage("email.png") %>" style="margin:0 0 0 5px;vertical-align:middle"  /></a>
<% } %>