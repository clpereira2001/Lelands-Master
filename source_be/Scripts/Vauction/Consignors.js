var consignor_rowID = null;
$(document).ready(function() {
  var grid_c = $("#c_list").jqGrid({
    mtype: 'GET',
    url: '/Auction/GetConsignorsList/',
    datatype: "json",
    height: '230',
    colNames: ['Cons.#', 'Seller#', 'Login', 'First and Last Name', 'Email', 'Event', 'Created Date', 'Specialist'],
    colModel: [
      { name: 'Consignment_ID', index: 'Consignment_ID', width: 80, key: true },
      { name: 'User_ID', index: 'User_ID', width: 70 },
      { name: 'Login', index: 'Login', width: 130 },
      { name: 'FLName', index: 'FLName', width: 160 },
      { name: 'Email', index: 'Email', width: 160 },
      { name: 'Event', index: 'Event', width: 200, stype: 'select', editoptions: { value: consignment_table_events} },
      { name: 'ConsDate', index: 'ConsDate', width: 160, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
      { name: 'Specialist', index: 'Specialist', width: 160 }
    ],
    loadtext: 'Loading ...',
    rowNum: 10,
    rowList: [10, 20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#c_pager',
    viewrecords: true,
    shrinkToFit: true,
    sortname: 'ConsDate',
    sortorder: "desc",
    postData: { isfirstload: true },
    onSelectRow: function(id) {
      consignor_rowID = id;
      $("#a_list").jqGrid('setPostData', { cons_id: id });
      $("#a_list").trigger('reloadGrid');
    },
    ondblClickRow: function(id) { OpenConsignorFormByID(id); },
    caption: 'Consignor Statements'
  });
  $("#c_list").jqGrid('navGrid', '#c_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function() { grid_c[0].clearToolbar(); } }).navSeparatorAdd('#c_pager');

  //btnAdd
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "Add", buttonicon: 'ui-icon-plus', title: 'Add new consignor information', onClickButton: function() { InitConsignorForm(null); } });
  //btnEdit
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "Edit", buttonicon: 'ui-icon-pencil', title: 'Edit consignor information', onClickButton: function() { OpenConsignorFormByID(consignor_rowID); } }).navSeparatorAdd('#c_pager');
  //btnDel
  $("#c_list").jqGrid('navButtonAdd', '#c_pager', { caption: "Delete", buttonicon: 'ui-icon-trash', title: 'Delete consignor information', onClickButton: function() { DeleteConsignorStatementByID(consignor_rowID); } }).navSeparatorAdd('#c_pager');

  $("#c_list").jqGrid('filterToolbar');

  $("#a_list").jqGrid({
    mtype: 'GET',
    url: '/Auction/GetAuctionsListByConsignor/',
    datatype: "json",
    height: '230',
    colNames: ['Auction#', 'Lot', 'Title', 'Description', 'Category', 'Reserve', 'Quantity', 'Commission Rate', 'LOA', 'Copy Notes', 'Photo Notes', 'Layout', 'Old#', 'Entered By', 'Writing Step'],
    colModel: [
       { name: 'Auction_ID', index: 'Auction_ID', width: 60, key: true, sortable:false },
       { name: 'Lot', index: 'Lot', width: 30, sortable: false },       
       { name: 'Title', index: 'Title', width: 200, sortable: false },
       { hidden:true, name: 'Description', index: 'Description', width: 110, sortable: false },
       { name: 'Category', index: 'Category', width: 120, sortable: false },
       { name: 'Reserve', index: 'Reserve', width: 80, sortable: false },
       { name: 'Quantity', index: 'Quantity', width: 20, sortable: false, align: 'center', hidden:true },
       { name: 'Commission', index: 'Commission', width: 120, sortable: false },
       { name: 'LOA', index: 'LOA', width: 30, align: 'center', formatter: 'checkbox', sortable: false },
       { name: 'CopyNotes', index: 'CopyNotes', width: 110, sortable: false },
       { name: 'PhotoNotes', index: 'PhotoNotes', width: 110, sortable: false },
       { name: 'Layout', index: 'Layout', width: 80, sortable: false },
       { name: 'OldAuction_ID', index: 'OldAuction_ID', width: 50, sortable: false },
       { name: 'Specialist', index: 'Specialist', width: 60, sortable: false },
       { name: 'ListingStepDescription', index: 'ListingStepDescription', width: 100, sortable: false }
    ],
    loadtext: 'Loading ...',
    rowNum: 500,
    rowList: [10, 20, 25, 30, 35, 40, 60, 80, 100, 200, 300, 400, 500],
    pager: '#a_pager',
    sortname: 'Title',
    sortorder: 'asc',
    viewrecords: true,
    shrinkToFit: true,    
    postData: { cons_id: null },
    caption: 'Inventory'
  });
  $("#a_list").jqGrid('navGrid', '#a_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function() { } });

  $("#c_list").jqGrid('setPostData', { isfirstload: false });
});
//--[Functions]------------------------------------------------------------------------------------
function OpenConsignorFormByID(id) {
  if (id == null) { MessageBox('Edit consignor statement', 'Please select row.', '', 'info'); return; }
  LoadingFormOpen();
  $.post('/Auction/GetConsignorStatementJSON', { cons_id: id }, function(data) { InitConsignorForm(data); LoadingFormClose(); }, 'json');
}

function DeleteConsignorStatementByID(id) {
  if (id == null) { MessageBox('Delete consignor statement', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Delete consignor statement', 'Do you really want to delete this consignor statement?', function () {
    LoadingFormOpen();
    $.post('/Auction/DeleteConsignorStatement', { cons_id: id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Deleting consignor statement", data.Message, '', "error");
          break;
        case "SUCCESS":          
          $("#с_list").jqGrid('delRowData', parseInt(data.Details));
          MessageBox("Deleting consignor statement", "Consignor statement was deleted successfully.", '', "info");
          break;
      }
    }, 'json');
  });
}

function EditItemInConsGridByID() {
  var id = $("#a_list").jqGrid('getGridParam', 'selrow');
  if (id == null) { MessageBox('Edit item', 'Please select item.', '', 'info'); return; }
  var ret = $("#a_list").jqGrid('getRowData', id);
  OpenAuctionFormByID(ret.Auction_ID);
}