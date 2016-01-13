<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Update Your Browser - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server"> 
<div id="left_column"><img alt="" width="181" height="461" src="<%=AppHelper.CompressImage("left_side_banner_0.jpg") %>" /></div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <div class="page_title">Update Your Browser</div>
    You are using Internet Explorer 6. Please update your browser to increase safety and your browsing experience. Choose one of the following links to download a modern browser:
    <table>
      <tr>
        <td colspan="2"><a href="http://www.mozilla.com/firefox/" title="Firefox" >Firefox</a></td>
        <td style="width:20px" rowspan="4">&nbsp;</td>
        <td colspan="2"><a href="http://www.opera.com/" title="Opera" >Opera</a></td>
      </tr>
      <tr>
        <td>
          <img src="<%=AppHelper.CompressImage("firefox_64x64.png") %>" width="64px" height="64px" />
        </td>
        <td>
            "The Web is all about innovation, and Firefox sets the pace with dozens of new features, including the smart location bar, one-click bookmarking and blindingly fast performance." - <a href="http://www.mozilla.com/firefox/features/" title="Firefox's features">Firefox's features</a>
        </td>
        <td>
          <img src="<%=AppHelper.CompressImage("opera_64x64.png") %>" width="64px" height="64px"  />
        </td>
        <td>
           "Opera is the only browser that comes with everything you need to be productive, safe and speedy online. Learn everything Opera can do for you..." - <a href="http://www.opera.com/discover/browser/" title="Discover Opera!">Discover Opera!</a>
        </td>
      </tr>      
      <tr>
        <td colspan="2"><a href="http://www.apple.com/safari/" title="Safari" >Safari</a></td>
        <td colspan="2"><a href="http://www.google.com/chrome" title="Chrome" >Chrome</a></td>
      </tr>
      <tr>
        <td>
          <img src="<%=AppHelper.CompressImage("safari_64x64.png") %>" width="64px" height="64px" />
        </td>
        <td>
                "The fastest, easiest-to-use web browser in the world. With its simple, elegant interface, Safari gets out of your way and lets you enjoy the web faster than any browser." - <a href="http://www.apple.com/safari/"  title="Safari's features">Safari's features</a>
        </td>
        <td>
          <img src="<%=AppHelper.CompressImage("chrome_64x64.png") %>" width="64px" height="64px"  />
        </td>
        <td>
          "Google Chrome is a browser that combines a minimal design with sophisticated technology to make the web faster, safer, and easier." - <a href="http://www.google.com/chrome/intl/en/features.html" title="Google Chrome's features">Google Chrome's features</a>
        </td>
      </tr>
    </table>  
  </div>
</asp:Content>