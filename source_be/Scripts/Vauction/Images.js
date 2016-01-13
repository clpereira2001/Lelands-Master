var image_table_name = null;
function InitImageGrid(img_tbl, img_pager, _height) {
  image_table_name = img_tbl;
  $(img_tbl).jqGrid({
    url: '/Auction/GetAuctionImages/',
    mtype: 'POST',
    datatype: "json",
    height: _height + 'px',
    width: '720',
    colNames: ['#', 'Thumbnail', 'Image', 'File Name', 'Order', 'Select', 'Thumbnail', 'Image', 'Image path', 'Width', 'Height'],
    colModel: [
        { name: 'Image_ID', index: 'Image_ID', width: 50, key: true, search: false },
        { name: 'Image', index: 'Image', width: 130, search: false, sortable: false, align: 'center' },
        { name: 'MImage', index: 'MImage', width: 130, search: false, sortable: false, align: 'center' },
        { name: 'Title', index: 'Title', width: 320, search: false, sortable: false },
        { name: 'Order', index: 'Order', width: 40, search: false, sortable: false, align: 'center' },
        { name: 'chkIsChecked', index: 'chkIsChecked', width: 40, search: false, sortable: false, align: 'center' },
        { name: 'SmallImage', index: 'SmallImage', width: 1, hidden: true },
        { name: 'MediumImage', index: 'MediumImage', width: 1, hidden: true },
        { name: 'MediumImagePath', index: 'MediumImagePath', width: 1, hidden: true },
        { name: 'ImageWidth', index: 'ImageWidth', width: 1, hidden: true },
        { name: 'ImageHeight', index: 'ImageHeight', width: 1, hidden: true}
      ],
    loadtext: 'Loading ...',
    shrinkToFit: false,
    ondblClickRow: function (id) { CreateSmallImage(id); },
    pager: img_pager,
    postData: { auction_id: -1 },
    onSelectRow: function (id) { image_rowID = id; }
  });
  $(img_tbl).jqGrid('navGrid', img_pager, { edit: false, add: false, del: false, search: false, refreshtext: "", afterRefresh: function () { } }).navSeparatorAdd(img_pager);
  //btnAdd
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-plus', title: 'Add new image', onClickButton: function () { InitUploadImageForm(); } }).navSeparatorAdd(img_pager);
  //btnDel
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-trash', title: 'Delete image', onClickButton: function () { DeleteImageByID(image_rowID); } }).navSeparatorAdd(img_pager);
  //btnUp
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-arrowthick-1-n', title: 'Move up  selected image', onClickButton: function () { MoveImage(image_rowID, true); } }).navSeparatorAdd(img_pager);
  //btnDown
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-arrowthick-1-s', title: 'Move down selected image', onClickButton: function () { MoveImage(image_rowID, false); } }).navSeparatorAdd(img_pager);
  //btnSetDefault
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-arrowthickstop-1-n', title: 'Set as a default image', onClickButton: function () { SetDefault(image_rowID); } }).navSeparatorAdd(img_pager);
  //btnSetDefault
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-shuffle', title: 'Resort images', onClickButton: function () { ResortImages(); } }).navSeparatorAdd(img_pager);
  //btnCreateThumbnail
  $(img_tbl).jqGrid('navButtonAdd', img_pager, { caption: "", buttonicon: 'ui-icon-image', title: 'Create a thumbnail', onClickButton: function(){CreateSmallImage(image_rowID);} }).navSeparatorAdd('#img_pager');
}

function InitUploadImageForm() {  
  $("#form_iupload").dialog('open');
}

 function DeleteImageByID(id) {    
    if (id == null) { MessageBox('Deleting the image', 'Please select the image.', '', 'info'); return; }
    ConfirmBox('Deleting the image', 'Do you really want to delete selected image?', function() {  
      LoadingFormOpen();
      var tbl = $(image_table_name).jqGrid('getPostData', 'auction_id');
      $.post('/Auction/DeleteImage', { image_id: id, auction_id: tbl.auction_id }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Deleting the image", data.Message, '', "error");
            break;
         case "SUCCESS":
            $(image_table_name).jqGrid('clearGridData');
            $(image_table_name).trigger('reloadGrid');            
            break;
        }      
      }, 'json');
    });
  }

  $("#form_iupload").dialog({
    bgiframe: true,
    autoOpen: false,
    height: 300,
    width: 390,
    modal: true,
    gridview: true,
    close: function () { $(image_table_name).trigger('reloadGrid'); /* ResortImages(); */ }
  });

  function InitUploader(auction_id, user_id) {
    $("#biuFile").remove();
    $("#form_iupload").html('<input type="file" id="biuFile" />');
    $('#biuFile').uploadify({
      'uploader': '/Content/images/uploadify.swf',
      'script': '/Auction/UploadBatchImage',
      'cancelImg': '/Content/images/cancel.png',
      'scriptData': { 'auction_id': auction_id, 'user_id': user_id },
      'multi': 'true',
      'auto': 'true',
      'fileDesc': 'Image Files (.JPG, .GIF, .PNG)',
      'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
      'method': 'POST'
    });
  }

  function MoveImage(id, up) {
    if (id == null) { MessageBox('Moving the image', 'Please select the image.', '', 'info'); return; }
    LoadingFormOpen();
    var tbl = $(image_table_name).jqGrid('getPostData', 'auction_id');
    $.post('/Auction/MoveImage', { image_id: id, isup: up, auction_id: tbl.auction_id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Moving the image", data.Message, '', "error");
          break;
        case "SUCCESS":
          $(image_table_name).trigger('reloadGrid');            
          break;
      }      
    }, 'json');    
  }
  
   function SetDefault(id) {    
    if (id == null) { MessageBox('Setting as default image', 'Please select the image.', '', 'info'); return; }
    ConfirmBox('Setting as default image', 'Do you really want to set this image as a default image?', function() {
      LoadingFormOpen();
      var tbl = $(image_table_name).jqGrid('getPostData', 'auction_id');
      $.post('/Auction/SetDefaultImage', { image_id: id, auction_id: tbl.auction_id }, function (data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Setting as default image", data.Message, '', "error");
            break;
         case "SUCCESS":
           $(image_table_name).trigger('reloadGrid');            
            break;
        }      
      }, 'json');
    });
  }

  function ResortImages() {    
    var tbl = $(image_table_name).jqGrid('getPostData', 'auction_id');
    LoadingFormOpen();
    $.post('/Auction/ResortAuctionImages', { auction_id: tbl.auction_id }, function (data) {
      LoadingFormClose();
      switch (data.Status) {
        case "ERROR":
          MessageBox("Resorting auction images", data.Message, '', "error");
          break;
        case "SUCCESS":
          $(image_table_name).trigger('reloadGrid');
          break;
      }
    }, 'json');    
  }

  function CreateSmallImage(id){
    if (id == null) { MessageBox('Create a thumbnail', 'Please select the image.', '', 'info'); return; }
    var tbl = $(image_table_name).jqGrid('getPostData', 'auction_id');
    var ret = $(image_table_name).jqGrid('getRowData', id);
    InitImageCroppingForm(tbl.auction_id, ret.SmallImage, ret.MediumImage, ret.MediumImagePath, ret.ImageWidth, ret.ImageHeight);
  }