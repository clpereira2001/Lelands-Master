using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Utils.Graphics
{
  public class ImageActions
  {
    public static System.Drawing.Size ImageResizing(System.Drawing.Image imgPhoto, int Width, bool IsEqual)
    {
      int sourceWidth = imgPhoto.Width;
      int sourceHeight = imgPhoto.Height;
      int destX = 0;
      int destY = 0;

      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;

      nPercentW = ((float)Width / (float)sourceWidth);
      int Height = (IsEqual) ? Width : (int)((float)nPercentW * (float)sourceHeight);
      nPercentH = ((float)Height / (float)sourceHeight);

      if (nPercentH < nPercentW)
      {
        nPercent = nPercentH;
        destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) * 0.5);
      }
      else
      {
        nPercent = nPercentW;
        destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) * 0.5);
      }

      return new System.Drawing.Size((int)(sourceWidth * nPercent), (int)(sourceHeight * nPercent));
    }
  }
}
