<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Different Images - <%=Consts.CompanyTitleName%></title>
  <% Html.Style("uploadify.css"); %>
  <% Html.Script("swfobject.js"); %>
  <% Html.Script("jquery.uploadify.v2.1.4.min.js"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Different Images</h2>
  <table id="img_list"></table>
  <div id="img_pager"></div>  
  <div id="form_iupload" title="Upload email images" class="dialog_form">    
    <input type="file" id="biuFile" />
  </div>  
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">  
  <%Html.ScriptV("DifferentImages.js"); %>
</asp:Content>
