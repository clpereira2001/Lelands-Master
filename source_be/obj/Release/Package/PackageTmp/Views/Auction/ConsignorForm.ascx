<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_consignor" title="Consignor information" class="dialog_form">
  <%
    List<SelectListItem> blank = new List<SelectListItem> {new SelectListItem {Text = "", Value = "", Selected = true}};
    List<SelectListItem> pageLayout = new List<SelectListItem>
                                        {
                                          new SelectListItem {Text = "Full Page (FP)", Value = "1"},
                                          new SelectListItem {Text = "Half Page (HP)", Value = "2"},
                                          new SelectListItem {Text = "Quarter Page (QP)", Value = "3"},
                                          new SelectListItem {Text = "Standart Size (SD)", Value = "4"}
                                        };

    List<SelectListItem> listingStep = new List<SelectListItem>
                                         {
                                           new SelectListItem {Text = "Copy Needed", Value = "0"},
                                           new SelectListItem {Text = "Written", Value = "1"},
                                           new SelectListItem {Text = "Proofed", Value = "2"},
                                           new SelectListItem {Text = "Approved", Value = "3"}
                                         };

%>
    <%=Html.Hidden("form_consignor_Cons_ID")%>
    <div id="form_consignor_tabs" class="dialog_form_tabs">
      <ul>
	      <li><a href="#form_consignor_tabs-1">General</a></li>
        <li><a href="#form_consignor_tabs-2">Payments</a></li>
	      <li><a href="#form_consignor_tabs-3">New item</a></li>
	      <li><a href="#form_consignor_tabs-4">Items List</a></li>		      
        <li><a href="#form_consignor_tabs-5">Contract</a></li>		      
      </ul>
      <!-- General -->
      <div id="form_consignor_tabs-1">
      <fieldset style="margin:-10px; height:560px;">      
      <table>
        <tr>
          <td colspan="2">
            <p>
              <label for="form_consignor_Seller" class="inline">Consignor:<em>*</em></label> 
              <button id="btnPreviewUser" style="margin-left:2px; float:right" title="Preview consignor's information"><span class="ui-icon ui-icon-person"></span></button>
              <button id="btnAddNewUser" style="float:right" title="Add new consignor"><span class="ui-icon ui-icon-plus"></span></button><br />              
              <%= Html.TextBox("form_consignor_Seller", "", new {@style = "width:420px", @class = "dialog_text", @readonly = "true", @res="" })%>
            </p>
          </td>
        </tr>      
         <tr>
          <td colspan="2">           
            <p>              
              <label for="form_consignor_Specialist">Specialist:<em>*</em></label>
              <%= Html.DropDownList("form_consignor_Specialist", blank, new { @style = "width:428px" })%>
            </p>          
          </td>
        </tr> 
         <tr>
          <td colspan="2">           
            <p>              
              <label for="form_consignor_Event">Event:<em>*</em></label>
              <%= Html.DropDownList("form_consignor_Event", blank, new { @style = "width:428px" })%>
            </p>          
          </td>
        </tr> 
         <tr>
          <td>
            <p>
              <label for="form_consignor_ConsDate">Date:<em>*</em></label> 
              <%= Html.TextBox("form_consignor_ConsDate")%>              
            </p>
          </td>       
          <td style="text-align:right; vertical-align:middle">
            <button id="btnUpdateConsignmentStatement">Add new consignor statement</button>
          </td>
        </tr>       
      </table>      
       </fieldset>
     </div>
     <!-- Payments -->
      <div id="form_consignor_tabs-2">
        <table id="pay_list"></table>
        <div id="pay_pager"></div>        
      </div>
     <!-- New Auction -->
      <div id="form_consignor_tabs-3">
        <%=Html.Hidden("form_consignor_Auction_ID") %>
	      <fieldset style="margin:-10px">
         <table>
          <tr>
            <td>
              <div id="form_consignor_accordion">
                <h3><a href="#">GENERAL INFORMATION</a></h3>
	              <div>
                   <table>
	                    <tr>
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_CommissionRate">Commission Rate:<em>*</em></label>
                          <%= Html.DropDownList("form_consignor_CommissionRate", blank)%>
                        </p>
                      </td>
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_Quantity">Number Of Items:<em>*</em></label>
                          <%=Html.TextBox("form_consignor_Quantity", "1")%>
                        </p>
                      </td>
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_Priority">Page Layout:<em>*</em></label>
                          <%= Html.DropDownList("form_consignor_Priority", pageLayout)%>
                        </p>
                      </td>
                     </tr>
                     <tr>
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_MainCategory">Main Category:<em>*</em></label>
                          <%= Html.DropDownList("form_consignor_MainCategory", blank)%>
                        </p>
                      </td>
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_Category">Category:<em>*</em></label>
                          <%= Html.TextBox("form_consignor_Category", "", new { @class = "dialog_text", @readonly = "true", @res = "" })%>
                        </p>
                      </td>
                      <td colspan="2">              
                        <p>
                          <label for="form_consignor_LOA" class="inline">LOA:</label>
                          <%= Html.CheckBox("form_consignor_LOA")%>
                        </p>            
                      </td>
                     </tr>           
                     <tr>
                       <td colspan="4">
                        <p>
                          <label for="form_consignor_Title">Title:<em>*</em></label>                
                          <%=Html.TextBox("form_consignor_Title", "", new { @style="width:430px" })%>
                        </p>
                      </td>   
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_Price">Reserve:<em>*</em></label>                
                          <%=Html.TextBox("form_consignor_Price")%>
                        </p>
                      </td>
                     </tr>
                     <tr>
                      <td colspan="4">
                        <p>
                          <label for="form_consignor_OldInventory" class="inline">Old Inventory:</label>
                          <button id="btnResetOldInventory" style="float:right" title="Reset old inventory value"><span class="ui-icon ui-icon-minus"></span></button><br />
                          <%= Html.TextBox("form_consignor_OldInventory", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style = "width:432px" })%>
                        </p>              
                      </td>        
                      <td colspan="2">
                        <p>
                          <label for="form_consignor_Step">Listing Step:</label>
                          <%= Html.DropDownList("form_consignor_Step", listingStep) %>
                        </p>
                      </td>  
                     </tr>
                     </table>
                     </div>
                <h3><a href="#">DESCRIPTION</a></h3>
	              <div>
                <table>
                <tr>
                <td>
                  <p>
                    <label for="form_consignor_CopyNotes">Copy Notes:</label>
                    <%= Html.TextArea("form_consignor_CopyNotes", "", new { @style = "width:310px;height:70px;font-size:80%" })%>
                  </p>
                </td>
                <td>
                  <p>
                    <label for="form_consignor_PhotoNotes">Photo Notes:</label>
                    <%= Html.TextArea("form_consignor_PhotoNotes", "", new { @style = "width:310px;height:70px;font-size:80%" })%>
                  </p>
                </td>
                </tr>
                <tr>
                <td colspan="2">
                  <p>
                    <label for="form_consignor_Description">Description: *</label>
                    <%= Html.TextArea("form_consignor_Description", "", new { @style = "width:670px;height:100px;font-size:80%" })%><br />
                    <span style='font-size:80%;margin-left:10px'>Character count limit: <span id="form_consignor_sleft" style="font-weight:bold">0</span> / <span id='form_consignor_stotal' style="font-weight:bold">0</span><span style='float:right'>Disable limit: <%= Html.CheckBox("form_consignor_DisableLimit", false)%></span>
                    <br />* Please try to keep within the <span id="form_consignor_range">0</span> range of characters
                    </span>
                  </p>
                </td>            
                </tr>
              </table>
          </div>
                <h3><a href="#">EXTRAS</a></h3>
	              <div>
                  <table>
                    <tr>
                      <td>
                        <p>
                          <label for="form_consignor_PurchasedWay">Purchased Way:</label>
                        <%= Html.DropDownList("form_consignor_PurchasedWay", blank)%>
                        </p>
                      </td>
                     <td>
                        <label for="form_consignor_PurchasedPrice">Purchased Price:</label>                
                        <%=Html.TextBox("form_consignor_PurchasedPrice")%>
                     </td>
                     <td>&nbsp;</td>
                    </tr>
                    <tr>
                      <td>
                        <p>
                          <label for="form_consignor_SoldWay">Sold Way:</label>
                        <%= Html.DropDownList("form_consignor_SoldWay", blank)%>
                        </p>
                      </td>
                     <td>
                        <label for="form_consignor_SoldPrice">Sold Price:</label>                
                        <%=Html.TextBox("form_consignor_SoldPrice")%>
                     </td>
                     <td>&nbsp;</td>
                    </tr>
                   </table>
                </div>
              </div>
            </td>
          </tr>
           <tr>
             <td style="text-align:right">
                <button id="btnSave">Save</button>
                <button id="btnSaveAdd" style="margin-left:10px">Save and add more</button>
                <button id="btnReset" style="margin-left:10px">Reset</button>
              </td>
           </tr>
	        </table>
         </fieldset>
      </div>
     <!-- Auction lists -->
      <div id="form_consignor_tabs-4">
	      <table id="item_list"></table>
        <div id="item_pager"></div> 
      </div>
