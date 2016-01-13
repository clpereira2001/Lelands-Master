<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="HeadContent" runat="server">
<title>General tables - <%= Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
<h2>General tables</h2>
<table>
<tr>
<td>
<table id="co_list"></table>
<div id="co_pager"></div>
</td>
<td>
<table id="com_list"></table>
<div id="com_pager"></div>
</td>
</tr>
<tr>
<td>
<table id="var_list"></table>
<div id="var_pager"></div>
</td>
<td>
<table id="st_list"></table>
<div id="st_pager"></div>
</td>
</tr>
<tr>
<td>
<table id="tagList"></table>
<div id="tagPager"></div>
</td>
</tr>
</table>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">
<%
%>
<script type="text/javascript">
  var w = (document.body.offsetWidth - 120) / 2;
  jQuery(document).ready(function () {
    var grid = jQuery("#co_list").jqGrid({
      url: '/General/CountryList/',
      datatype: "json",
      height: 300,
      width: w,
      colNames: ['Country#', 'Title', 'Code'],
      colModel: [
        { name: 'ID', index: 'Country_ID', width: 80, align: 'left', key: true, editable: true, editoptions: { readonly: true, size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
        { name: 'Title', index: 'Title', width: 150, align: 'left', editable: true, editoptions: { size: 25 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'Code', index: 'Code', width: 60, align: 'left', editable: true, editoptions: { size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" } }
      ],
      loadtext: 'Loading ...',
      rowNum: 13,
      rowList: [13, 26, 39],
      pager: '#co_pager',
      sortname: 'ID',
      viewrecords: true,
      sortorder: "asc",
      shrinkToFit: false,
      caption: 'Countries',
      //toolbar: [true, "top"]
      editurl: "/General/EditCountry/"
    });
    jQuery("#co_list").jqGrid('navGrid', '#co_pager', { edit: true, edittext: "Edit", add: true, addtext: "Save", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { grid[0].clearToolbar(); } },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete" }, {}, {});
    jQuery("#co_list").jqGrid('filterToolbar');

    var grid2 = jQuery("#com_list").jqGrid({
      url: '/General/CommissionRateList/',
      datatype: "json",
      height: 300,
      width: w,
      colNames: ['Rate#', 'Description', 'MaxPercent', 'Text Description'],
      colModel: [
        { name: 'RateID', index: 'RateID', width: 60, align: 'left', key: true, editable: true, editoptions: { readonly: true, size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
        { name: 'Description', index: 'Description', width: 150, align: 'left', editable: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'MaxPercent', index: 'MaxPercent', width: 90, align: 'left', editable: true, number: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'LongDescription', index: 'LongDescription', width: 250, align: 'left', editable: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } }
      ],
      loadtext: 'Loading ...',
      rowNum: 13,
      rowList: [13, 26, 39],
      pager: '#com_pager',
      sortname: 'RateID',
      viewrecords: true,
      sortorder: "asc",
      shrinkToFit: false,
      caption: 'Commission Rate',
      //toolbar: [true, "top"]
      editurl: "/General/EditCommissionRateList/"
    });
    jQuery("#com_list").jqGrid('navGrid', '#com_pager', { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { grid2[0].clearToolbar(); } },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete" }, {}, {});
    jQuery("#com_list").jqGrid('filterToolbar');

    var grid3 = jQuery("#var_list").jqGrid({
      url: '/General/VariableList/',
      datatype: "json",
      height: 300,
      width: w,
      colNames: ['ID#', 'Name', 'Value'],
      colModel: [
        { name: 'ID', index: 'Varibale_ID', width: 60, align: 'left', key: true, editable: true, editoptions: { readonly: true, size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
        { name: 'Name', index: 'Name', width: 150, align: 'left', editable: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'Value', index: 'Value', width: 90, align: 'left', editable: true, number: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } }
      ],
      loadtext: 'Loading ...',
      rowNum: 13,
      rowList: [13, 26, 39],
      pager: '#var_pager',
      sortname: 'ID',
      viewrecords: true,
      sortorder: "asc",
      shrinkToFit: false,
      caption: 'Variable',
      //toolbar: [true, "top"]
      editurl: "/General/EditVariableList/"
    });
    jQuery("#var_list").jqGrid('navGrid', '#var_pager', { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { grid3[0].clearToolbar(); } },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete" }, {}, {});
    jQuery("#var_list").jqGrid('filterToolbar');

    var grid4 = jQuery("#st_list").jqGrid({
      url: '/General/StateList/',
      datatype: "json",
      height: 300,
      width: w,
      colNames: ['ID#', 'Title', 'Code', 'Country#'],
      colModel: [
        { name: 'ID', index: 'State_ID', width: 60, align: 'left', key: true, editable: true, editoptions: { readonly: true, size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" } },
        { name: 'Title', index: 'Title', width: 150, align: 'left', editable: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'Code', index: 'Code', width: 90, align: 'left', editable: true, number: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'Country_ID', index: 'Country_ID', width: 90, align: 'left', editable: true, number: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } }
      ],
      loadtext: 'Loading ...',
      rowNum: 13,
      rowList: [13, 26, 39],
      pager: '#st_pager',
      sortname: 'ID',
      viewrecords: true,
      sortorder: "asc",
      shrinkToFit: false,
      caption: 'State',
      //toolbar: [true, "top"]
      editurl: "/General/EditStateList/"
    });
    jQuery("#st_list").jqGrid('navGrid', '#st_pager', { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { grid4[0].clearToolbar(); } },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete" }, {}, {});
    jQuery("#st_list").jqGrid('filterToolbar');

    var grid5 = $("#tagList").jqGrid({
      url: '/General/GetTags/',
      datatype: "json",
      height: 300,
      width: w,
      colNames: ['ID#', 'Title', 'System', 'View on FE'],
      colModel: [
        { name: 'ID', index: 'ID', width: 60, align: 'left', key: true, editable: true, editoptions: { readonly: true, size: 10 }, formoptions: { elmprefix: "    ", elmsuffix: "" }, search: false },
        { name: 'Title', index: 'Title', width: 150, align: 'left', editable: true, editoptions: { size: 10 }, editrules: { edithidden: true, required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
        { name: 'IsSystem', index: 'IsSystem', width: 60, align: 'center', search: false, editable: false, formatter: 'checkbox' },
        { name: 'IsViewable', index: 'IsViewable', width: 60, align: 'center', search: false, editable: true, formatter: 'checkbox', edittype: 'checkbox', editoptions: { value: "True:False" }, formoptions: { elmprefix: "   ", elmsuffix: "" } }
      ],
      loadtext: 'Loading ...',
      rowNum: 13,
      rowList: [13, 26, 39],
      pager: '#tagPager',
      sortname: 'Title',
      viewrecords: true,
      sortorder: "asc",
      shrinkToFit: false,
      caption: 'Tags',
      editurl: "/General/UpdateTag/"
    });
    $("#tagList").jqGrid('navGrid', '#tagPager', { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { grid5[0].clearToolbar(); } },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterAdd: true, bSubmit: "Save" },
      { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete" }, {}, {});
    $("#tagList").jqGrid('filterToolbar');

  });
</script>
</asp:Content>
