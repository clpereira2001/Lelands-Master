<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Upcoming Events - <%=ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div style="text-align:justify; padding:0px 20px 20px 20px;font-size:12px">
    <div class="page_title">Upcoming Events</div>
    <table cellpadding="0" cellspacing="0">
      <%--<tr>          
        <td style='vertical-align:top;width:210px'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_04_01-03.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>Rich Altman’s Boston Show</span><br /><br />
          <span style='color:#555'>April 1-3, 2011, Shriner’s Auditorium, Wilmington, MA</span><br />
          <a style='font-style:italic' href='http://www.hollywoodcollectibles.com/pages.php?section=2'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_04_08-10.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>Ohio Sports Collector Convention</span><br /><br />
          <span style='color:#555'>April 8-10, 2011, Holiday Inn Select, Strongsville, OH</span><br />
          <a style='font-style:italic' href='http://www.ohiosportscollector.com'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_04_09-10.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>The Happiest Show on Earth</span><br /><br />
          <span style='color:#555'>April 9-10, 2011 - Philadelphia Expo Center, Oaks, PA</span><br />
          <a style='font-style:italic' href='http://www.antiquecityshow.com'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_04_15-17.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>Show Collectors’ Showcase of America</span><br /><br />
          <span style='color:#555'>April 15-17, 2011, Greater Philadelphia Expo Center at Oaks, PA</span><br />
          <a style='font-style:italic' href='http://www.csashows.com'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>--%>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_05_20-22.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>Pittsburgh XXXIII Spring Classic National Sports Card and Memorabilia Show</span><br /><br />
          <span style='color:#555'>May 20-22, 2011</span><br />
          <a style='font-style:italic' href='http://www.jpaulsports.com/shows.html'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_08_03-07.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>32nd National Sports Collectors Convention</span><br /><br />
          <span style='color:#555'>August 3-7, 2011, Donald E Stephens Convention Center Rosemont, IL</span><br />
          <a style='font-style:italic' href='http://www.nsccshow.com'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_08_20-21.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>60th Papermania Plus Antique Paper Advertising & Photography Show</span><br /><br />
          <span style='color:#555'>August 20-21, 2011, XL Center, Hartford, CT</span><br />
          <a style='font-style:italic' href='http://www.papermaniaplus.com'>read more ...</a>
        </td>
      </tr>
      <tr><td colspan="2"><hr /></td></tr>
      <tr>          
        <td style='vertical-align:top'>          
          <img src="<%=AppHelper.CompressImagePath("/public/images/upcomingevents/2011_11.png") %>" />
        </td>
        <td style='text-align:justify;vertical-align:top'>
          <span style='font-weight:bold;color:#222'>26th Annual GBSCC Card & Memorabilia Show</span><br /><br />
          <span style='color:#555'>November 2011 (TBA), Shriner’s Auditorium, Wilmington, MA</span><br />
          <a style='font-style:italic' href='http://www.gbscc.com'>read more ...</a>
        </td>
      </tr>
    </table>
  </div>
</asp:Content>