<!-- Contract -->
      <div id="form_consignor_tabs-5">
        <h3>Contract Status</h3>
        <div >
          <span id="form_consignor_Contract_StatusID"></span><a id="contractFileLink" style="float: right; margin-right: 15px; display: none;" >Download</a>
        </div>  
        <div>
          <object id="conscontract" type="application/pdf" style="width: 700px; margin-top: 10px; display: none;" height="410">
          </object>
        </div>      
        <div id="contractText">
          <h3>Terms And Conditions</h3>
          <div>
            <%= Html.TextArea("form_consignor_Contract_Text", "", new { @style = "width:700px; height:410px" })%>
          </div>
        </div>
        <button id="btnSaveContractText" class="ui-button ui-state-default">Save & Generate</button>
        <button id="btnRegenerateContract" class="ui-button ui-state-default">Regenerate</button>
        <button id="btnResetContractText" class="ui-button ui-state-default">Reset To Default</button>
      </div>
	  </div>	  
</div>

<div id="form_consignor_print" title="Printing labels" class="dialog_form">
 <fieldset style="margin:-10px">
    <div style="overflow:auto; height:300px">
      <table id="printingLabels">     
        <colgroup>
          <col width="380px" />
          <col width="50px" />
        </colgroup> 
        <tbody>
        </tbody>
      </table>
    </div>
    <br />
    <div style="width:100%; text-align:right">
      <button id="btnSetDefault">Default</button>
      <button id="btnClear" style="margin-left:10px">Clear</button>
      <button id="btnPrint" style="margin-left:10px">Print</button>
    </div>
  </fieldset>
