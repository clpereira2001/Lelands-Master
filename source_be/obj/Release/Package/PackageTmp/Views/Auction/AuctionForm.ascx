<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_auction" title="Auction information" class="dialog_form">
    <%
        List<SelectListItem> blank = new List<SelectListItem>();
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

        var tags = new SelectList(new List<SelectListItem> { new SelectListItem { Selected = true, Value = "", Text = "" } }, "Value", "Text").Union(new SelectList(ViewData["Tags"] as List<Tag>, "ID", "Title"));
%>
    <%=Html.Hidden("form_auction_Auction_ID") %>
    <div id="form_auction_tabs" style="margin: 0;">
        <ul>
            <li><a href="#form_auction_tabs-1">General</a></li>
            <li><a href="#form_auction_tabs-2">Description</a></li>
            <li><a href="#form_auction_tabs-3">Images</a></li>
            <li><a href="#form_auction_tabs-4">Extra</a></li>
        </ul>
        <!-- General -->
        <div id="form_auction_tabs-1">
            <fieldset style="margin: -10px;">
                <table>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_Event">Event:<em>*</em></label>
                                <%= Html.DropDownList("form_auction_Event", blank)%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Seller" class="inline">Consignor:<em>*</em></label>
                                <button id="form_auction_btnPreviewUser" style="margin-left: 2px; float: right" title="Preview consignor's information"><span class="ui-icon ui-icon-person"></span></button>
                                <br />
                                <%= Html.TextBox("form_auction_Seller", "", new { @class = "dialog_text", @readonly = "true", @res = "" })%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_CommissionRate">Commission Rate:<em>*</em></label>
                                <%= Html.DropDownList("form_auction_CommissionRate", blank)%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_StartDate">Start Date:<em>*</em></label>
                                <%= Html.TextBox("form_auction_StartDate")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_EndDate">End Date:<em>*</em></label>
                                <%= Html.TextBox("form_auction_EndDate")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Status">Status:<em>*</em></label>
                                <%= Html.DropDownList("form_auction_Status", blank)%>
                            </p>
                            <p>
                                <label for="form_auction_Catlog" class="inline">Catlog:</label>
                                <%= Html.CheckBox("form_auction_Catlog")%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                                <label for="form_auction_Title">Title:<em>*</em></label>
                                <%=Html.TextBox("form_auction_Title", "", new { @style="width:435px" })%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Lot">Lot:</label>
                                <%=Html.TextBox("form_auction_Lot")%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_MainCategory">Main Category:<em>*</em></label>
                                <%= Html.DropDownList("form_auction_MainCategory", blank)%>
                            </p>
                        </td>
                        <td colspan="2">
                            <p>
                                <label for="form_auction_Category">Category:<em>*</em></label>
                                <%= Html.TextBox("form_auction_Category", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style = "width:435px" })%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_collection" class="inline">Collection:</label>
                                <button id="form_auction_btnResetCollection" style="float: right" title="Reset collection"><span class="ui-icon ui-icon-minus"></span></button>
                                <br />
                                <%= Html.TextBox("form_auction_collection", "", new { @class = "dialog_text", @readonly = "true", @res = ""})%>
                            </p>
                        </td>
                        <td colspan="2">
                            <p>
                                <label for="form_auction_tags">Tags:</label>
                                <%= Html.DropDownList("form_auction_tags", tags)%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p style="display: none;">
                                <label for="form_auction_Price">Price:<em>*</em></label>
                                <%=Html.TextBox("form_auction_Price")%>
                            </p>
                            <p>
                                <label for="form_auction_Reserve">Reserve:<em>*</em></label>
                                <%=Html.TextBox("form_auction_Reserve")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Estimate">Estimate:</label>
                                <%=Html.TextBox("form_auction_Estimate")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Shipping">Shipping:<em>*</em></label>
                                <%=Html.TextBox("form_auction_Shipping")%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_Quantity">Number Of Items:<em>*</em></label>
                                <%=Html.TextBox("form_auction_Quantity", "1")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_Priority">Page Layout:<em>*</em></label>
                                <%= Html.DropDownList("form_auction_Priority", pageLayout)%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_LOA" class="inline">LOA:</label>
                                <%= Html.CheckBox("form_auction_LOA")%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_Addendum">Addendum:</label>
                                <%=Html.TextBox("form_auction_Addendum")%>
                            </p>
                        </td>
                        <td colspan="2">
                            <p>
                                <label for="form_auction_OldInventory" class="inline">Old Inventory:</label>
                                <button id="form_auction_btnResetOldInventory" style="float: right" title="Reset old inventory value"><span class="ui-icon ui-icon-minus"></span></button>
                                <br />
                                <%= Html.TextBox("form_auction_OldInventory", "", new { @class = "dialog_text", @readonly = "true", @res = "", @style = "width:432px" })%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_IsPulledOut" class="inline">Pulled Out:</label>
                                <%= Html.CheckBox("form_auction_IsPulledOut")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_IsUnsold" class="inline">Unsold:</label>
                                <%= Html.CheckBox("form_auction_IsUnsold")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_IsPrinted" class="inline">Printed:</label>
                                <%= Html.CheckBox("form_auction_IsPrinted")%>
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_Step" class="inline">Writing Step:</label>
                                <%= Html.DropDownList("form_auction_Step", listingStep, new { @style = "width:130px" })%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_LOA" class="inline">Photographed:</label>
                                <%= Html.CheckBox("form_auction_Photographed")%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_LOA" class="inline">In Layout:</label>
                                <%= Html.CheckBox("form_auction_InLayout")%>
                            </p>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
        <!-- Description -->
        <div id="form_auction_tabs-2">
            <fieldset style="margin: -10px;">
                <table>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_CopyNotes">Copy Notes:</label>
                                <%= Html.TextArea("form_auction_CopyNotes", "", new { @style = "width:100%;height:110px;font-size:80%" })%>
                            </p>
                        </td>
                        <td>
                            <p>
                                <label for="form_auction_PhotoNotes">Photo Notes:</label>
                                <%= Html.TextArea("form_auction_PhotoNotes", "", new { @style = "width:100%;height:110px;font-size:80%" })%>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <p>
                                <label for="form_auction_Description">Description:</label>
                                <%= Html.TextArea("form_auction_Description", "", new { @style = "width:670px;height:310px;font-size:80%;" })%><br />
                            </p>
                            <div style='font-size: 80%; margin-left: 10px'>Character count limit: <span id="form_auction_sleft" style="font-weight: bold">0</span> / <span id='form_auction_stotal' style="font-weight: bold">0</span><span style='float: right'>Disable limit: <%= Html.CheckBox("form_auction_DisableLimit")%></span></div>
                            <div style='font-size: 80%; margin-left: 10px'>
                                * Please try to keep within the <span id="form_auction_description_range" class="">7900-8000</span> range of characters
           
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
        <!-- Images -->
        <div id="form_auction_tabs-3">
            <table id="img_list"></table>
            <div id="img_pager"></div>
            <div id="form_iupload" title="Upload images" class="dialog_form">
                <input type="file" id="biuFile" />
            </div>
        </div>
        <!-- Extra -->
        <div id="form_auction_tabs-4">
            <fieldset style="margin: -10px;">
                <table>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_PurchasedWay">Purchased Way:</label>
                                <%= Html.DropDownList("form_auction_PurchasedWay", blank)%>
                            </p>
                        </td>
                        <td>
                            <label for="form_auction_PurchasedPrice">Purchased Price:</label>
                            <%=Html.TextBox("form_auction_PurchasedPrice")%>
         </td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <p>
                                <label for="form_auction_SoldWay">Sold Way:</label>
                                <%= Html.DropDownList("form_auction_SoldWay", blank)%>
                            </p>
                        </td>
                        <td>
                            <label for="form_auction_SoldPrice">Sold Price:</label>
                            <%=Html.TextBox("form_auction_SoldPrice")%>
         </td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </div>
