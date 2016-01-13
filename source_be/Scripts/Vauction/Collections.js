$(document).ready(function() {
    var tableWidth = document.body.offsetWidth - 100;
    $("#collections_list").jqGrid({
        mtype: 'POST',
        url: '/Auction/GetCollections/',
        datatype: "json",
        height: '100%',
        width: tableWidth,
        colNames: ['#', 'Title', 'Web Title', 'Description'],
        colModel: [
            { name: 'ID', index: 'ID', width: 50, key: true, search: false, sortable: false },
            { name: 'Title', index: 'Title', width: 300, search: false, sortable: false, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
            { name: 'WebTitle', index: 'WebTitle', width: 300, search: false, sortable: false, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } },
            { name: 'Description', index: 'Description', width: 600, search: false, sortable: false, editable: true, editoptions: { "rows": 20 }, edittype: "textarea", editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: "" } }
        ],
        loadtext: 'Loading ...',
        rowNum: 10,
        rowList: [10, 15, 20, 25, 30, 35, 40, 60, 80, 100],
        pager: '#collections_pager',
        viewrecords: true,
        shrinkToFit: false,
        sortname: 'IsEnabled',
        sortorder: 'desc',
        multiselect: true
    });
    $("#collections_list").jqGrid('navGrid', '#collections_pager', { edit: true, edittext: "Edit", add: true, addtext: "Add", del: true, deltext: "Delete", search: false, refreshtext: "Refresh" },
        { jqModal: false, closeOnEscape: true, bSubmit: "Save", bottominfo: "Fields marked with (*) are required", editCaption: "Edit", closeAfterEdit: true, afterSubmit: AfterSubmitFunction, url: "/Auction/UpdateCollection/" }, { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Add", bottominfo: "Fields marked with (*) are required", afterSubmit: AfterSubmitFunction, url: "/Auction/AddCollection/" },
        { jqModal: false, closeOnEscape: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Auction/DeleteCollection/" }, {}, {}).navSeparatorAdd('#collections_pager');
});