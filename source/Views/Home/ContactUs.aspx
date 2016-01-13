<%@ Page Language="C#"  MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Contact Us - <%= ConfigurationManager.AppSettings["CompanyName"] %></title>
</asp:Content>
<asp:Content ID="leftImg" ContentPlaceHolderID="leftImg" runat="server">
    <% decimal version;
       var isIE6 = (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"); %>
    <div id="left_column">
        <img alt="" width="173" height="461" <%= isIE6 ? "src='" + AppHelper.CompressImage("left_side_banner_2.jpg") + "'" : String.Empty %> />
    </div>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="center_content" style="text-align: justify">
        <div class="page_title">Contact Us</div>
        Below you will find our detailed listings for contacting Lelands.com. For any issues regarding site functionality, issues with placing a bid, any questions or comments about an upcoming auction date or item please direct your questions to the appropriate contact. We appreciate your interest in Lelands.com and thank you for your patronage.
        <br /><br />		
        <h4>Corporate Headquarters</h4>
        <span style='color: brown'>Lelands.com<br/>
            130 Knickerbocker Ave, Suite E<br/>
            Bohemia,NY 11716<br/>
            Phone  1-631-244-0077<br/>
            Phone  631-244-3604<br/>
            Fax 631-750-5216<br/>
            Email:<a href="mailto:info@lelands.com" style="text-size: 14px">info@lelands.com</a><br />
            Office Hours: 9AM-5PM Monday-Friday</span>
        <br /><br />

        <h4>Consignment Services</h4>
        <u>Sports Memorabilia & Americana</u><br/>
        From one item to entire collections, we handle it all.<br/>
        <span style='color: brown'>Please contact us at 631-244-0077<br/>
            E-mail: <a href="mailto:sell@lelands.com" style="text-size: 14px">sell@lelands.com</a></span><br /><br />

        <h4>Customer Support</h4>
        For questions related to registrations, passwords, billing, administrative support and other general questions<br/>
        <span style='color: brown'>Please contact customer service:<br/>
            631-244-0077<br/>
            E-mail:<a href="mailto:laura@lelands.com" style="text-size: 14px">laura@lelands.com</a><br />
            Office Hours: 9AM-5PM Monday-Friday</span>
        <br />
        <br/>
        <u>Lelands Specialist</u><br/>
        Josh Evans: <a href="mailto:&#106;&#111;&#115;&#104;&#101;&#118;&#097;&#110;&#115;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;">joshevans@lelands.com</a><br/>
        Mike Heffner: <a href="mailto:&#109;&#105;&#107;&#101;&#104;&#101;&#102;&#102;&#110;&#101;&#114;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;">mikeheffner@lelands.com</a><br/>
        Kevin Bronson: <a href="mailto:&#107;&#101;&#118;&#105;&#110;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;">kevin@lelands.com</a><br/>
        Casey Samuelson: <a href="mailto:&#105;&#110;&#102;&#111;&#064;&#108;&#101;&#108;&#097;&#110;&#100;&#115;&#046;&#099;&#111;&#109;">info@lelands.com</a><br/>
    </div>
</asp:Content>
<asp:Content ID="j" ContentPlaceHolderID="cphJS" runat="server">
    <% decimal version;
       if (!(Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE"))
       { %>
        <script type="text/javascript">
            $(document).ready(function() {
                $("#left_column img").attr("src", '<%= AppHelper.CompressImage("left_side_banner_2.jpg") %>');
            });
        </script>
    <% } %>
</asp:Content>