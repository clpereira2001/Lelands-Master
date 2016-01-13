<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% List<SelectListItem> blank = new List<SelectListItem>(); %>
<div id="form_dow" title="DOW information" class="dialog_form">
  <%=Html.Hidden("form_dow_Auction_ID") %>
  <div id="form_dow_tabs" style="margin:0px">
    <ul>
      <li><a href="#form_dow_tabs-1">General</a></li>
      <li><a href="#form_dow_tabs-3">Images</a></li>
    </ul>
    <!-- General -->
    <div id="form_dow_tabs-1">
    <fieldset style="margin:-10px;">
      <table>
        <tr>
          <td colspan="2">
            <p>
              <label for="form_dow_Title">Title:<em>*</em></label>                
              <%=Html.TextBox("form_dow_Title", "", new { @style="width:435px" })%>
            </p>
          </td>
          <td>
            <p>
              <label for="form_dow_Seller" class="inline">Consignor:<em>*</em></label> 
              <button id="form_dow_btnPreviewUser" style="margin-left:2px; float:right" title="Preview consignor's information"><span class="ui-icon ui-icon-person"></span></button><br />              
              <%= Html.TextBox("form_dow_Seller", "", new { @class = "dialog_text", @readonly = "true", @res = "" })%>
            </p>
          </td>
        </tr>
        <tr>
          <td>
            <p>
              <label for="form_dow_MainCategory">Main Category:<em>*</em></label>
              <%= Html.DropDownList("form_dow_MainCategory", blank)%>
            </p>
          </td>
          <td>
            <p>
              <label for="form_dow_Category">Category:<em>*</em></label>
              <%= Html.TextBox("form_dow_Category", "", new { @class = "dialog_text", @readonly = "true", @res = ""})%>
            </p>
          </td>
          <td>            
            <p>
              <label for="form_dow_CommissionRate">Commission Rate:<em>*</em></label>
              <%= Html.DropDownList("form_dow_CommissionRate", blank)%>
            </p>
          </td>
        </tr>          
        <tr>
          <td>
            <p>
              <label for="form_dow_Status">Status:<em>*</em></label>
              <%= Html.DropDownList("form_dow_Status", blank)%>
            </p>
            
          </td>
          <td>
            <p>
              <label for="form_dow_Price">Price:<em>*</em></label>
              <%=Html.TextBox("form_dow_Price")%>
            </p>
          </td>
          <td>
            <p>
              <label for="form_dow_Shipping">Shipping:<em>*</em></label>
              <%=Html.TextBox("form_dow_Shipping")%>
            </p>
          </td>   
          </tr>           
          <tr>            
          <td>
             <p>
              <label for="form_dow_Addendum">Addendum:</label>
              <%=Html.TextBox("form_dow_Addendum")%>
            </p>
          </td>            
          <td colspan="2">
            <p>
              <label for="form_dow_OldInventory" class="inline">Old Inventory:</label>
              <button id="form_dow_btnResetOldInventory" style="float:right" title="Reset old inventory value"><span class="ui-icon ui-icon-minus"></span></button><br />
              <%= Html.TextBox("form_dow_OldInventory", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style = "width:440px" })%>
            </p>
          </td>          
        </tr>
        <tr>
          <td colspan="3">
            <p>
              <label for="form_dow_Addendum">Description:</label>
              <%= Html.TextArea("form_dow_Description", "", new { @style = "width:690px;height:210px;font-size:80%;margin:0px" })%>
            </p>
          </td>
        </tr>
     </table>
    </fieldset>  
   </div>   
   <!-- Images -->
    <div id="form_dow_tabs-3">
      <iframe id="form_dow_iframe" frameborder="0" src="<%=this.ResolveUrl("~/Auction/AuctionImages") %>" scrolling="no" width="100%" height="520px"></iframe>
    </div>
  </div>
  
  
</div>

<!-- Additional forms -->
<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dSellers", Title = "Sellers list", Method = "/User/GetSellersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }", ResultElement = "form_dow_Seller", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID", CallbackSuccess = "$('#form_dow_CommissionRate').val(ret.CommRate_ID);$('#dAuctions_list').jqGrid('setPostData', { owner_id: ret.User_ID }); $('#dAuctions_list').trigger('reloadGrid'); $('#form_auction_OldInventory').attr('title', '').attr('value', '').attr('res', '')", CallbackCancel = "$('#form_dow_CommissionRate').val(0); $('#dAuctions_list').jqGrid('setPostData', { owner_id: -1 }); $('#dAuctions_list').clearGridData(false);$('#form_dow_OldInventory').attr('title', '').attr('value', '').attr('res', '');" }); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dCategory", Title = "Categories", Method = "/Category/GetCategoryList/", SortName = "Title", SortOrder = "asc", ColumnHeaders = "'#', 'Title'", Columns = "{ name: 'Cat_ID', index: 'Cat_ID', width: 70}, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_dow_Category", ResultShow = "ret.Title", ResultID = "ret.Cat_ID" }); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctions", Title = "Auctions list", Method = "/Auction/GetItemsForSale/", SortName = "Auction_ID", SortOrder = "desc", ColumnHeaders = "'Auction#', 'Lot', 'Title', 'Price', 'Shipping','Description', 'Category_ID', 'MainCategory_ID', 'Category', 'Addendum', 'CommissionRate'", Columns = "{ name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true }, { name: 'Lot', index: 'Lot', width: 50 }, { name: 'Title', index: 'Title', width: 240}, { name:'Price', index:'Price', width:80, align:'right' }, { name:'Shipping', index:'Shipping', width:1, hidden:true }, { name:'Description', index:'Description', width:1, hidden:true }, { name:'MainCategory_ID', index:'MainCategory_ID', width:1, hidden:true },{ name:'Category_ID', index:'Category_ID', width:1, hidden:true },  { name:'Category', index:'Category', width:1, hidden:true }, { name:'Addendum', index:'Addendum', width:1, hidden:true }, { name:'CommissionRate_ID', index:'CommissionRate_ID', width:1, hidden:true }", ResultElement = "form_dow_OldInventory", ResultShow = "ret.Title", ResultID = "ret.Auction_ID", CallbackSuccess = "$('#form_dow_Title').attr('value', ret.Title);$('#form_dow_Price').attr('value', ret.Price);$('#form_dow_Shipping').attr('value', ret.Shipping);$('#form_dow_Description').attr('value', ret.Description);$('#form_dow_Category').attr('value', ret.Category).attr('title', ret.Category).attr('res', ret.Category_ID);$('#form_dow_CommissionRate').val(ret.CommissionRate_ID);$('#form_dow_Addendum').attr('value', ret.Addendum);$('#form_dow_MainCategory').val(ret.MainCategory_ID);" }); %>

<% Html.RenderPartial("~/Views/User/UserForm.ascx"); %>
<script src="/Scripts/VAuction/UserForm.js" type="text/javascript"></script>

<script type="text/javascript">
  var auction_auctionimageform_url = '<%=this.ResolveUrl("~/Auction/AuctionImages") %>';
</script> 