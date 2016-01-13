function InitEventForm(data) {  
  $("#form_event_CategoriesMap li").each(function() {
    jQuery.tree.plugins.checkbox.uncheck(this);
  });  
  if (data == null) {
    $("#form_event_Event_ID").attr("value", '0');
    $("#form_event_Ordinary").attr("value", '1');
    $("#form_event_BFee").attr("value", '19.50');
    $("#form_event_IsCurrent").attr("value", 'false');
    $("#form_event_Title").attr("value", '');
    $("#form_event_Ordinary").attr("value", 1);
    $("#form_event_StartDate").datepicker('setDate', 0);
    $('#form_event_EndDate').datepicker('setDate', 0);
    $("#form_event_Description").attr("value", '');
    $("#form_event_CloseStep").attr("value", '0'),
    $("#form_event_type").attr("value", 1),
    $("#form_event_IsViewable").removeAttr("checked");
    $("#form_event_IsClickable").removeAttr("checked");    
    if (default_event_categories != null && default_event_categories.length > 0) {
      for (var i = 0; i < default_event_categories.length; i++) {
        jQuery.tree.plugins.checkbox.check($("#form_event_CategoriesMap li[id='" + default_event_categories[i] + "']"));
      }
    }
  } else {
    $("#form_event_Event_ID").attr("value", data.id);
    $("#form_event_Title").attr("value", data.t);
    var queryDate = data.s_1;
    dateParts = queryDate.match(/(\d+)/g);
    $("#form_event_StartDate").datepicker('setDate', new Date(dateParts[0], dateParts[1] - 1, dateParts[2]));
    $("#form_event_StartDate_1").attr("value", data.s_1);
    $("#form_event_StartDate_2").val(data.s_2);
    queryDate = data.e_1;
    dateParts = queryDate.match(/(\d+)/g);
    $('#form_event_EndDate').datepicker('setDate', new Date(dateParts[0], dateParts[1] - 1, dateParts[2]));
    $("#form_event_EndDate_1").attr("value", data.e_1);
    $("#form_event_EndDate_2").val(data.e_2);
    $("#form_event_BFee").attr("value", data.bf);
    $("#form_event_Description").attr("value", data.d);
    $("#form_event_IsViewable").attr("checked", data.iv);
    $("#form_event_IsClickable").attr("checked", data.icl);
    $("#form_event_Ordinary").attr("value", data.o);
    $("#form_event_IsCurrent").attr("value", data.ic);
    $("#form_event_CloseStep").attr("value", data.clst);
    $("#form_event_type").attr("value", data.et);
    if (data.c.length > 0) {
      var res = data.c.split(',');      
      for (i = 0; i < res.length; i++) {        
        $("#form_event_CategoriesMap li[id='" + res[i] + "']").each(function() {          
          jQuery.tree.plugins.checkbox.check(this);
        });
      }
    }
    $("#c_list").jqGrid('setPostData', { event_id: data.id });
    $("#c_list").trigger('reloadGrid');
  }
  $('#form_event').dialog('open');
}
function SubmitEventForm() {
  var checked_ids = [];
  $.tree.plugins.checkbox.get_checked($.tree.reference("#form_event_CategoriesMap")).each(function() {    
    if (this.id.indexOf("mc", 0) == -1) {
      checked_ids.push(this.id);
    }    
  });  
  return {
    ID: $("#form_event_Event_ID").attr("value"),
    Title: $("#form_event_Title").attr("value"),
    DateStart: $("#form_event_StartDate_1").attr("value") + ' ' + $("#form_event_StartDate_2").val(),
    DateEnd: $("#form_event_EndDate_1").attr("value") + ' ' + $("#form_event_EndDate_2").val(),
    BuyerFee: $("#form_event_BFee").attr("value"),
    Description: $("#form_event_Description").attr("value"),
    IsViewable: $("#form_event_IsViewable").attr("checked"),
    IsClickable: $("#form_event_IsClickable").attr("checked"),
    Ordinary: $("#form_event_Ordinary").attr("value"),
    IsCurrent: $("#form_event_IsCurrent").attr("value"),
    CloseStep: $("#form_event_CloseStep").attr("value"),    
    Categories: checked_ids.join(","),
    Type_ID: $("#form_event_type").attr("value"),
  };
}
function TransferModelToEventForm(data) {
  var msg = data.Message;
  var det = '';
  if (data.Details != null) {
    det += '<ul>';
    var exists = false;
    $.each(data.Details, function(i, item) {
      exists = true;
      switch (item.field) {
        case "Title": $("#form_event_Title").addClass("input-validation-error"); break;
        case "DateStart": $("#form_event_StartDate_1").addClass("input-validation-error"); $("#form_event_StartDate_2").addClass("input-validation-error"); break;
        case "DateEnd": $("#form_event_EndDate_1").addClass("input-validation-error"); $("#form_event_EndDate_2").addClass("input-validation-error"); break;
        case "BuyerFee": $("#form_event_BFee").addClass("input-validation-error"); break;
        case "EnteringFee": $("#form_event_EFee").addClass("input-validation-error"); break;
        case "Categories": $("#form_event_CategoriesMap").addClass("input-validation-error"); break;
        default: exists = false; break;
      }
      det += (exists) ? ('<li>' + item.message + '</li>') : '';
    });
    det += '</ul>';
  }
  MessageBox("Saving event", msg, det, "error");
}
function UpdateEvent() {
  $("*", "#form_event").removeClass("input-validation-error");
  LoadingFormOpen();
  $.post('/Event/UpdateEvent', { evnt: Sys.Serialization.JavaScriptSerializer.serialize(SubmitEventForm()) }, function(data) {
    LoadingFormClose();
    switch (data.Status) {
      case "ERROR":
        TransferModelToEventForm(data);
        break;
      case "SUCCESS":
        MessageBox('Saving event', 'Event was saved successfuly.', '', 'info');
        $("#form_event").dialog('close');
        break;
    }
  }, 'json');
}
var default_event_categories = new Array();
function InitDefaultEventCategories() {
  $.post('/Event/GetEventDefaultCategories', {}, function(data) {
    switch (data.Status) {
      case "ERROR":
        MessageBox("Init default event categories", data.Message, '', "error");
        break;
      case "SUCCESS":
        default_event_categories = data.Details;
        break;
    }
  }, 'json');
}
//------------------------------------------------------------------------------
$(document).ready(function () {
  $("#form_event").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 615,
    width: 540,
    modal: true,
    gridview: true,
    position: ['center', 'top'],
    beforeclose: function (event, ui) {
    },
    buttons: {
      'Cancel': function () { $(this).dialog('close'); },
      'Save': function () { UpdateEvent(); }
    },
    close: function () { $("*", "#form_event").removeClass("input-validation-error"); }
  });
  $("#form_event_tabs").tabs();

  $('#form_event_StartDate').datepicker({ altFormat: 'yy-mm-dd', altField: '#form_event_StartDate_1', dateFormat: 'yy-mm-dd' });
  $('#form_event_EndDate').datepicker({ altFormat: 'yy-mm-dd', altField: '#form_event_EndDate_1', dateFormat: 'yy-mm-dd' });

  $("#form_event_CloseStep").attr("disabled", "true");

  $("#form_event_CategoriesMap").tree({
    data: {
      type: "json",
      async: true,
      opts: {
        method: "POST",
        url: "/Category/GetCategoriesMapTreeJson"
      }
    },
    ui: {
      theme_name: "checkbox"
    },
    plugins: {
      checkbox: { three_state: true }
    },
    callback: {
      onselect: function (node, tree_obj) {//example: $(node).attr("id");        
      }
    }
  });

  $("#c_list").jqGrid({
    url: '/Event/GetInActiveCategories/',
    mtype: 'POST',
    datatype: "json",
    height: 400,
    colNames: ['#', 'Category'],
    colModel: [
      { name: 'EvCat_ID', index: 'EvCat_ID', width: 70, key: true, search: false },
      { name: 'Category', index: 'Category', width: 380, search: false, sortable: false }
    ],
    loadtext: 'Loading ...',
    shrinkToFit: true,
    postData: { event_id: null }
  });

  InitDefaultEventCategories();

});