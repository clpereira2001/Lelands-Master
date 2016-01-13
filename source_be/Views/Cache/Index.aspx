<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Cache < <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Cache</h2>
  <table id="c_list"></table>
  <div id="c_pager"></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jsContent" runat="server">
  <%--<%Html.ScriptV("Cache.js"); %>--%>
  <script src="/Scripts/Vauction/Cache.js" type="text/javascript"></script>
</asp:Content>