</div>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dSellersA", Title = "Sellers list", Method = "/User/GetSellersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone', 'CommissionRate'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }, { name: 'CommRate_ID', index: 'CommRate_ID', width: 1, hidden:true }", ResultElement = "form_auction_Seller", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID", CallbackSuccess = "$('#form_auction_CommissionRate').val(ret.CommRate_ID); $('#dAuctionsA_list').jqGrid('setPostData', { owner_id: ret.User_ID }); $('#dAuctionsA_list').trigger('reloadGrid');", CallbackCancel = "$('#form_auction_CommissionRate').val(0);$('#dAuctionsA_list').jqGrid('setPostData', { owner_id: -1 }); $('#dAuctionsA_list').trigger('reloadGrid');" }); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dCategoryA", Title = "Categories", Method = "/Category/GetEventCategories/", SortName = "Title", SortOrder = "asc", ColumnHeaders = "'#', 'Title'", Columns = "{ name: 'Cat_ID', index: 'Cat_ID', width: 70}, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_auction_Category", ResultShow = "ret.Title", ResultID = "ret.Cat_ID" }); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctionsA", Title = "Auctions list", Method = "/Auction/GetAuctionsList/", SortName = "Event_ID", SortOrder = "desc", ColumnHeaders = "'Auction#', 'Old Inv.#', 'Lot', 'Title', 'Event', 'MainCategory', 'Category_ID', 'Category', 'CommRate', 'Quantity', 'Priority', 'LOA', 'Price', 'CopyNotes', 'PhotoNotes', 'Description', 'IsLimitDisabled', 'Reserve', 'Estimate', 'Shipping', 'Addendum' ", Columns = "{ name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true }, { name: 'OldInventory_ID', index: 'OldInventory_ID', width: 50 }, { name: 'Lot', index: 'Lot', width: 50}, { name: 'Title', index: 'Title', width: 150}, { name: 'EventTitle', index: 'EventTitle', width: 130 }, { name:'MainCategory_ID', index:'MainCategory_ID', width:1, hidden:true},{ name:'Category_ID', index:'Category_ID', width:1, hidden:true},{ name:'Category', index:'Category', width:1, hidden:true},{ name:'CommRate', index:'CommRate', hidden:true, width:1 },{ name:'Quantity', index:'Quantity', hidden:true, width:1 },{ name:'Priority', index:'Priority', hidden:true, width:1 },{ name:'LOA', index:'LOA', hidden:true, width:1 },{ name:'Price', index:'Price', hidden:true, width:1 },{ name:'CopyNotes', index:'CopyNotes', hidden:true, width:1 },{ name:'PhotoNotes', index:'PhotoNotes', hidden:true, width:1 },{ name:'Description', index:'Description', hidden:true, width:1 },{ name:'IsLimitDisabled', index:'IsLimitDisabled', hidden:true, width:1 },{ name:'Reserve', index:'Reserve', hidden:true, width:1 }, { name:'Estimate', index:'Estimate', hidden:true, width:1 }, { name:'Shipping', index:'Shipping', hidden:true, width:1 }, { name:'Addendum', index:'Addendum', hidden:true, width:1 } ", ResultElement = "form_auction_OldInventory", ResultShow = "ret.Title", ResultID = "ret.Auction_ID", CallbackSuccess = "$('#form_auction_Title').attr('value', ret.Title); $('#form_auction_Quantity').attr('value', ret.Quantity); $('#form_auction_Price').attr('value', ret.Price); $('#form_auction_Priority').val(ret.Priority); if (ret.LOA==1) $('#form_auction_LOA').attr('checked', 'true'); else $('#form_auction_LOA').removeAttr('checked'); $('#form_auction_Description').attr('value', ret.Description); $('#form_auction_CopyNotes').attr('value', ret.CopyNotes); $('#form_auction_PhotoNotes').attr('value', ret.PhotoNotes); $('#form_auction_Reserve').attr('value', ret.Reserve); $('#form_auction_Estimate').attr('value', ret.Estimate); $('#form_auction_Shipping').attr('value', ret.Shipping); $('#form_auction_Addendum').attr('value', ret.Addendum); " }); %>

<% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctionsCollection", Title = "Collections list", Method = "/Auction/GetCollections/", SortName = "ID", SortOrder = "desc", ColumnHeaders = "'#', 'Title'", Columns = "{ name: 'ID', index: 'ID', width: 70, key: true }, { name: 'Title', index: 'Title', width: 370 }", ResultElement = "form_auction_collection", ResultShow = "ret.Title", ResultID = "ret.ID" }); %>

<script type="text/javascript">
    var current_user_id = parseInt("<%=AppHelper.CurrentUser.ID %>");
</script>
