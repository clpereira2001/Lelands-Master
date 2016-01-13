<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Users Management - <%=Consts.CompanyTitleName %></title>  
  <link href="../../Scripts/plugins/jquery.signaturepad.css" rel="stylesheet" />
  <!--[if lt IE 9]><script src="../../Scripts/plugins/flashcanvas.js"></script><![endif]-->
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Users Managment</h2>
  <table id="u_list"></table>
  <div id="u_pager"></div>
  <% Html.RenderPartial("~/Views/User/UserForm.ascx"); %>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
  <script type="text/javascript" src="../../Scripts/plugins/jquery.signaturepad.min.js"></script>
  <script type="text/javascript" src="../../Scripts/plugins/json2.min.js"></script>
  <script type="text/javascript">
    var users_table_usertype = "<%=ViewData["UserTypes"] %>";
    var users_table_userstatus = "<%=ViewData["UserStatuses"] %>";
    var users_table_commrates = "<%=ViewData["CommissionRates"] %>";
  </script>  
  <%Html.ScriptV("Users.js"); %>
  <script type="text/javascript" src="../../Scripts/Vauction/UserForm.js"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      $('.sigPad').signaturePad({ output: "#newSignature" });

      $("#btnChangeSignature").click(function () {
        $(".drawIt a").click();
        $("#signatureImg, #btnChangeSignature").hide();
        $('.sigPad, #btnCancelSignature').show();
      });

      $("#btnCancelSignature").click(function() {
        $(".clearButton a").click();
        $("#signatureImg, #btnChangeSignature").show();
        $('.sigPad, #btnCancelSignature').hide();
      });
    });
  </script>
</asp:Content>
