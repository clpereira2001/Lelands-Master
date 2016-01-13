using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;

namespace Vauction.Utils.Graphics
{
    public class Thumbnail
    {        
        private Bitmap thumbnail;
        private int sizeX, sizeY;
        private bool changeSmallImage;
        private bool forSave = true;        
        private string imageName;

        #region image parameters
        private ImageFormat imageFormat;                
        #endregion


        #region Private methods
        private int BigDelta(int size1, int size2)
        {
            if ((size2 - size1) % 2 == 0)
            {
                return (size2 - size1) / 2;
            }
            return (size2 - size1) / 2 + 1;
        }
        private int SmallDelta(int size1, int size2)
        {
            return (size2 - size1) / 2;
        }
        private void FillThumbs(Bitmap picture, Color back)
        {
            for (int i = 0, posX = 0; i < sizeX; i++)
            {
                for (int j = 0, posY = 0; j < sizeY; j++)
                {
                    if (i < SmallDelta(picture.Width, sizeX) || i >= sizeX - BigDelta(picture.Width, sizeX) ||
                        j < SmallDelta(picture.Height, sizeY) || j >= sizeY - BigDelta(picture.Height, sizeY))
                        thumbnail.SetPixel(i, j, back);
                    else
                        thumbnail.SetPixel(i, j, picture.GetPixel(posX, posY++));
                }
                if (i >= SmallDelta(picture.Width, sizeX) && i < sizeX - BigDelta(picture.Width, sizeX))
                    posX += 1;
            }
        }
        private void CopyThumbs(Bitmap picture)
        {            
            for (int i = 0; i < picture.Width; i++)
            {
                for (int j = 0; j < picture.Height; j++)
                {
                    thumbnail.SetPixel(i, j, picture.GetPixel(i, j));
                }
            }
        }
        private void SetTransparent(Color back, bool transparentBack)
        {
            if (transparentBack)
                thumbnail.MakeTransparent(back);
        }
        #endregion

        protected void Prepare(Color backGround, bool fillBack)
        {
            using (Bitmap fullSize = new Bitmap(imageName, true))
            {
                if (sizeX > fullSize.Width && sizeY > fullSize.Height && !changeSmallImage)
                {
                    forSave = false;
                }
                else
                {
                    double coefX = Convert.ToDouble(fullSize.Width) / sizeX;
                    double coefY = Convert.ToDouble(fullSize.Height) / sizeY;
                    double divider = coefX > coefY ? coefX : coefY;

                    Size smallSize = new Size(Convert.ToInt32(fullSize.Width / divider),
                                               Convert.ToInt32(fullSize.Height / divider));
                    using (Bitmap smallSizePicture = new Bitmap(fullSize, smallSize))
                    {
                        imageFormat = fullSize.RawFormat;
                        if (fillBack)
                        {
                            thumbnail = new Bitmap(sizeX, sizeY, fullSize.PixelFormat);
                            thumbnail.SetResolution(fullSize.HorizontalResolution, fullSize.VerticalResolution);
                            FillThumbs(smallSizePicture, backGround);
                        }
                        else
                        {
                            thumbnail = new Bitmap(smallSize.Width, smallSize.Height, fullSize.PixelFormat);
                            thumbnail.SetResolution(fullSize.HorizontalResolution, fullSize.VerticalResolution);
                            CopyThumbs(smallSizePicture);
                        }                        
                    }                    
                }
            }
        }

        #region c-tors
        public Thumbnail(int x, int y, string fileName):this(x,y, fileName, false, true)
        {

        }
        public Thumbnail(int x, int y, string fileName, bool changeSmall, bool fillBack)
            : this(x, y, fileName, Color.White, false, changeSmall, fillBack)
        {
            
        }
        
        public Thumbnail(int x, int y, string fileName, Color backGround, bool transparentBack, bool changeSmall, bool fillBack)
        {
            sizeX = x;
            sizeY = y;
            changeSmallImage = changeSmall;
            imageName = fileName;

            Prepare(backGround, fillBack);
        }
        #endregion

        
        public void Save(string fileName)
        {
            if (forSave)
            {
                thumbnail.Save(fileName, imageFormat);
            }
            else
            {
                File.Copy(imageName, fileName, true);
            }
        }
        public Bitmap Picture
        {
            get
            {
                return thumbnail;
            }
        }        
    }
   
}
