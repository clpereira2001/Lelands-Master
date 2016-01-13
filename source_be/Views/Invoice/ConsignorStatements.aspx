<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="server">
  <title>Consignor Statements - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Consignor Statements</h2>
  <table>
  <tr>
    <td >
       <label for="invoice_id">Statement#</label>
	     <input type="text" name="invoice_id" id="invoice_id" class="text ui-widget-content" style="width:70px" />
	  
	     <label for="event_id" style="margin-left:8px">Event</label>
	     <%--<input type="text" name="event_id" id="event_id" class="dialog_text" readonly="true" res='' value='' />		   
	     <button class="reset_text" elem='event_id' >-</button>--%>
	     <%=Html.DropDownList("event_id", new List<SelectListItem>(), new { @style="width:220px" })%>

	     <label for="auction_id" style="margin-left:8px">Auction</label>
	     <input type="text" name="auction_id" id="auction_id" class="dialog_text" readonly="true" res='' value='' />		   
	     <button class="reset_text" elem='auction_id' >-</button>
	     
	     <label for="user_id" style="margin-left:8px">Seller</label>
	     <input type="text" name="user_id" id="user_id" class="dialog_text" readonly="true" res='' value='' />		   
	     <button class="reset_text" elem='user_id' >-</button>
	     
	     <button id="btnSearch" style="margin-left:5px">Search</button>
	     <button id="btnReset" >Reset</button>
    </td>
  </tr>
    <tr>       
      <td> 
        <table id="i_list"></table>
        <div id="i_pager"></div>
      </td>
    </tr>    
   <tr>
    <td>
      <div id="tabs">
	      <ul>
		      <li><a href="#tabs-1">Sold Items</a></li>
		      <li><a href="#tabs-2">Unsold Items</a></li>
		      <li><a href="#tabs-3">Payments</a></li>		      
		      <li><a href="#tabs-4">Buyer and Consignor Info</a></li>		      
	      </ul>
	      <div id="tabs-1">
		      <table id="inv_list"></table>
          <div id="inv_pager"></div>
	      </div>
	      <div id="tabs-2">
		      <table id="uns_list"></table>
          <div id="uns_pager"></div>          
	      </div>
	      <div id="tabs-3">
		      <table id="pay_list"></table>
          <div id="pay_pager"></div>
	      </div>
	      <div id="tabs-4">	        
	       <table>
	        <tr><td style="width:510px">
          <div id="dv_sellers_list"><table id="seller_list"></table></div>
          <form method="post" name="seller_form" id="seller_form" action="" title='' style="width:350px;margin:0px;">
            <fieldset>
              <legend>Seller</legend>
              <table>
                <tbody>
	                <tr>
		                <td><label>User#:</label></td>
		                <td><input type="text" name="seller_user_id" readonly="true" id="seller_user_id"/></td>
	                </tr>
	                <tr>
		                <td>Bidder ID (Login)</td>
		                <td><input type="text" name="seller_login" readonly="true" id="seller_login"/></td>
	                </tr>
	                <tr>
		                <td>First Name</td>
		                <td><input type="text" name="seller_first_name" readonly="true" id="seller_first_name"/></td>
	                </tr>
	                <tr>
		                <td>Last Name</td>
		                <td><input type="text" name="seller_last_name" readonly="true" id="seller_last_name"/></td>
	                </tr>
	                <tr>
		                <td>Email</td>
		                <td><input type="text" name="seller_email" readonly="true" id="seller_email"/></td>
	                </tr>
	                <tr>
		                <td>Day Phone</td>
		                <td><input type="text" name="seller_dayphone" readonly="true" id="seller_dayphone"/></td>
	                </tr>	                
	                <tr>
		                <td>Billing Address</td>
		                <td><textarea rows="6" name="seller_bladdress" readonly="true" id="seller_bladdress"></textarea></td>
	                </tr>
	                 <tr>
		                <td>Shipping Address</td>
		                <td><textarea rows="5" name="seller_shaddress" readonly="true" id="seller_shaddress"></textarea></td>
	                </tr>
                </tbody>
              </table>
            </fieldset>
          </form>     
          </td>
          <td>         
            <table id="buyer_list"></table>
            <div id="buyer_pager"></div>
          </td>
          </tr>
          </table>
        </div>	      
      </div>
    </td>
   </tr>
  </table>  
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dAuctions", Title = "Auctions list", Method = "/Auction/GetAuctionsList/", SortName = "Event_ID", SortOrder = "desc", ColumnHeaders = "'Auction#', 'Old Inv.#', 'Lot', 'Title', 'Event'", Columns = "{ name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true }, { name: 'OldInventory_ID', index: 'OldInventory_ID', width: 50 }, { name: 'Lot', index: 'Lot', width: 50}, { name: 'Title', index: 'Title', width: 150}, { name: 'EventTitle', index: 'EventTitle', width: 130 }", ResultElement = "auction_id", ResultShow = "'['+ret.Lot+'] '+ret.Title", ResultID = "ret.Auction_ID" }); %>
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dUsers", Title = "Sellers list", Method = "/User/GetSellersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }", ResultElement = "user_id", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID" }); %>  
  
  <% Html.RenderPartial("~/Views/Shared/ChooseDialog.ascx", new ChooseDialogParam { Name = "dBuyers", Title = "Buyers list", Method = "/User/GetBuyersList/", SortName = "Login", SortOrder = "asc", ColumnHeaders = "'User#', 'Login', 'Name', 'Email', 'Phone'", Columns = "{ name: 'User_ID', index: 'User_ID', width: 50, key: true }, { name: 'Login', index: 'Login', width: 80 }, { name: 'FN', index: 'FN', width: 120}, { name: 'Email', index: 'Email', width: 100}, { name: 'Phone', index: 'Phone', width: 80 }", ResultElement = "buyer_id", ResultShow = "ret.Login+' ('+ret.FN+')'", ResultID = "ret.User_ID", CallbackSuccess = "SellToBuyer(unsolditem_id, ret.User_ID)" }); %>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="jsContent" runat="server">  
  <script type="text/javascript">
    var cstatement_paymentypes = "<%=ViewData["PaymentTypes"] %>";
    var cstatement_commrates = "<%=ViewData["CommissionRates"] %>";
	</script>  
  <%Html.ScriptV("ConsignorStatement.js"); %>  
</asp:Content>