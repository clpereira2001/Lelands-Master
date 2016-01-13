<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>

<% if (Model.LinkParams.Category_ID == 673){ %>
  <div class="items_notice">
    <%--The Famous Rock Photographers<br />--%>
    &nbsp;&nbsp;&nbsp;The works of the following men and women represent some of the greatest rock n roll photographers who ever roamed backstage, peeked around the corners of hotel lobbies, or experienced the full on effect of these gyrating legends on stage.  The names here you may recognize: Bob Gruen, Jim Marshall, Linda McCartney and others.  But their art you will never forget.<br />
    &nbsp;&nbsp;&nbsp;All the pieces here are approximately 8x10”.  Some have printed later but within 10 years or so of when they were originally shot.  However all are the works of those artists with their stamps and/or handwriting on the versos.  All are vintage prints from the photographers themselves and are printed by them off their one-of-a-kind “from the camera” original negatives.
  </div>      
<%} else if (Model.LinkParams.ID==65789 || Model.LinkParams.ID==65790){ %>
   <div class="items_notice">
    <%--The James J. Kriegsmann Collection<br />--%>
    From a top “The Crossroads of the World,” James J. Kriegsmann’s studio produced iconic images of the biggest names in show business for 4 decades.  Located in New York City’s Times Square, it welcomed an endless stream of stars:  Sinatra, The Andrews Sisters, Buddy Holly, The Ink Spots, Paul Anka, The Supremes, Johnny Carson and many more. With an artistry previously unknown in the world of publicity photographs, Kriegsmann captured the essence of each subject. Now, Lelands offers the unique opportunity to own these legendary items, many one-of-a-kind and previously unknown.
  </div>     
<%} %>