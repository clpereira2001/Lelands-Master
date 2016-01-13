<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>For Sale - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <img src="<%=AppHelper.CompressImagePath("/public/images/hotnews/Unsold-List-Page-201501.jpg") %>" alt="" style="width: 100%" />   
</asp:Content>

