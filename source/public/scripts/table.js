function UpdateTableView() {
  $.each($(".tbl_res_row"), function () {
    $(this).css("background-color", $(this).attr("defcolor"));
  });
  //setTimeout(function () {
    $.post('/Auction/UpdateCategoryViewResult', { prms: $("#hd_pgpms").attr("value") }, function (data) {
      if (data != "null") {
        $.each(data, function (i, item) {
          if ($("#cv_cb_" + item.id).text() != item.col1 || $("#cv_b_" + item.id).text() != item.col2)
            $("#tbl_res_row_" + item.id).css("background-color", "#FFA");
          $("#cv_cb_" + item.id).html(item.col1);
          $("#cv_b_" + item.id).html(item.col2);
        });
      }
      $("#lnkRBR,#lnkRBR_b").show();
      $("#lnkRBR_loading,#lnkRBR_loading_b").hide();
    }, 'json');
  //}, 3000);
}
function UpdateGridView() {
  $.each($(".cv_gv_panel"), function () {
    $(this).css("background-color", "#FFF");
  });
  //setTimeout(function () {
    $.post('/Auction/UpdateCategoryViewResult', { prms: $("#hd_pgpms").attr("value") }, function (data) {
      if (data != "null") {
        $.each(data, function (i, item) {
          if ($("#cv_cb_" + item.id).attr("value") != item.col1 || $("#cv_b_" + item.id).text() != item.col2)
            $("#cv_gv_" + item.id).css("background-color", "#FFA");
          $("#cv_gv_" + item.id).html(item.col3);
          $("#cv_cb_" + item.id).attr("value", item.col1);
        });
      }
      $("#lnkRBR,#lnkRBR_b").show();
      $("#lnkRBR_loading,#lnkRBR_loading_b").hide();
    }, 'json');
  //}, 3000);
}