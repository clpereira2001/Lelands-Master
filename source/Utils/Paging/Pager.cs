using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vauction.Utils.Paging
{
	public class Pager
	{
		private ViewContext viewContext;
		private readonly int pageSize;
		private readonly int currentPage;
		private readonly int totalItemCount;
		private readonly RouteValueDictionary linkWithoutPageValuesDictionary;

		public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
		{
			this.viewContext = viewContext;
			this.pageSize = pageSize;
			this.currentPage = currentPage;
			this.totalItemCount = totalItemCount;
			this.linkWithoutPageValuesDictionary = valuesDictionary;
		}

    public string RenderHtml()
    {
      int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
      int maxElemCount = 7;
      int minElem = 3;

      var sb = new StringBuilder();

      if (pageCount == 1)
      {
        sb.Append("<span class=\"pager\">");
        sb.Append(String.Format("<span class=\"page_box disabled\" style='vertical-align:top;'>{0} item(s)</span>", totalItemCount, pageCount));
        sb.Append("</span>");
        return sb.ToString();
      }

      sb.Append("<span class=\"pager\">");
      sb.Append(String.Format("<span class=\"page_box disabled\" style='vertical-align:top;'>{0} item(s) in {1} page(s)</span>", totalItemCount, pageCount));
      
      // Previous
      sb.Append(this.currentPage > 1?GeneratePageLink("&laquo; Previous&nbsp;|&nbsp;", this.currentPage - 1):"<span class=\"page_box disabled\" style='vertical-align:top;'>&laquo; Previous&nbsp;|&nbsp;</span>");

      int start = 1;
      int end = pageCount;
      if (pageCount > maxElemCount)
      {
        int middle = (int)Math.Ceiling(minElem / 2d) - 1;
        int below = (this.currentPage - middle);
        int above = (this.currentPage + middle);

        if (below < minElem)
        {
          above = minElem;
          below = 1;
        }
        else if (above > (pageCount - minElem + 1))
        {
          above = pageCount;
          below = (pageCount - minElem + 1);
        }
        start = below;
        end = above;
      }

      if (pageCount <= maxElemCount)
      {
        for (int i = 1; i <= pageCount; i++)
          sb.Append(i == this.currentPage ? String.Format("<span class=\"page_box current\">{0}</span>", i) : GeneratePageLink(i.ToString(), i));
      }
      else
      {
        if (start >= minElem){
          for(int i=1; i<=(currentPage-minElem>minElem?minElem:currentPage-minElem);i++)
            sb.Append(GeneratePageLink(i.ToString(), i));
          sb.Append("<span class=\"page_box disabled\" style='vertical-align:top;'>...</span>");
        }

        if (currentPage == pageCount - minElem + 1)
          sb.Append(GeneratePageLink((currentPage - 1).ToString(), (currentPage - 1)));
        
        for (int i=start; i<=end; i++)
          sb.Append(i == this.currentPage ? String.Format("<span class=\"page_box current\">{0}</span>", i) : GeneratePageLink(i.ToString(), i));
        if (currentPage == minElem)
          sb.Append(GeneratePageLink((currentPage + 1).ToString(), (currentPage + 1)));

        if (pageCount - start >= minElem)
        {
          sb.Append("<span class=\"page_box disabled\" style='vertical-align:top;'>...</span>");
          for (int i = (pageCount-minElem > end? pageCount - minElem + 1 : currentPage + minElem); i <= pageCount; i++)
           sb.Append(GeneratePageLink(i.ToString(), i));
        }
      }
     
      // Next
      sb.Append(this.currentPage < pageCount?GeneratePageLink("&nbsp;|&nbsp;Next &raquo;", (this.currentPage + 1)) : "<span class=\"page_box disabled\">&nbsp;|&nbsp;Next &raquo;</span>");
      
      sb.Append("</span>");
      return sb.ToString();
    }

    //public string RenderHtml()
    //{
    //  int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
    //  int nrOfPagesToDisplay = 5;

    //  var sb = new StringBuilder();

    //        sb.Append("<span class=\"pager\">"); 
    //        sb.Append(String.Format("<span class=\"page_box disabled\" style='vertical-align:middle;'>{0} item(s) in {1} page(s)</span>", totalItemCount, pageCount));
    //  // Previous
    //  if (this.currentPage > 1)
    //  {
    //            sb.Append(GeneratePageLink("&laquo; Previous&nbsp;|&nbsp;", this.currentPage - 1));

    //    //sb.Append("<span class=\"page_box disabled\">&nbsp;|&nbsp;Page: </span>");
    //  }
    //  else
    //  {
    //            sb.Append("<span class=\"page_box disabled\" style='vertical-align:middle;'>&laquo; Previous&nbsp;|&nbsp;</span>");
    //  }

    //  int start = 1;
    //  int end = pageCount;

    //  if (pageCount > nrOfPagesToDisplay)
    //  {
    //    int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
    //    int below = (this.currentPage - middle);
    //    int above = (this.currentPage + middle);

    //    if (below < 4)
    //    {
    //      above = nrOfPagesToDisplay;
    //      below = 1;
    //    }
    //    else if (above > (pageCount - 4))
    //    {
    //      above = pageCount;
    //      below = (pageCount - nrOfPagesToDisplay);
    //    }

    //    start = below;
    //    end = above;
    //  }

    //  if (start > 3)
    //  {
    //    sb.Append(GeneratePageLink("1", 1));
    //    sb.Append(GeneratePageLink("2", 2));
    //    sb.Append("...");
    //  }
    //  for (int i = start; i <= end; i++)
    //  {
    //    if (i == this.currentPage)
    //    {
    //      sb.AppendFormat("<span class=\"page_box current\">{0}</span>", i);
    //    }
    //    else
    //    {
    //      sb.Append(GeneratePageLink(i.ToString(), i));
    //    }
    //  }
    //  if (end < (pageCount - 3))
    //  {
    //    sb.Append("...");
    //    sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
    //    sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
    //  }

    //  // Next
    //  if (this.currentPage < pageCount)
    //  {
    //    //sb.Append("<span class=\"page_box disabled\">&nbsp;|</span>");

    //            sb.Append(GeneratePageLink("&nbsp;|&nbsp;Next &raquo;", (this.currentPage + 1)));
    //  }
    //  else
    //  {
    //            sb.Append("<span class=\"page_box disabled\">&nbsp;|&nbsp;Next &raquo;</span>");
    //  }

    //        sb.Append("</span>");

    //  return sb.ToString();
    //}

    private string GeneratePageLink(string linkText, int pageNumber)
    {
      var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);

      if (!pageLinkValueDictionary.ContainsKey("page"))
      {
        pageLinkValueDictionary.Add("page", pageNumber);
      }
      else
        pageLinkValueDictionary["page"] = pageNumber;
      //var virtualPathData = this.viewContext.RouteData.Route.GetVirtualPath(this.viewContext, pageLinkValueDictionary);
      var tmpRequestContext = this.viewContext.RequestContext;
      if (this.viewContext.ViewData["PageActionPath"] != null && !String.IsNullOrEmpty(this.viewContext.ViewData["PageActionPath"].ToString()))
        tmpRequestContext.RouteData.Values["action"] = this.viewContext.ViewData["PageActionPath"].ToString();

      var virtualPathData = RouteTable.Routes.GetVirtualPath(tmpRequestContext, pageLinkValueDictionary);
      //var virtualPathData = RouteTable.Routes.GetVirtualPath(this.viewContext.RequestContext, pageLinkValueDictionary);

      if (virtualPathData != null)
      {
        string linkFormat = "<a class=\"page_box\" href=\"{0}\">{1}</a>";
        return String.Format(linkFormat, virtualPathData.VirtualPath, linkText);
      }
      else
      {
        return null;
      }
    }
	}
}