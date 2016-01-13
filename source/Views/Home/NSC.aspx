<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Lelands.com at The National - August 4th-8th, Booth 321&420 - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
  <style type="text/css">
    table tr th {color: #D70000}
    table tr td {vertical-align:top}
  </style>
</asp:Content>

<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
 <% decimal version;bool isIE6=(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE");%>
<div id="left_column">    
    <img alt="" width="173" height="574" src="<%=isIE6?"/public/images/left_side_banner_1.jpg":"" %>" />
  </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server"> 
  <div class="center_content" >
  <div class="page_title">Lelands.com at The National - August 4th-8th - Booth 321 & 420 </div>  
  <div style="text-align:justify; font-size:14px;">
    <b>Dear Friends:</b><br />
      Yes, the National Sports Collectors Convention is almost upon us. This is to remind you that we will be accepting consignments of classic sports memorabilia, vintage sports cards and non sports cards, plus all types of Americana. We will also be buying outright and will have our normal boatload of cash on hand. Please stop by our booth <b>321</b> & <b>420</b> and visit us whether you have something to consign or sell, or just to say hello. Our charming office personnel will also be on hand joining company principles Mike Heffner and Josh Evans. We will also have some amazing things to sell if you are looking for that special treasure. 
    <br /><br />
    If you would like to make an appointment let us know. You can meet with us privately at the show, we can stop by your home to view your collection in the area, or stop by on the way south to Baltimore. 
    <br /><br />
    Looking forward to seeing you.
    <br /><br />
    <b>Josh Evans and Mike Heffner</b><br />
    Lelands.com | 516.409.9700
    <br /><br />
    Here are just some of the categories we are looking for stuff in:    
  </div>
    <table style="table-layout:fixed">
      <colgroup><col width="350px" /><col width="350px" /></colgroup>
      <tr>
        <th>Sports</th>
        <th>Americana</th>
      </tr>
      <tr>
        <td>
          <ul>
            <li>Game used jerseys of HOFers in all sports</li>
            <li>Game Used Bats and Equipment</li>
            <li>Basketball jerseys and memorabilia</li>
            <li>Championship Rings, Trophies and Awards</li> 
            <li>Old game programs and publications of all types</li>
            <li>Autographs of all kinds (signed photos, documents, signed baseballs, etc.)</li>
            <li>Vintage Photographs</li>
            <li>19th Century Baseball</li>
            <li>Early Sports Advertising</li>
            <li>Negro League Memorabilia</li>
            <li>Old Tobacco Cards</li>
            <li>19th Century Baseball Cards</li>
            <li>Football, Boxing, Hockey Cards pre-1970</li>
            <li>Football Memorabilia</li>
            <li>Boxing Memorabilia (programs, tickets, autographs, posters, Ali)</li>
            <li>Olympics, Golf, Tennis, Horse Racing, all sports</li>
            <li>Hockey (game used jerseys & equipment, autographs, anything early)</li>
            <li>Baltimore Orioles & Washington Senators</li>
            <li>Brooklyn Dodgers</li>
            <li>NY Yankees 1903-2010</li>
            <li>Large Collections and Accumulations</li>
            <li>Archives and Institutional Collections</li>
            <li>One of a kind pieces</li>
            <li>Anything Expensive</li>
          </ul>
        </td>
        <td>
          <ul>
            <li>Autographs of Famous People</li>
            <li>Vintage Photography</li>
            <li>Rock ‘n Roll Memorabilia and Records</li>
            <li>Hollywood, TV and Entertainment Memorabilia</li>
            <li>Comic Books and Character Memorabilia</li>
            <li>Old Toys</li>
            <li>Political & Military</li>
            <li>Space and NASA</li>
            <li>Coin-Opsa</li>
            <li>Any very unusual and high quality pieces</li>
          </ul>
        </td>
      </tr>
    </table>
    <img src="<%=this.ResolveUrl("~/public/images/natl_floor_2010.jpg") %>" />
    <br /><br />
    More information about the National - <a href="http://www.nsccshow.com/" style="font-size:14px">National Sports Collectors Convention</a>
  </div>
</asp:Content>

<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
<%decimal version; if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
    { %>
  <script type="text/javascript">
    $(document).ready(function() {
      $("#left_column img").attr("src", "/public/images/left_side_banner_1.jpg");
    });
  </script>
  <%} %>
</asp:Content>
