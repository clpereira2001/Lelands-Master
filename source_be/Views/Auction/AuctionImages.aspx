<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
  <% Html.Style("site.css"); %>
  <% Html.StyleUrl("/content/menu/menu_style.css"); %>
  <% Html.StyleUrl("/content/redmond/jquery-ui-1.7.1.custom.css"); %>
  <% Html.Style("ui.jqgrid.css"); %>

  <% Html.Script("jquery-1.3.2.min.js"); %>
  <% Html.Script("jquery-ui-1.7.2.custom.min.js"); %>
  <% Html.Script("grid.locale-en.js"); %>
  <% Html.Script("jquery.jqGrid.min.js"); %>
  <% Html.Script("MicrosoftAjax.js"); %>
  <% Html.ScriptV("general.js"); %>

  <%= Html.CompressJs(Url) %>
  <%= Html.CompressCss(Url) %>
  <% Html.Clear(); %>
</head>
<body style="background-color:White">
  <% long auction_id = (long)ViewData["Auction_ID"]; %>
   <table id="img_list"></table>
   <div id="img_pager"></div>
   
   <div id="form_upload" title="Upload image" class="dialog_form">
  <% using (Html.BeginForm("UploadImage", "Auction", FormMethod.Post, new { enctype = "multipart/form-data" }))
     { %>
      <%=Html.Hidden("auction_id", auction_id)%>    
      <fieldset style="margin:-10px">    
        <p>
          <label for="Image" style="font-size:14px">File name:</label>
          <input type="file" id="Image" name="Image" size="65" accept="image/gif,image/jpeg,image/png,image/bmp" />
        </p>
      </fieldset> 
      <input type="submit" value="Save" style="display:none" id="btnUpload" />
    <% } %>  
  </div>
  
  <script type="text/javascript">
    //InitUploadImageForm
    function InitUploadImageForm() {
      $("#Image").attr("value", "");
      $("#flName").attr("value", "");
      $("#form_upload").dialog('open');
    }
    //UploadImage
    function UploadImage() {      
      LoadingFormOpen();
      if ($("#Image").attr("value")!='')
      {
        $("#btnUpload").click();
      } else {
        LoadingFormClose();
        return;            
      }
      $("#form_upload").dialog('close');      
    }
    //-------------------------------------
    var image_rowID = null;
    $(document).ready(function() {
      LoadingFormClose();
            
      $("#form_upload").dialog({
        bgiframe: true,
        autoOpen: false,
        height: 170,
        width: 555,
        modal: true,
        gridview: true,
        position: ['center', 'top'],
        overlay: {
          backgroundColor: '#000',
          opacity: 0.5
        },
        resizable: false,      
        buttons: {
          'Cancel': function() { $(this).dialog('close'); },
          'Save': function() { UploadImage(); }
        },
        close : function() {  }
      });
      
      $("#img_list").jqGrid({
        url: '/Auction/GetAuctionImages/',
        mtype: 'POST',
        datatype: "json",
        height: '465px',
        width: 720,
        colNames: ['#', 'Image', 'File Name', 'Order', 'Uploaded File Name'],
        colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 60, key: true, search: false },
          { name: 'Image', index: 'Image', width: 120, search: false, sortable: false, align: 'center' },
          { name: 'Title', index: 'Title', width: 320, search: false, sortable: false },
          { name: 'Order', index: 'Order', width: 50, search: false, sortable: false, align: 'center' },
          { name: 'Title', index: 'Title', width: 130, search: false, sortable: false }
        ],
        loadtext: 'Loading ...',
        shrinkToFit: false,
        rowNum: 100000,
        pager: '#img_pager',
        postData: { auction_id: <%=auction_id %> },
        onSelectRow: function(id) { image_rowID = id; }
      });
      $("#img_list").jqGrid('navGrid', '#img_pager', { edit: false, add: false, del: false, search: false, refreshtext: "", afterRefresh: function() { } }).navSeparatorAdd('#img_pager');
      //btnAdd
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-plus', title: 'Add new image', onClickButton: function() { InitUploadImageForm(); } }).navSeparatorAdd('#img_pager');
      //btnDel
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-trash', title: 'Delete image', onClickButton: function() { DeleteImageByID(image_rowID); } }).navSeparatorAdd('#img_pager');
      //btnUp
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthick-1-n', title: 'Move up  selected image', onClickButton: function() { MoveImage(image_rowID, true); } }).navSeparatorAdd('#img_pager');
      //btnDown
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthick-1-s', title: 'Move down selected image', onClickButton: function() { MoveImage(image_rowID, false); } }).navSeparatorAdd('#img_pager');
      //btnSetDefault
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthickstop-1-n', title: 'Set as a default image', onClickButton: function() { SetDefault(image_rowID); } }).navSeparatorAdd('#img_pager');
    });
    
  function DeleteImageByID(id) {    
    if (id == null) { MessageBox('Deleting the image', 'Please select the image.', '', 'info'); return; }
    ConfirmBox('Deleting the image', 'Do you really want to delete selected image?', function() {  
      LoadingFormOpen();
      $.post('/Auction/DeleteImage', { image_id: id, auction_id : <%=auction_id %> }, function(data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Deleting the image", data.Message, '', "error");
            break;
         case "SUCCESS":
            $('#img_list').jqGrid('clearGridData');
            $("#img_list").trigger('reloadGrid');            
            break;
        }      
      }, 'json');
    });
  }
  
  function MoveImage(id, up) {
    if (id == null) { MessageBox('Moving the image', 'Please select the image.', '', 'info'); return; }
      LoadingFormOpen();
      $.post('/Auction/MoveImage', { image_id: id, isup:up, auction_id : <%=auction_id %> }, function(data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Moving the image", data.Message, '', "error");
            break;
         case "SUCCESS":            
            $("#img_list").trigger('reloadGrid');            
            break;
        }      
      }, 'json');    
  }
  
   function SetDefault(id) {    
    if (id == null) { MessageBox('Setting as default image', 'Please select the image.', '', 'info'); return; }
    ConfirmBox('Setting as default image', 'Do you really want to set this image as a default image?', function() {  
      LoadingFormOpen();
      $.post('/Auction/SetDefaultImage', { image_id: id, auction_id : <%=auction_id %> }, function(data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Setting as default image", data.Message, '', "error");
            break;
         case "SUCCESS":            
            $("#img_list").trigger('reloadGrid');            
            break;
        }      
      }, 'json');
    });
  }
  </script>
</body>
</html>
