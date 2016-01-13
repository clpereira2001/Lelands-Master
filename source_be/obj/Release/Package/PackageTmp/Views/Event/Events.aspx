<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Events - <%=Consts.CompanyTitleName%></title>  
  <%= Html.CompressCss(Url) %>
  <%= Html.CompressJs(Url) %>
  <% Html.Clear(); %>

  <link href="<%=this.ResolveUrl("~/Scripts/jsTree/themes/checkbox/style.css") %>" rel="stylesheet" type="text/css" />
  <script src="<%=this.ResolveUrl("~/Scripts/jsTree/jquery.tree.js") %>" type="text/javascript"></script>
  <script src="<%=this.ResolveUrl("~/Scripts/jsTree/plugins/jquery.tree.checkbox.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Events</h2>
  <table id="e_list"></table>
  <div id="e_pager"></div>
 <% Html.RenderPartial("~/Views/Event/EventForm.ascx"); %>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <%Html.ScriptV("Events.js"); %>
  <%Html.ScriptV("EventForm.js"); %>  
</asp:Content>