function ClearForm() {
  $('#copynotes, #description, #id').attr('value', '');
  $('#iswritten').removeAttr('checked');
  $('#sleft, #stotal').html('0').css('color', '#696969');
  $('#priority').attr('value', '0');
  $('#auction_id').attr('title', '').attr('value', '').attr('res', '');
  $('#iswritten').removeAttr('disabled');
  setformfieldsize($('#description'), 0, '');
}
function ResetForm() {
  var id = $('#id').attr('value');
  if (id != null && parseInt(id) > 0) {
    ConfirmBox('Updating description', 'You have an opened lot# '+id+'. All unsaved changes will be lost. Do you really want to reset all changes?', function () {
      ClearForm();
    });
  } else {
    ClearForm();
  }
}
function SetEvent(id) {
  $('#auction_id').attr('title', '').attr('value', '').attr('res', '');
  $("#dAuctions_list").jqGrid('setPostData', { event_id: id });
  $("#dAuctions_list").trigger('reloadGrid');
}

var sarr = [0, 7900, 750, 650, 650];
$(document).ready(function () {
  $("#event_id").change(function () {
    ClearForm();
    SetEvent($("#event_id").val());
  });
  InitDropDown('/General/GetPendingEventsJSON', '#event_id', function () { $("#event_id").change(); });
  $("#auction_id").click(function () { $('#dAuctions').dialog('open'); });
  $("#description").change(function () {
    if (parseInt($('#islimitdisabled').attr('value')) == 1) {
      $('#sleft').css('color', 'green');
    } else {
      $('#sleft').html($('#description').val().length);
      $('#sleft').css('color', parseInt($('#sleft').html()) <= parseInt($('#priority').attr("value")) ? 'green' : 'red');
    }
  });
  $("#description").keypress(function (event) {
    $("#description").change();
  });
  $("#description").bind('paste', function (e) {
    var el = $(this);
    setTimeout(function () {
      if (parseInt($('#islimitdisabled').attr('value')) == 1) return;
      $("#description").change();
      var text = $(el).val();
      var lim = parseInt($('#priority').attr("value")) + 100;
      if ($("#description").val() < 2 && (text.length >= lim - 5 && text.length <= lim)) {
        MessageBox("Updating description", "Your description for this item is close to Maximum Characters. Please make sure copy is complete.", '', "info");
      } else if (text.length > lim) {
        MessageBox("Updating description", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
      }
    }, 100);
  });

  $('#btnCancel').click(function () {
    ResetForm();
  });
  $('#btnUpdate').click(function () {
    var desc = $('#description').val().length;
    var lim = parseInt($('#priority').attr("value")) + 100;
    if (desc > lim && parseInt($('#islimitdisabled').attr('value')) == 0) {
      MessageBox("Updating description", "Warning! You are over your character limit for this item. Please revise your copy", '', "error");
      return;
    }
    ConfirmBox('Updating description', 'Do you want to update the copy notes and description?', function () {
      var desc = $('#description').val().length;
      var lim = parseInt($('#priority').attr("value")) + 100;
      if (desc >= lim - 5 && desc <= lim) {
        MessageBox("Updating description", "Your description for this item is close to Maximum Characters. Please make sure copy is complete.", '', "info");
      }
      LoadingFormOpen();
      $.post('/Auction/UpdateDescription', { auction_id: $('#id').attr('value'), descr: $('#description').attr('value'), cnotes: $('#copynotes').attr('value'), iswritten: $('#iswritten').attr('checked') }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Updating description", data.Message, '', "error");
            break;
          case "SUCCESS":
            $("#event_id").change();
            MessageBox("Updating description", "Description and copy notes were saved successfully.", '', "info");
            break;
        }
      }, 'json');
    });
  });
});