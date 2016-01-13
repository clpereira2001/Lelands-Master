<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<HotNew>>" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
<title>National Sports Collectors Convention - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<img src="<%=AppHelper.CompressImagePath("/public/images/hotnews/NationalConvention.jpg") %>" alt="" style="width: 100%" />
</asp:Content>
