<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Home Page - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
  <h1>Hello, <%=(AppHelper.CurrentUser!=null)?AppHelper.CurrentUser.FirstName:"---" %></h1>
  <%--User role: <%=(AppHelper.CurrentUser==null)?"---":((Vauction.Utils.Consts.UserTypes)AppHelper.CurrentUser.UserType).ToString() %>--%>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">  
</asp:Content>
