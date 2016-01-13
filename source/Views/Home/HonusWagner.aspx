<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Honus Wagner - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 <div style="text-align:justify; padding:0px 20px 20px 20px;font-size:12px">
    <div class="page_title">HONUS WAGNER CARD</div>
    <div style="margin:10px">
      The rarest of all vintage Honus Wagner cards with only two copies in the PSA population report and no copies graded by SGC. Issued in Louisville Kentucky during the late 1890s it advertises for Honus Wagner 10 Cent Cigar. This copy, deemed authentic, presents as VG due to a horizontal crease.
    </div>
    <div style="text-align:center">
      <img src="/public/images/temp/68919a.jpg" /><img src="/public/images/temp/68919b.jpg" />
    </div>
  </div>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="leftImg" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="subMenu" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphJS" runat="server">
</asp:Content>
