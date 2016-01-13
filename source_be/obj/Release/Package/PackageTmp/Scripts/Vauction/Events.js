// =========================================================================================================================
// Events.aspx
// =========================================================================================================================
var events_rowID = null;
$(document).ready(function() {
  var events_w = document.body.offsetWidth - 100;
  var grid_e = $("#e_list").jqGrid({
    url: '/Event/GetEventsList/',
    mtype: 'POST',
    datatype: "json",
    height: '100%',
    width: events_w,
    colNames: ['Event#', 'Ordinary', 'Status', 'Title', 'Start Date', 'End Date', 'Viewable', 'Clickable', 'Step', 'Buyer Fee', 'Current'],
    colModel: [
          { name: 'Id', index: 'Event_Id', width: 70, key: true },
 		      { name: 'Ordinary', index: 'Ordinary', width: 60, search: false, align:'center' },
 		      { name: 'Status', index: 'Status', width: 60, align: 'center', search: false, sortable: false },
 		      { name: 'Title', index: 'Title', width: 340 },
 		      { name: 'DateStart', index: 'DateStart', width: 160, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
 		      { name: 'DateEnd', index: 'DateEnd', width: 160, searchoptions: { dataInit: function(el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
 		      { name: 'IsViewable', index: 'IsViewable', width: 60, align: 'center', search: false, formatter: 'checkbox' },
 		      { name: 'IsClickable', index: 'IsClickable', width: 60, align: 'center', search: false, formatter: 'checkbox' },
 		      { name: 'CloseStep', index: 'CloseStep', width: 40, search: false, align: 'center' },
 		      { name: 'BuyerFee', index: 'BuyerFee', width: 60 }, 		      
 		      { hidden: true, name: 'IsCurrent', index: 'IsCurrent', width: 1 }
 	      ],
    loadtext: 'Loading ...',
    rowNum: 20,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#e_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'DateStart',
    sortorder: "desc",
    onSelectRow: function(id) { events_rowID = id; },
    ondblClickRow: function(id) { OpenEventFormByID(id); },
    afterInsertRow: function(rowid, aData) {
      if (aData.IsCurrent == 'True')
        grid_e.jqGrid('setCell', rowid, 'Status', '', { color: 'green', 'font-weight': 'bold' });
      else if (aData.Status == 'pending')
        grid_e.jqGrid('setCell', rowid, 'Status', '', { color: '#1382CC', 'font-weight': 'normal' });
      else
        grid_e.jqGrid('setCell', rowid, 'Status', '', { color: 'gray', 'font-weight': 'normal' });
    }
  });

  grid_e.jqGrid('navGrid', '#e_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function() { grid_e[0].clearToolbar(); } }).navSeparatorAdd('#e_pager');
  //btnAdd
  grid_e.jqGrid('navButtonAdd', '#e_pager', { caption: "Add", buttonicon: 'ui-icon-plus', title: 'Add event', onClickButton: function() { InitEventForm(null); } });
  //btnEdit
  grid_e.jqGrid('navButtonAdd', '#e_pager', { caption: "Edit", buttonicon: 'ui-icon-pencil', title: 'Edit event', onClickButton: function() { OpenEventFormByID(events_rowID); } }).navSeparatorAdd('#e_pager');
  //btnDel
  grid_e.jqGrid('navButtonAdd', '#e_pager', { caption: "Delete", buttonicon: 'ui-icon-trash', title: 'Delete event', onClickButton: function() { DeleteEventByID(events_rowID); } }).navSeparatorAdd('#e_pager');

  grid_e.jqGrid('filterToolbar');
});
//--[Functions]------------------------------------------------------------------------------------
function OpenEventFormByID(id) {
  if (id == null) { MessageBox('Edit event', 'Please select row.', '', 'info'); return; }
  LoadingFormOpen();  
  $.post('/Event/GetEventDataJSON', { event_id: id }, function(data) { InitEventForm(data); LoadingFormClose(); }, 'json');
}

function DeleteEventByID(id) {
  if (id == null) { MessageBox('Delete event', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Delete event', 'Do you really want to delete this event?', function() {
    LoadingFormOpen();
    $.post('/Event/DeleteEvent', { event_id: id }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Deleting event", data.Message, '', "error");
          break;
        case "SUCCESS":
          $("#e_list").jqGrid('delRowData', events_rowID);
          break;
      }
    }, 'json');
  });
}
//--------------------------------------------------------------------------------------------------