</div>

<!-- Additional forms -->
<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dSellers", Title = "Sellers list", Method = "/User/GetUsersListSearchBox/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate', 'Type', 'Type_ID', 'Status'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 95}, { name: 'Phone', index: 'Phone', width: 40 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }, { name: 'UserType', index: 'UserType', width: 40, search:false, sortable:false }, { name: 'UserType_ID', index: 'UserType_ID', width: 1, hidden:true }, { name: 'UserStatus', index: 'UserStatus', width: 1, hidden:true }", ResultElement = "form_consignor_Seller", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID", CallbackSuccess = " ValidSeller(ret.User_ID, ret.UserType_ID, ret.UserStatus,ret.CommRate_ID, ret.UserType);", CallbackCancel = "SetDefaultCommissionAndClearSeller();" }); %> 

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dCategory", Title = "Categories", Method = "/Category/GetEventCategories/", SortName = "Title", SortOrder = "asc", ColumnHeaders = "'#', 'Title'", Columns = "{ name: 'Cat_ID', index: 'Cat_ID', width: 70}, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_consignor_Category", ResultShow = "ret.Title", ResultID = "ret.Cat_ID" } ); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctions", Title = "Auctions list", Method = "/Auction/GetAuctionsList/", SortName = "Event_ID", SortOrder = "desc", ColumnHeaders = "'Auction#', 'Old Inv.#', 'Lot', 'Title', 'Event', 'MainCategory', 'Category_ID', 'Category', 'CommRate', 'Quantity', 'Priority', 'LOA', 'Price', 'CopyNotes', 'PhotoNotes', 'Description', 'IsLimitDisabled' ", Columns = "{ name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true }, { name: 'OldInventory_ID', index: 'OldInventory_ID', width: 50 }, { name: 'Lot', index: 'Lot', width: 50}, { name: 'Title', index: 'Title', width: 150}, { name: 'EventTitle', index: 'EventTitle', width: 130 }, { name:'MainCategory_ID', index:'MainCategory_ID', width:1, hidden:true},{ name:'Category_ID', index:'Category_ID', width:1, hidden:true},{ name:'Category', index:'Category', width:1, hidden:true},{ name:'CommRate', index:'CommRate', hidden:true, width:1 },{ name:'Quantity', index:'Quantity', hidden:true, width:1 },{ name:'Priority', index:'Priority', hidden:true, width:1 },{ name:'LOA', index:'LOA', hidden:true, width:1 },{ name:'Price', index:'Price', hidden:true, width:1 },{ name:'CopyNotes', index:'CopyNotes', hidden:true, width:1 },{ name:'PhotoNotes', index:'PhotoNotes', hidden:true, width:1 },{ name:'Description', index:'Description', hidden:true, width:1 },{ name:'IsLimitDisabled', index:'IsLimitDisabled', hidden:true, width:1 } ", ResultElement = "form_consignor_OldInventory", ResultShow = "ret.Title", ResultID = "ret.Auction_ID", CallbackSuccess = "$('#form_consignor_Price').attr('value', ret.Price); $('#form_consignor_Title').attr('value', ret.Title); $('#form_consignor_CopyNotes').attr('value', ret.CopyNotes); $('#form_consignor_PhotoNotes').attr('value', ret.PhotoNotes);$('#form_consignor_Description').attr('value', ret.Description); $('#form_consignor_Quantity').attr('value', ret.Quantity); $('#form_consignor_Priority').val(ret.Priority);if (ret.LOA==1) $('#form_consignor_LOA').attr('checked', 'true'); else $('#form_consignor_LOA').removeAttr('checked');" }); %>

<% Html.RenderPartial("~/Views/User/UserForm.ascx"); %>
<%--<%=Html.ScriptVauctionLink("UserForm.js") %>--%>