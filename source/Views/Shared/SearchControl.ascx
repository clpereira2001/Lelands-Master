<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

  <form action="/Home/SearchResult" method="get" id="formSearch">  
   <div style="float:right;">
    <select name="Type" id="Type" style="width:45px; padding-right:0px;">
      <option value="l">Lot</option>
      <option value="t">Title</option>
    </select>
    <input type="text" name="Description" id="Description" style="width:120px; padding-left:0px" />    
    <a title="Search" href='javascript:$("#formSearch")[0].submit();'>Search</a>    
   </div>
  </form>  

<script type="text/javascript">
  $(document).ready(function() {
    $("#formSearch #Description").val("Lot# or Title").focus(function() {
      this.value = "";
      $(this).addClass("typing");
    }).blur(function() {
      if (this.value == "") {
        $(this).removeClass("typing");
        this.value = "Lot# or Title";
      }
    });
  });
</script>