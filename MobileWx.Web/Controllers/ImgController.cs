using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class ImgController : Controller
    {
        public ActionResult TIMESHARE(string id)
        {
            using (Bitmap bmp = new Bitmap(360, 150))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    id = id.Substring(0, id.IndexOf("."));
                    string sourceUrl = string.Format("http://180.96.21.230:8889/WXGOODSTIMESHARE/TIMESHARE_{0}.png?r={1:yyyyMMddHHMM}", id, DateTime.Now);
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(sourceUrl);
                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    int buffLength = 512;
                    //    byte[] buff = new byte[buffLength];
                    //    int readlength;

                    //    while ((readlength = responseStream.Read(buff, 0, buffLength)) > 0)
                    //    {
                    //        ms.Write(buff, 0, readlength);
                    //    }
                    //}
                    Image img = Image.FromStream(responseStream);
                    g.DrawImage(img, 0, 0, 360, 150);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        return File(ms.ToArray(), "image/png");
                    }
                }
            }
        }

    }
}
