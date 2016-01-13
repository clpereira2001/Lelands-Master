<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_user" title="User" class="dialog_form">

<%
  List<SelectListItem> blank = new List<SelectListItem>();
  blank.Add(new SelectListItem { Text="", Value="", Selected=true });
%>

<%=Html.Hidden("form_user_User_ID") %>
  <div id="form_user_tabs" style="margin:0px">
    <ul>
      <li><a href="#form_user_tabs-1">General</a></li>
      <li><a href="#form_user_tabs-2">References</a></li>
      <li><a href="#form_user_tabs-3">Billing information</a></li>
      <li><a href="#form_user_tabs-4">Shipping information</a></li>		  
      <li><a href="#form_user_tabs-5">Notes</a></li>
      <li><a id="signatureTab" href="#form_user_tabs-6">Signature</a></li>
    </ul>
    <!-- General -->
    <div id="form_user_tabs-1">
    <fieldset style="margin:-10px">
    <table>
     <tr>            
      <td>
        <p>
          <label for="form_user_Login">Login:<em>*</em></label>
          <%= Html.TextBox("form_user_Login")%>
        </p>
      </td>
      <td>
        <p>
            <label for="form_user_Password">Password:<em>*</em></label>
            <%= Html.TextBox("form_user_Password")%>
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_UserType_ID">Type:<em>*</em></label>
          <%= Html.DropDownList("form_user_UserType_ID", blank)%>
        </p>  
      </td>
     </tr>
     <tr>            
      <td>
        <p>
          <label for="form_user_UserStatus_ID">Status:<em>*</em></label>
          <%= Html.DropDownList("form_user_UserStatus_ID", blank)%>
        </p>
      </td>
      <td>
         <p>
          <label for="form_user_CommissionRate_ID">Commission:</label>
          <%= Html.DropDownList("form_user_CommissionRate_ID", blank)%>
        </p>
        
      </td>
      <td>
        <p>
          <label for="form_user_Email">Email:<em>*</em></label>
          <%= Html.TextBox("form_user_Email")%>
        </p>  
      </td>
     </tr>
     <tr>        
      <td>
        <p>
          <label for="form_user_DateIN">Registration Date:</label>
          <%= Html.TextBox("form_user_DateIN")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_IsConfirmed" class="inline">Is Confirmed:</label>
          <%= Html.CheckBox("form_user_IsConfirmed")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_IsModifyed" class="inline">User updated his account:</label>
          <%= Html.CheckBox("form_user_IsModifyed")%>
        </p>
      </td>
     </tr>       
     <tr>
      <td>
       <p>
          <label for="form_user_DayPhone">Phone (1):<em>*</em></label>
          <%= Html.TextBox("form_user_DayPhone")%>                
        </p>
      </td>        
      <td>
        <p>
          <label for="form_user_EveningPhone">Phone (2):</label>
          <%= Html.TextBox("form_user_EveningPhone")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_MobilePhone">Mobile Phone:</label>
          <%= Html.TextBox("form_user_MobilePhone")%>
        </p>
      </td>
     </tr>
     <tr>
      <td>
       <p>
          <label for="form_user_Fax">Fax:</label>
          <%= Html.TextBox("form_user_Fax")%>                
        </p>
      </td>        
      <td>
        <p>
          <label for="form_user_Tax">Taxpayer ID:</label>
          <%= Html.TextBox("form_user_Tax")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_FailedAttempts">Failed Attempts:</label>
          <%= Html.TextBox("form_user_FailedAttempts")%>                
        </p>
      </td>
     </tr>
     <tr>
      <td>
        <p>
          <label for="form_user_IP">Last Entered IP:</label>
          <%= Html.TextBox("form_user_IP") %>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_LastAttempt">Last Entered Date:</label>
          <%= Html.TextBox("form_user_LastAttempt")%>                
        </p> 
      </td>
      <td>
        <p>
          <label for="form_user_Catalog" class="inline">Catalog:</label>
          <%= Html.CheckBox("form_user_Catalog", false, new { @style="float:right" })%>                
        </p>
        <p>
          <label for="form_user_PostCards" class="inline">Postcards</label>
          <%= Html.CheckBox("form_user_PostCards", false, new { @style = "float:right" })%>                  
        </p>
      </td>
     </tr>
     <tr>
      <td>
        <p>
          <label for="form_user_RecieveWeeklySpecials" class="inline">Recieve Weekly Specials:</label>
          <%= Html.CheckBox("form_user_RecieveWeeklySpecials", false, new { @style="float:right" })%>                
        </p>
        <p>
          <label for="form_user_RecieveNewsUpdates" class="inline">Recieve Auction Announcements:</label>
          <%= Html.CheckBox("form_user_RecieveNewsUpdates", false, new { @style = "float:right" })%>                  
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_IsRecievingBidConfirmation" class="inline">Recieve Bid Confirmation:</label>
          <%= Html.CheckBox("form_user_IsRecievingBidConfirmation", false, new { @style = "float:right" })%>                
        </p>
        <p>
          <label for="form_user_IsRecievingOutBidNotice" class="inline">Recieve OutBid Notice:</label>
          <%= Html.CheckBox("form_user_IsRecievingOutBidNotice", false, new { @style = "float:right" })%>                  
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_IsRecievingLotSoldNotice" class="inline">Recieve Lot Sold Notice:</label>
          <%= Html.CheckBox("form_user_IsRecievingLotSoldNotice", false, new { @style = "float:right" })%>                
        </p>
        <p>
          <label for="form_user_IsRecievingLotClosedNotice" class="inline">Recieve Lot Closed Notice:</label>
          <%= Html.CheckBox("form_user_IsRecievingLotClosedNotice", false, new { @style = "float:right" })%>                  
        </p>
      </td>
     </tr>
    </table>  
     </fieldset>    
   </div>
    <!-- References -->
    <div id="form_user_tabs-2">
      <fieldset style="margin:-10px">
      <table>
        <tr>
          <td colspan="3">Reference 1</td>
        </tr>
        <tr>
      <td>
       <p>
          <label for="form_user_AH1">Auction House:</label>
          <%= Html.TextBox("form_user_AH1")%>                
        </p>
      </td>        
      <td>
        <p>
          <label for="form_user_PN1">Phone Number:</label>
          <%= Html.TextBox("form_user_PN1")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_LBD1">Last Bid Date:</label>
          <%= Html.TextBox("form_user_LBD1")%>
        </p>
      </td>
     </tr>
      <tr>
          <td colspan="3">Reference 2</td>
        </tr>
     <tr>
      <td>
       <p>
          <label for="form_user_AH2">Auction House:</label>
          <%= Html.TextBox("form_user_AH2")%>                
        </p>
      </td>        
      <td>
        <p>
          <label for="form_user_PN2">Phone Number:</label>
          <%= Html.TextBox("form_user_PN2")%>                
        </p>
      </td>
      <td>
        <p>
          <label for="form_user_LBD2">Last Bid Date:</label>
          <%= Html.TextBox("form_user_LBD2")%>
        </p>
      </td>
     </tr>
      <tr>
            <td colspan="3">EBay Information</td>
        </tr>
     <tr>
      <td>
       <p>
          <label for="form_user_EBID">Bidder ID:</label>
          <%= Html.TextBox("form_user_EBID")%>                
        </p>
      </td>        
      <td>
        <p>
          <label for="form_user_Feedback">Feedback:</label>
          <%= Html.TextBox("form_user_Feedback")%>                
        </p>
      </td>
      <td>&nbsp;</td>
     </tr>
      </table>
      </fieldset>
     </div>   
   <!-- Billing tab -->
    <div id="form_user_tabs-3">
      <fieldset style="margin:-10px">
      <table>
      <tr>            
        <td>
          <p>
              <label for="form_user_b_FirstName">First Name:<em>*</em></label>
              <%= Html.TextBox("form_user_b_FirstName")%>
          </p>
         </td>
         <td>
          <p>
              <label for="form_user_b_MiddleName">Middle Name:</label>
              <%= Html.TextBox("form_user_b_MiddleName")%>                
          </p>
         </td>
         <td>
          <p>
              <label for="form_user_b_LastName">Last Name:<em>*</em></label>
              <%= Html.TextBox("form_user_b_LastName")%>
          </p>
          </td>
          </tr>         
          <tr>
          <td>
          <p>
              <label for="form_user_b_Address1">Address 1:<em>*</em></label>
              <%= Html.TextBox("form_user_b_Address1")%>
          </p>
          </td>
         <td>
          <p>
              <label for="form_user_b_Address2">Address 2:</label>
              <%= Html.TextBox("form_user_b_Address2")%>                
          </p>
          </td>
         <td>
          <p>
              <label for="form_user_b_City">City:<em>*</em></label>
              <%= Html.TextBox("form_user_b_City")%>
          </p>
          </td>
          </tr>
          <tr>
          <td>
          <p>
              <label for="form_user_b_State">State:<em>*</em></label>
              <%= Html.DropDownList("form_user_b_State", blank)%>
          </p> 
          </td>
         <td>           
          <p>
              <label for="form_user_b_Zip">Zip:<em>*</em></label>
              <%= Html.TextBox("form_user_b_Zip")%>
          </p>            
          </td>
         <td>
          <p>
              <label for="form_user_b_Country">Country:<em>*</em></label>
              <%= Html.DropDownList("form_user_b_Country", blank)%>
          </p>
          </td>
          </tr>
          <tr>
          <td>
            <p>
                <label for="form_user_b_InternationalState">International State:</label>
                <%= Html.TextBox("form_user_b_InternationalState")%>                
            </p>
          </td>
            <td>
              <p>
                <label for="form_user_b_Company">Company:</label>
                <%= Html.TextBox("form_user_b_Company")%>
              </p>
            </td>
            <td>&nbsp;</td>
          </tr>
          </table>            
       </fieldset>
    </div>
   <!-- Shipping tab -->
    <div id="form_user_tabs-4">
      <fieldset style="margin:-10px">	      
      <table>
      <tr>
        <td colspan="3">
          <p>
            <label for="form_user_s_BillingLikeShipping" class="inline">Billing and shipping addresses are identical:</label>
            <%= Html.CheckBox("form_user_s_BillingLikeShipping")%>            
          </p>
        </td>
      </tr>
      <tr>            
        <td>
          <p>
              <label for="form_user_s_FirstName">First Name:</label>
              <%= Html.TextBox("form_user_s_FirstName")%>
          </p>
         </td>
         <td>
          <p>
              <label for="form_user_s_MiddleName">Middle Name:</label>
              <%= Html.TextBox("form_user_s_MiddleName")%>                
          </p>
         </td>
         <td>
          <p>
              <label for="form_user_s_LastName">Last Name:</label>
              <%= Html.TextBox("form_user_s_LastName")%>
          </p>
          </td>
          </tr>          
          <tr>
          <td>
          <p>
              <label for="form_user_s_Address1">Address 1:<em>*</em></label>
              <%= Html.TextBox("form_user_s_Address1")%>
          </p>
          </td>
         <td>
          <p>
              <label for="form_user_s_Address2">Address 2:</label>
              <%= Html.TextBox("form_user_s_Address2")%>                
          </p>
          </td>
         <td>
          <p>
              <label for="form_user_s_City">City:<em>*</em></label>
              <%= Html.TextBox("form_user_s_City")%>
          </p>
          </td>
          </tr>
          <tr>
          <td>
          <p>
              <label for="form_user_s_State">State:<em>*</em></label>
              <%= Html.DropDownList("form_user_s_State", blank)%>
          </p> 
          </td>
         <td>           
          <p>
              <label for="form_user_s_Zip">Zip:<em>*</em></label>
              <%= Html.TextBox("form_user_s_Zip")%>
          </p>            
          </td>
         <td>
          <p>
              <label for="form_user_s_Country">Country:<em>*</em></label>
              <%= Html.DropDownList("form_user_s_Country", blank)%>
          </p>
          </td>
          </tr>
          <tr>
          <td>
            <p>
                <label for="form_user_s_InternationalState">International State:</label>
                <%= Html.TextBox("form_user_s_InternationalState")%>                
            </p>
          </td>
            <td>
              <p>
                <label for="form_user_s_Company">Company:</label>
                <%= Html.TextBox("form_user_s_Company")%> 
            </p>
            </td>
            <td>&nbsp;</td>
          </tr>
          </table>            
       </fieldset>
    </div>
    <!-- Notes tab -->
     <div id="form_user_tabs-5">
      <fieldset style="margin:-10px">	      
      <table>
      <tr>
        <td colspan="3">
          <p>
            <%= Html.TextArea("form_user_s_Notes", "", new { @style = "margin:  -5px -5px -5px -5px; width:690px; height:440px" })%> 
          </p>
        </td>
      </tr>
      </table>
    </fieldset>
    </div>
    <!-- Signature tab -->
    <div id="form_user_tabs-6">
      <fieldset style="margin:-10px">	      
      <table>
      <tr>
        <td colspan="3">
          <label>Signature:</label>
          <div>
              <img style="display: none;" id="signatureImg" src="<%=DiffMethods.PublicImages("blank_image.jpg") %>" alt="signature" />
          <div>
          <button id="btnChangeSignature" class="ui-button ui-state-default">Change Signature</button>
          </div>
          <div class="sigPad" style="display: none;">
            <ul class="sigNav">
              <li class="drawIt"><a href="#draw-it">Draw Signature</a></li>
              <li class="clearButton"><a href="#clear">Clear</a></li>
            </ul>
            <div class="sig sigWrapper">
              <div class="typed"></div>
              <canvas class="pad" width="198" height="55"></canvas>
              <input type="hidden" name="output" id="newSignature" />
            </div>
          </div>
          <button id="btnCancelSignature" class="ui-button ui-state-default" style="display: none; margin-top: 10px;">Cancel</button>
          </div>
        </td>
      </tr>
      </table>
      </fieldset>
    </div>
  </div>
</div>