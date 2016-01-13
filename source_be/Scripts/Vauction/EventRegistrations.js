// =========================================================================================================================
// EventRegistrations.aspx
// =========================================================================================================================
function InitEventRegistrationForm(IsHB) {
  if (IsHB) {
    $("#tdUser").hide();
    $("#tdHB").show();
  }
  else {
    $("#tdUser").show();
    $("#tdHB").hide();
  }
  isHB = IsHB;
  $("#form_eventreg_User").attr("res", "").attr("value", "").attr("title", "");
  $('#form_eventreg').dialog('open');
}
var isHB = false;
//-------------------------------------------------------------------------------------
var events_rowID = null;
$(document).ready(function() {
  var eregs_w = document.body.offsetWidth - 100;
  $("#e_list").jqGrid({
    url: '/Event/GetEventRegistration/',
    mtype: 'POST',
    datatype: "json",
    height: '100%',
    width: eregs_w,
    colNames: ['Ev.Reg#', 'Event', 'User#', 'Login', 'Status', 'Type', 'Email', 'Registration Date'],
    colModel: [
      { name: 'EventRegistrationID', index: 'EventRegistrationID', width: 70, key: true, editable: false },
      { name: 'Event', index: 'Event', width: 250, stype: 'select', editoptions: { value: evreg_table_events} },
      { name: 'UserID', index: 'UserId', width: 70 },
      { name: 'Login', index: 'Login', width: 130 },
      { name: 'UserType', index: 'UserType', width: 100, stype: 'select', editoptions: { value: evreg_table_usertype} },
      { name: 'Status', index: 'Status', width: 100, search: false },
      { name: 'Email', index: 'Email', width: 200, search: false },
      { name: 'DateRegistered', index: 'DateRegistered', width: 200, search: false }
    ],
    loadtext: 'Loading ...',
    rowNum: 25,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#e_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'EventRegistrationID',
    sortorder: "desc",
    postData: { isfirstload: true },
    onSelectRow: function(id) { events_rowID = id; },
    ondblClickRow: function(id) { OpenEventRegFormByID(); }
  });
  
  $("#e_list").jqGrid('navGrid', '#e_pager', { edit: false, add: false, addtext: "Add", del: false, deltext: "Delete", search: false, refreshtext: "Refresh" }).navSeparatorAdd('#e_pager'); //, afterRefresh: function() { $("#e_list")[0].clearToolbar(); } 

  //btnAdd
  $("#e_list").jqGrid('navButtonAdd', '#e_pager', { caption: "Add", buttonicon: 'ui-icon-plus', title: 'Add event registration', onClickButton: function() { InitEventRegistrationForm(false); } }).navSeparatorAdd('#e_pager');
  //btnAddRandom
  $("#e_list").jqGrid('navButtonAdd', '#e_pager', { caption: "Add HB", buttonicon: 'ui-icon-plus', title: 'Add event registrations for some house bidders', onClickButton: function() { InitEventRegistrationForm(true); } }).navSeparatorAdd('#e_pager');
  //btnDel
  $("#e_list").jqGrid('navButtonAdd', '#e_pager', { caption: "Delete", buttonicon: 'ui-icon-trash', title: 'Delete event registration', onClickButton: function() { DeleteEventRegByID(events_rowID); } }).navSeparatorAdd('#e_pager');

  $("#e_list").jqGrid('filterToolbar');

  $("#e_list").jqGrid('setPostData', { isfirstload: false });

  $("#form_eventreg").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 230,
    width: 260,
    modal: true,
    gridview: true,
    position: ['center', 'top'],
    buttons: {
      'Cancel': function() { $(this).dialog('close'); },
      'Save': function() { UpdateEventRegistration(); }
    },
    close: function() { }
  });

  InitDropDown('/General/GetEventsJSON', '#form_consignor_Event');
  $("#form_eventreg_User").click(function() { $('#dBuyer').dialog('open'); });
});
//--[Functions]--------------------------------------------------------------------
function DeleteEventRegByID(id) {
  if (id == null) { MessageBox('Delete event registration', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Delete event registration', 'Do you really want to delete this event registration?', function() {
    LoadingFormOpen();
    $.post('/Event/DeleteEventRegistrations', { eventreg_id: id }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Deleting event registration", data.Message, '', "error");
          break;
        case "SUCCESS":
          $("#e_list").jqGrid('delRowData', id);
          MessageBox("Deleting event registration", "The event registration deleted successfully.", '', "info");
          $('#form_eventreg').dialog('close');
          break;
      }
    }, 'json');
  });
}
//---------------------------------------------------------------------------------
function UpdateEventRegistration() {
  var er = $("#form_eventreg_User").attr("res");
  if (!isHB && er == null) MessageBox("Adding event registration", "Please select the user", '', "error");
  if (isHB) {
    var hbN = $("#form_eventreg_HBNumber").attr("value");
    if (hbN == null || hbN == '') { MessageBox('Adding event registration', 'Please enter the number of house bidders.', '', 'info'); return; }
    ConfirmBox('Adding event registration', 'Do you really want to add registrations for some house bidders?', function() {
      LoadingFormOpen();
      $.post('/Event/UpdateEventRegistrations', { event_id: $("#form_consignor_Event").val(), user_id: hbN, ishb: true }, function(data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Adding event registration", data.Message, '', "error");
            break;
          case "SUCCESS":
            MessageBox("Adding event registration", "The house bidders were registered for the event successfully.", '', "info");
            $('#form_eventreg').dialog('close');
            break;
        }
      }, 'json');
    });
  }
  else {
    LoadingFormOpen();
    $.post('/Event/UpdateEventRegistrations', { event_id: $("#form_consignor_Event").val(), user_id: er, ishb: false }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Adding event registration", data.Message, '', "error");
          break;
        case "SUCCESS":
          MessageBox("Adding event registration", "The user was registered successfully.", '', "info");
          $('#form_eventreg').dialog('close');
          break;
      }
    }, 'json');
  }
}
