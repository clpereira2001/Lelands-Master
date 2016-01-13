<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="form_categories_megring" title="Categories merging" class="dialog_form">
  <table id="tblSchema" style="margin-top:5px">
    <tr>
      <td>
        <table id="from_tm_list"></table>
      </td>
      <td style="vertical-align:middle">        
        <button id="btnMove" >></button>
        <img src="<%=this.ResolveUrl("~/Content/images/blue-loading.gif") %>" id="imgMove" style="display:none; width:29px; height:29px;" />
      </td>
      <td>         
        <table id="to_tm_list"></table>
      </td>
     </tr>
     <tr>
      <td style="height:150px">
        <u>Details:</u><br />
         <div id="imgLoadFrom" style="display:none; vertical-align:middle; text-align:center; width:300px; height:100px">
          <img src="<%=this.ResolveUrl("~/Content/images/blue-loading.gif") %>" />
        </div> 
        <p id="from_description">&nbsp;</p>       
      </td>  
      <td>&nbsp;</td>   
      <td style="height:150px">
        <u>Details:</u><br />
        <div id="imgLoadTo" style="display:none; vertical-align:middle; text-align:center; width:300px; height:100px">
          <img src="<%=this.ResolveUrl("~/Content/images/blue-loading.gif") %>" />
        </div>        
        <p id="to_description">&nbsp;</p>        
      </td>
    </tr>
  </table>  
</div>