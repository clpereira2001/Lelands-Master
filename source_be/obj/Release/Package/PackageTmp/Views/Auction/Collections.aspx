<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Collections - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 <h2>Collections</h2>
  <table id="collections_list"></table>
  <div id="collections_pager"></div>  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">  
    <% Html.ScriptV("Collections.js"); %>
</asp:Content>