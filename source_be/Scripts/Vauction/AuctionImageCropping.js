var current_image_width, current_image_height;

function InitImageCroppingForm(auction_id, si, mi, mip, w, h) {
  $("#form_cropping_auction_id").val(auction_id);
  $("#form_cropping_smallimage").val(si);
  $("#form_cropping_mediumimage").val(mi);
  $("#form_cropping_Source").html('<img width="'+w+'px" height="'+h+'px" id="form_cropping_imgSource" />');
  $("#form_cropping_imgPreview,#form_cropping_imgSource").attr("src", mip);
  $("#form_cropping").dialog('open');
  InitJCrop();
  current_image_width = w;
  current_image_height = h;
}

function showPreview(coords) {
  if (parseInt(coords.w) <= 0) return;
  var rx = 96 / coords.w;
  var ry = 96 / coords.h;

  $('#form_cropping_imgPreview').css({
    width: Math.round(rx * current_image_width) + 'px',
    height: Math.round(ry * current_image_height) + 'px',
    marginLeft: '-' + Math.round(rx * coords.x) + 'px',
    marginTop: '-' + Math.round(ry * coords.y) + 'px'
  });

  $('#form_cropping_X').val(coords.x);
  $('#form_cropping_Y').val(coords.y);
  $('#form_cropping_W').val(coords.w);
  $('#form_cropping_H').val(coords.h);
}

var jcrop_api;
function InitJCrop() {
  jcrop_api = $.Jcrop('#form_cropping_imgSource', {
    setSelect: [0, 0, 96, 96],
    minSize:[96, 96],
    onChange: showPreview,
    onSelect: showPreview,
    bgColor: '#222',
    bgOpacity: 0.4,
    aspectRatio: 1
  });
}

function SaveNewThumbnail() {
  ConfirmBox('Create new thumbnail', 'Do you really want to create the new thumbnail?', function () {
    LoadingFormOpen();
    jcrop_api.disable();
    jcrop_api.destroy();
    jcrop_api.release();
    jcrop_api = null;
    $("#form_cropping_imgPreview,#form_cropping_imgSource").attr("src", "");
    $.post('/Auction/CropImage', { auction_id: $("#form_cropping_auction_id").val(), smallimage: $("#form_cropping_smallimage").val(), mediumimage: $("#form_cropping_mediumimage").val(), X: $('#form_cropping_X').val(), Y: $('#form_cropping_Y').val(), W: $('#form_cropping_W').val(), H: $('#form_cropping_H').val() }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Creating new thumbnail", data.Message, '', "error");
          break;
        case "SUCCESS":
          break;
      }
      $("#form_cropping").dialog('close');
      $(image_table_name).trigger('reloadGrid');
    }, 'json');
  });
}

$(document).ready(function () {
  $("#form_cropping").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 620,
    width: 700,
    modal: true,
    gridview: true,
    beforeclose: function (event, ui) {},
    buttons: {
      'Cancel': function () { $("#form_cropping").dialog('close'); },
      'Save': function () { SaveNewThumbnail(); }
    },
    close: function () { }
  });
});