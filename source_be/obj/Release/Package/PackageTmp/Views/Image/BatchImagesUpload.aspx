<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Batch Image Upload - <%=Consts.CompanyTitleName%></title>
  <% Html.Style("uploadify.css"); %>
  <% Html.Script("swfobject.js"); %>
  <% Html.Script("jquery.uploadify.v2.1.4.min.js"); %>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Batch Images Upload</h2>
  <table id="img_list"></table>
  <div id="img_pager"></div>
  
  <div id="form_iupload" title="Batch image upload" class="dialog_form">    
    <input type="file" id="biuFile" />
  </div>
  <% Html.RenderPartial("~/Views/Image/BatchUploadEventForm.ascx"); %>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <script src="<%=ResolveUrl("~/Scripts/VAuction/BatchImages.js") %>" type="text/javascript"></script>
  <%--<%Html.ScriptV("BatchImages.js"); %>--%>
  <%Html.ScriptV("BatchUploadEventForm.js"); %>
</asp:Content>
