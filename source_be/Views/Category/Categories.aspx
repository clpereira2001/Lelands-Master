<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Categories - <%=Consts.CompanyTitleName%></title>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    Categories</h2>
  <table id="tblSchema">
    <tr>
      <td>
        <table id="mc_list"></table>
        <div id="mc_pager"></div>
        <br />
        <table id="c_list"></table>
        <div id="c_pager"></div>
      </td>
      <td>
        <table id="cm_list"></table>
        <div id="cm_pager"></div>
      </td>
    </tr>
  </table>
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dMainCategory", Title = "Main category list", Method = "/Category/GetMainCategoriesList/", SortName = "Title", SortOrder = "asc", ColumnHeaders = "'Main Category#', 'Title'", Columns = "{ name: 'MainCat_ID', index: 'MainCat_ID', width: 70, key: true }, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_maincategory", ResultShow = "ret.Title", ResultID = "ret.MainCat_ID" }); %>
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dCategory", Title = "Category list", Method = "/Category/GetCategoriesList/", SortName = "Title", SortOrder = "asc", ColumnHeaders = "'Categoty#', 'Title'", Columns = "{ name: 'Cat_ID', index: 'Cat_ID', width: 70, key: true }, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_category", ResultShow = "ret.Title", ResultID = "ret.Cat_ID" }); %>
  <% Html.RenderPartial("~/Views/Category/CategoriesMergingForm.ascx"); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="jsContent" runat="server">  
  <%Html.ScriptV("Categories.js"); %>
  <%Html.ScriptV("CategoriesMergingForm.js"); %>
</asp:Content>