using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
namespace Sys.Utility
{
    public class ImgUtility
    {
        #region

        public static string TxtWaterMark(string oldfile, Stream s, string addText
         , float left, float top, Font f, Color clr)
        {
            string rtn = "";
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(oldfile);

                System.Drawing.Image.GetThumbnailImageAbort callb = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                System.Drawing.Image bt = new System.Drawing.Bitmap(image.Width, image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bt);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                Brush b = new SolidBrush(clr);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                g.DrawString(addText, f, b, left, top, StrFormat);
                //保存加水印过后的图片,删除原始图片
                switch (Path.GetExtension(oldfile).ToUpper())
                {
                    case ".JPG":
                        bt.Save(s, ImageFormat.Jpeg);
                        break;
                    case ".GIF":
                        bt.Save(s, ImageFormat.Gif);
                        break;
                    case ".PNG":
                        bt.Save(s, ImageFormat.Gif);
                        break;
                    default:
                        bt.Save(s, ImageFormat.Jpeg);
                        break;
                }
                g.Dispose();
                bt.Dispose();
                image.Dispose();
            }
            catch (Exception ex)
            {
                rtn = ex.Message;
            }
            return rtn;
        }
        public static string TxtWaterMark(string oldfile, string newfile, string addText
    , float left, float top, Font f, Color clr)
        {
            string rtn = "";
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(oldfile);

                System.Drawing.Image.GetThumbnailImageAbort callb = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);
                System.Drawing.Image bt = new System.Drawing.Bitmap(image.Width, image.Height);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bt);
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                Brush b = new SolidBrush(clr);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                g.DrawString(addText, f, b, left, top, StrFormat);
                //保存加水印过后的图片,删除原始图片
                switch (Path.GetExtension(oldfile).ToUpper())
                {
                    case ".JPG":
                        bt.Save(newfile, ImageFormat.Jpeg);
                        break;
                    case ".GIF":
                        bt.Save(newfile, ImageFormat.Gif);
                        break;
                    case ".PNG":
                        bt.Save(newfile, ImageFormat.Png);
                        break;
                    default:
                        bt.Save(newfile, ImageFormat.Jpeg);
                        break;
                }
                g.Dispose();
                bt.Dispose();
                image.Dispose();
            }
            catch (Exception ex)
            {
                rtn = ex.Message;
            }
            return rtn;
        }
        protected static bool ThumbnailCallback()
        {
            return false;
        }
        #endregion


        public static bool Resize(string sourceImageFileFullName, int thumbWidth, int thumbHeight, int quality = 90)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceImageFileFullName)
                    || sourceImageFileFullName.IndexOf('.') == -1
                    || thumbWidth <= 0
                    || thumbHeight <= 0)
                {
                    return false;
                }
                string thumbnailFullName = string.Format("{0}_{1}_{2}.jpg",
                    sourceImageFileFullName.Substring(0, sourceImageFileFullName.LastIndexOf('.')), thumbWidth, thumbHeight);
                Image sourceImage = Image.FromFile(sourceImageFileFullName);
                int oheight = sourceImage.Height;
                int owidth = sourceImage.Width;

                Bitmap thumbBmp = new Bitmap(thumbWidth, thumbHeight);
                Graphics thumbGrap = Graphics.FromImage(thumbBmp);
                thumbGrap.FillRectangle(new SolidBrush(Color.White), 0, 0, thumbWidth, thumbHeight);
                if (owidth < thumbWidth || oheight < thumbHeight)
                {
                    thumbGrap.DrawImage(sourceImage, 0, 0, thumbWidth, thumbHeight);
                }
                else if (thumbWidth * oheight < owidth * thumbHeight)
                {
                    thumbGrap.DrawImage(sourceImage, 0, 0, thumbWidth * owidth / oheight, thumbHeight);
                }
                else
                {
                    thumbGrap.DrawImage(sourceImage, 0, 0, thumbWidth, thumbHeight * oheight / owidth);
                }

                ImageCodecInfo codecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(d => d.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                thumbBmp.Save(thumbnailFullName, codecInfo, encoderParameters);
                thumbGrap.Dispose();
                thumbBmp.Dispose();
                sourceImage.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
