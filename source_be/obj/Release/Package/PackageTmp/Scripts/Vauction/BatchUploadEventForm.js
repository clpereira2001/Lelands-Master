function InitEventFormForAsignImage(data) {
  $('#form_biupload').dialog('open');
  array_images = data;
}
//MarkInvoicesAsPaid
function AsignImagesForm() {
  var ev = $("#form_biupload_event").val();
  if (ev == null) {
    MessageBox("Asign images", "You can't asign images because there are no pending events", "", "error");
    $("#form_biupload").dialog('close');
    return;
  }
  LoadingFormOpen();
  $.post('/Image/AsignImages', { images: array_images, event_id:ev }, function(data) {
    LoadingFormClose();
    $("#img_list").trigger('reloadGrid');
    switch (data.Status) {
      case "ERROR":
        MessageBox("Asign images", data.Message, '', "error");
        break;
      case "SUCCESS":
        MessageBox("Asign images", "The images were asigned successfully.", '', "info");
        break;
    }
    $("#form_biupload").dialog('close');
  }, 'json');  
}
var array_images = null;
//----------------------------------------------------------------------------------------------------------------
$(document).ready(function() {
  $("#form_biupload").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 190,
    width: 380,
    modal: true,
    position: ['center', 'top'],
    close: function() { },
    buttons: {
      'Cancel': function() { $(this).dialog('close'); },
      'Asign images': function() { AsignImagesForm(); }
    }
  });
  InitDropDown('/General/GetPendingEventsJSON', '#form_biupload_event');
});