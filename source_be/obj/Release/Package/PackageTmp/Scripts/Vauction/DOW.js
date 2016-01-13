var dow_rowID = null;
$(document).ready(function () {
  var items_table_w = document.body.offsetWidth - 100;
  $("#d_list").jqGrid({
    mtype: 'POST',
    url: '/Auction/GetDOWList/',
    datatype: "json",
    height: '100%',
    width: items_table_w,
    colNames: ['Auction#', 'Status', 'Category', 'Title', 'Price', 'Shipping', 'Consignor', 'Comm.Rate'],
    colModel: [
       { name: 'Auction_ID', index: 'Auction_ID', width: 60, key: true },
       { name: 'Status_ID', index: 'Status_ID', width: 60, align: 'left', stype: 'select', editoptions: { value: auction_table_statuses} },
       { name: 'Category', index: 'Category', width: 220 },
       { name: 'Title', index: 'Title', width: 300 },
       { name: 'Price', index: 'Price', width: 80, align: 'right' },       
       { name: 'Shipping', index: 'Shipping', width: 60, align: 'right' },
       { name: 'Consignor', index: 'Consignor', width: 100 },
       { name: 'CommRate', index: 'CommRate', width: 120, stype: 'select', editoptions: { value: auction_table_commissionrate} }
    ],
    loadtext: 'Loading ...',
    rowNum: 20,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#d_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'Auction_ID',
    sortorder: 'desc',
    onSelectRow: function (id) { dow_rowID = id; },
    ondblClickRow: function (id) { OpenDOWFormByID(id); },
    postData: { isfirstload: true }
//    subGrid: true,
//    subGridUrl: '/Invoice/GetPaymentByAuction/',
//    subGridModel: [
//      { name: ['Payment#', 'User', 'Method', 'Amount', 'Paid Date', 'Note', 'CC Number', 'Auth. Code'],
//        width: [80, 200, 110, 100, 80, 150, 100, 100],
//        params: ['Auction_ID']
//      }]
  });
    $("#d_list").jqGrid('navGrid', '#d_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function () { $("#d_list")[0].clearToolbar(); } }).navSeparatorAdd('#d_pager');
  //btnAdd
  $("#d_list").jqGrid('navButtonAdd', '#d_pager', { caption: "Add", buttonicon: 'ui-icon-plus', title: 'Add new DOW', onClickButton: function () { InitDOWForm(null); } }).navSeparatorAdd('#d_pager');
  //btnEdit
  $("#d_list").jqGrid('navButtonAdd', '#d_pager', { caption: "Edit", buttonicon: 'ui-icon-pencil', title: 'Edit DOW', onClickButton: function () { OpenDOWFormByID(dow_rowID); } }).navSeparatorAdd('#d_pager');
  //btnDel
  $("#d_list").jqGrid('navButtonAdd', '#d_pager', { caption: "Delete", buttonicon: 'ui-icon-trash', title: 'Delete DOW', onClickButton: function () { DeleteDOWByID(dow_rowID); } }).navSeparatorAdd('#d_pager');
  //btnBids
//  $("#d_list").jqGrid('navButtonAdd', '#d_pager', { caption: "Bids", buttonicon: 'ui-icon-bookmark', title: 'Bids preview', onClickButton: function () { ShowBids(dow_rowID); } }).navSeparatorAdd('#d_pager');
  //btnInvoices
//  $("#d_list").jqGrid('navButtonAdd', '#d_pager', { caption: "Invoices", buttonicon: 'ui-icon-clipboard', title: 'Invoices', onClickButton: function () { InitDOWInvoiceForm(dow_rowID); } }).navSeparatorAdd('#d_pager');

  $("#d_list").jqGrid('filterToolbar');

  $("#d_list").jqGrid('setPostData', { isfirstload: false });
});
//--[Functions]------------------------------------------------------------------------------------

//function DeleteDOWByID(id) {
//  if (id == null) { MessageBox('Deleting DOW', 'Please select row.', '', 'info'); return; }
//  ConfirmBox('Deleting the DOW', 'Do you really want to delete selected Deal of the Week?', function () {
//    LoadingFormOpen();
//    $.post('/Auction/DeleteAuction', { auction_id: id }, function (data) {
//      LoadingFormClose();
//      switch (data.Status) {
//        case "ERROR":
//          MessageBox("Deleting auction", data.Message, '', "error");
//          break;
//        case "SUCCESS":
//          $("#d_list").jqGrid('delRowData', id);
//          MessageBox("Deleting DOW", "The deal was deleted successfully.", '', "info");
//          break;
//      }
//    }, 'json');
//  });
//}

//function ShowBids(id) {
//  if (id == null) { MessageBox('DOW bids', 'Please select the row', '', 'info'); return; }
//  LoadingFormOpen();
//  InitDOWBidsForm(id);
//  LoadingFormClose();
//}