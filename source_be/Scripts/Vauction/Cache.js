var cache_rowID = null;
$(document).ready(function () {
  var items_table_w = document.body.offsetWidth - 100;
  $("#c_list").jqGrid({
    mtype: 'POST',
    url: '/Cache/GetCacheList/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['id', '#', 'Type', 'Region', 'Method', 'Data', 'TTL'],
    colModel: [
       { name: 'ID', index: 'ID', width: 1, key: true, hidden:true },
       { name: 'RowNumber', index: 'RowNumber', width: 60, hidden:true},
       { name: 'Type', index: 'Type', width: 150, stype: 'select', editoptions: { value: ':;REFERENCE:REFERENCE;RESOURCE:RESOURCE;ACTIVITY:ACTIVITY'} },
       { name: 'Region', index: 'Region', width: 150 },
       { name: 'Method', index: 'Method', width: 280 },
       { name: 'Data', index: 'Data', width: 400 },
       { name: 'TTL', index: 'TTL', width: 100 }
    ],
    rowNum: 20,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100, 200, 500, 1000, 10000, 100000],
    pager: '#c_pager',
    viewrecords: true,
    shrinkToFit: false,
    onSelectRow: function (id) { cache_rowID = id; },
    postData: { _firstload: true }
  });
  $("#c_list").jqGrid('navGrid', '#c_pager', { edit: false, add: false, del: true, deltext:"Delete", search: false, refreshtext: "Refresh", afterRefresh: function () { $("#c_list")[0].clearToolbar(); } },
    {},{},
    {jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Cache/DeleteCache/" }, //delete
    {}
  ).navSeparatorAdd('#c_pager');

  //btnCreateRegions
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "Init regions", buttonicon: 'ui-icon-wrench', title: 'Init regions', onClickButton: function () { InitRegions(); } }).navSeparatorAdd('#c_pager');

  //btnClearRegion
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "REF.", buttonicon: 'ui-icon-minus', title: 'Clear REFERENCE', onClickButton: function () { ClearRegion("REFERENCE"); } }).navSeparatorAdd('#c_pager');
  //btnClearRegion
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "RES.", buttonicon: 'ui-icon-minus', title: 'Clear RESOURCE', onClickButton: function () { ClearRegion("RESOURCE"); } }).navSeparatorAdd('#c_pager');
  //btnClearRegion
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "ACT.", buttonicon: 'ui-icon-minus', title: 'Clear ACTIVITY', onClickButton: function () { ClearRegion("ACTIVITY"); } }).navSeparatorAdd('#c_pager');
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "All", buttonicon: 'ui-icon-minus', title: 'Clear All Cache', onClickButton: function () { ClearRegion(""); } }).navSeparatorAdd('#c_pager');

  $("#c_list").jqGrid('filterToolbar');

  $("#c_list").jqGrid('setPostData', { _firstload: false });
});

function InitRegions() {
  $.post('/Cache/InitRegions', {}, function (data) {
    switch (data.Status) {
      case "ERROR":
        MessageBox("Init cache regions", data.Message, '', "error");
        break;
      case "SUCCESS":
        MessageBox("Init cache regions", "The region initialization was finished successfully", "", "info");
        break;
    }
  }, 'json');
}

function ClearRegion(key) {
  $.post('/Cache/ClearRegion', { type : key}, function (data) {
    switch (data.Status) {
      case "ERROR":
        MessageBox("Clear cache type", data.Message, '', "error");
        break;
      case "SUCCESS":
        MessageBox("Clear cache type", "The type was cleared successfully", "", "info");
        break;
    }
  }, 'json');
}