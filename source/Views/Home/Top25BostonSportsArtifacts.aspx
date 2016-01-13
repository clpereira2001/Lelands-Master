<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
<title>Top 25 Hit List of Important Boston Sports Artifacts - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<img src="<%=AppHelper.CompressImagePath("/public/images/hotnews/boston7.jpg") %>" alt="" style="width: 100%" />
</asp:Content>
