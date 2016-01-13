/*--[DIFFERENT]--------------------------------------------------------------------------------------------*/
// DateToString
function DateToString(datetime) {
  var minutes = datetime.getMinutes()
  if (minutes < 10) minutes = "0" + minutes
  var pmam = (datetime.getHours() > 11) ? "PM" : "AM";
  return (datetime.getMonth() + 1) + '/' + datetime.getDate() + '/' + datetime.getFullYear() + ' ' + datetime.getHours() + ':' + minutes + ' '+pmam;
}

/*--[DROPDOWN]--------------------------------------------------------------------------------------------*/
// InitDropDown
function InitDropDown(method, control, success_function) {
  $.ajax({
    url: method,
    dataType: "json",
    success: function(data, textStatus) {
      if (textStatus != "success") return;
      $(control + " option").remove();
      $.each(data.rows, function(i, item) {
        $(control).append('<option value="' + item.val + '">' + item.txt + '</option>');
      });
      if (success_function != null) success_function();
    }
  });
}
// InitDropDownWithGroup
function InitDropDownWithGroup(method, control) {
  $.ajax({
    url: method,
    dataType: "json",
    success: function(data, textStatus) {
      if (textStatus != "success") return;
      var gr = "";
      $(control + " option").remove();
      $.each(data.rows, function(i, item) {
        if (gr != item.gr) {
          if (gr != "") $(control).append('</optgroup>');
          gr = item.gr;
          $(control).append('<optgroup label="' + gr + '">');
        }
        $(control).append('<option value="' + item.val + '">' + item.txt + '</option>');
      });
      if ($(control).length > 0) $(control).append('</optgroup>');      
    }
  });
}

// InitDropDown
function InitDropDownPost(method, control, success_function) {
  $.ajax({
    url: method,
    type: 'POST',    
    dataType: "json",
    success: function (data, textStatus) {
      if (textStatus != "success") return;
      $(control + " option").remove();
      $.each(data.rows, function (i, item) {
        $(control).append('<option value="' + item.val + '">' + item.txt + '</option>');
      });
      if (success_function != null) success_function();
    }
  });
}

// InitDropDownByCopingContent
//function InitDropDownByCopingContent(controlSource, controlDestination) {
//  $(controlDestination + ' option').remove();
//  $(controlSource).clone().appendTo(controlDestination);
//}

/*========================================================================================================
---------[MessageBox]-------------------------------------------------------------------------------------
==========================================================================================================*/
function MessageBox(title, message, details, type) {
  var msgbox = '<div id="dialog_messagebox" title="'+title+'" style="magrin:0">'
  switch(type)
  {
    case "error":
      msgbox += '<div class="ui-state-error ui-corner-all" style="padding: 0 .7em;">' +
					'<p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em; margin-top:3px"></span>' +
					'<strong>Error:</strong> '+message+'</p>'+details+'</div>';
      break;
    case "info":
      msgbox += '<div class="ui-state-highlight ui-corner-all" style="padding: 0 .7em;">' +
					'<p><span class="ui-icon ui-icon-info" style="float: left; margin-right: .3em;margin-top:3px"></span>' +
					'<strong>Info:</strong> '+message+'</p>'+details+'</div>';
      break;
  }
  msgbox += '</div>';
  $(msgbox).appendTo("body");
  $("#dialog_messagebox").dialog({
    bgiframe: true,
    modal: true,
    width: 400,
    minHeight: 50,
    close: function() { $("#dialog_messagebox").remove(); }    
  });
  $('#dialog_messagebox').dialog('open');
}
//------------------------------------------------------------------------------------------------------------------
function ConfirmBox(title, question, function_yes, function_no) {
  var msgbox = '<div id="dialog_confirmbox" title="' + title + '" style="magrin:0">'+
    '<div class="ui-state-highlight ui-corner-all" style="padding: 0 .7em;">' +
		'<p><span class="ui-icon ui-icon-help" style="float: left; margin-right: .3em;margin-top:3px"></span>' +
		'<strong>Question:</strong> ' + question + '</p></div></div>';
  $(msgbox).appendTo("body");
  $("#dialog_confirmbox").dialog({
    bgiframe: true,
    modal: true,
    width: 400,
    minHeight: 50,
    overlay: {
      backgroundColor: '#000',
      opacity: 0.5
    },
    close: function() { $("#dialog_confirmbox").remove(); },
    buttons: {
      'no': function() { if (function_no!=null) function_no(); $(this).dialog('close'); },
      'yes': function() { if (function_yes != null) function_yes(); $(this).dialog('close'); }
    }
  });
  $('#dialog_confirmbox').dialog('open');
}
/*========================================================================================================
---------[LoadingMessage]-------------------------------------------------------------------------------------
==========================================================================================================*/
function LoadingFormOpen() {
  var msgbox = '<div id="dialog_loading" title="Loading, please wait ..." style="magrin:0">'
  msgbox += '<img src="/Zip/Image?path=/content/images/ajax-loader-bar.gif" />';
  msgbox += '</div>';
  $(msgbox).appendTo("body");
  $("#dialog_loading").dialog({
    bgiframe: true,
    modal: true,
    width: 250,
    minHeight: 50,
    closeOnEscape: false,
    resizable: false,
    open: function(event, ui) { $(".ui-dialog-titlebar-close").hide(); },
    close: function() { $("#dialog_loading").remove(); $(".ui-dialog-titlebar-close").show(); }
  });
  $('#dialog_loading').dialog('open');
}
function LoadingFormClose() {  
  $('#dialog_loading').dialog('close');
}
/*========================================================================================================
---------[jqGrid]-------------------------------------------------------------------------------------
==========================================================================================================*/
function AfterSubmitFunction(response, postdata) {
  var success = true;
  var message = ""
  var json = eval('(' + response.responseText + ')');
  if (json.Status != "SUCCESS") {
    success = false;
    message += json.Message;
    if (json.Details != null) {
      message += '<br/><ul>'
      $.each(json.Details, function(i, item) {
        message += '<li>' + item + '</li>';
      });
      message += '</ul>';
    }
  }
  var new_id = "1";
  return [success, message, new_id];
}
function InlineRowEditingResult(response) {
  var success = true;
  var message = ""
  var json = eval('(' + response.responseText + ')');
  if (json.Status != "SUCCESS") {
    success = false;
    message += json.Message;
    if (json.Details != null) {
      message += '<br/><ul>'
      $.each(json.Details, function (i, item) {
        message += '<li>' + item + '</li>';
      });
      message += '</ul>';
    }
    MessageBox('Error', message, '', 'error');
  }
  return success;
}