var users_rowID = null;
$(document).ready(function () {
  var users_w = document.body.offsetWidth - 100;

  $("#u_list").jqGrid({
    url: '/User/GetUsersList/',
    method: 'GET',
    datatype: "json",
    height: '100%',
    width: users_w,
    colNames: ['User#', 'Login', 'Password', 'Name', 'Email Approved', 'Status', 'Type', 'Email', 'Registration Date', 'Commission', 'Phone (1)', 'Phone (2)', 'Billing Address', 'Shipping Address'],
    colModel: [
          { name: 'Id', index: 'User_Id', width: 70, key: true },
 		      { name: 'Login', index: 'Login', width: 130 },
 		      { name: 'Password', index: 'Password', width: 130, search: false, sortable: false },
 		      { name: 'Name', index: 'Name', width: 160 },
 		      { name: 'IsConfirmed', index: 'IsConfirmed', width: 50, align: 'center', search: false, formatter: 'checkbox' },
 		      { name: 'Status', index: 'Status', width: 80, stype: 'select', editoptions: { value: users_table_userstatus} },
 		      { name: 'UserType', index: 'UserType', width: 80, stype: 'select', editoptions: { value: users_table_usertype} },
 		      { name: 'Email', index: 'Email', width: 160 },
 		      { name: 'DateIN', index: 'DateIN', width: 160, searchoptions: { dataInit: function (el) { $(el).datepicker({ dateFormat: 'mm/dd/yy' }); } } },
 		      { name: 'CommRate_ID', index: 'CommRate_ID', width: 140, stype: 'select', editoptions: { value: users_table_commrates }, sortable: false },
 		      { name: 'DayPhone', index: 'DayPhone', width: 150, sortable: false },
 		      { name: 'EveningPhone', index: 'EveningPhone', width: 150, sortable: false },
 		      { name: 'BillingAddress', index: 'BillingAddress', width: 300, search: false, sortable: false },
 		      { name: 'ShippingAddress', index: 'ShippingAddress', width: 300, search: false, sortable: false }
 	      ],
    loadtext: 'Loading ...',
    rowNum: 25,
    rowList: [20, 25, 30, 35, 40, 60, 80, 100],
    pager: '#u_pager',
    viewrecords: true,
    shrinkToFit: false,
    sortname: 'DateIN',
    sortorder: "desc",
    onSelectRow: function (id) { users_rowID = id; },
    ondblClickRow: function (id) { OpenUserFormByID(id); },
    postData: { isfirstload: true }
  });
  $("#u_list").jqGrid('navGrid', '#u_pager', { edit: false, add: false, del: false, search: false, refreshtext: "Refresh", afterRefresh: function () { $("#u_list")[0].clearToolbar(); } }).navSeparatorAdd('#u_pager');
  //btnAdd
  $("#u_list").jqGrid('navButtonAdd', '#u_pager', { caption: "Add", buttonicon: 'ui-icon-plus', title: 'Add new user', onClickButton: function () { InitUserForm(-1); } });
  //btnEdit
  $("#u_list").jqGrid('navButtonAdd', '#u_pager', { caption: "Edit", buttonicon: 'ui-icon-pencil', title: 'Edit user data', onClickButton: function () { OpenUserFormByID(users_rowID); } }).navSeparatorAdd('#u_pager');
  //btnActivate
  $("#u_list").jqGrid('navButtonAdd', '#u_pager', { caption: "Activate", buttonicon: 'ui-icon-unlocked', title: 'Activate user', onClickButton: function () { AcivateUserByID(users_rowID); } }).navSeparatorAdd('#u_pager');
  //btnDel
  $("#u_list").jqGrid('navButtonAdd', '#u_pager', { caption: "Delete", buttonicon: 'ui-icon-trash', title: 'Delete user data', onClickButton: function () { DeleteUserByID(users_rowID); } }).navSeparatorAdd('#u_pager');

  $("#u_list").jqGrid('filterToolbar');

  $("#u_list").jqGrid('setPostData', { isfirstload: false });
});
//--[Functions]------------------------------------------------------------------------------------
function OpenUserFormByID(id) {
  if (id == null) { MessageBox('Edit user', 'Please select row.', '', 'info'); return; }
  $.post('/User/GetUserDataJSON', { user_id: id }, function(data) { LoadingFormOpen(); InitUserForm(data); LoadingFormClose(); }, 'json');
}

function AcivateUserByID(id) {
  if (id == null) { MessageBox('Activating user', 'Please select row.', '', 'info'); return; }
  LoadingFormOpen();
  $.post('/User/ActivateUser', { user_id: id }, function(data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        MessageBox("Activating user", data.Message, '', "error");
        break;
      case "SUCCESS":
        $("#u_list").jqGrid('setRowData', users_rowID, {Status : "Active" });
        MessageBox("Activating user", "The user status set to Active successfully", '', "info");
        break;
    }
  }, 'json');
}

function DeleteUserByID(id) {
  if (id == null) { MessageBox('Delete user', 'Please select row.', '', 'info'); return; }
  ConfirmBox('Delete user', 'Do you really want to delete this user?', function() {
    LoadingFormOpen();
    $.post('/User/DeleteUser', { user_id: id }, function(data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Deleting user", data.Message, '', "error");
          break;
        case "SUCCESS":
          $("#u_list").jqGrid('delRowData', users_rowID);
          //MessageBox("Deleting user", "User's state is set to INACTIVE", '', "info");
          break;
      }
    }, 'json');
 }); 
}
//--------------------------------------------------------------------------------------------------