using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace MobileWx.Test
{
    public class HtmlThumbnail
    {
        private static Bitmap m_Bitmap;
        private static int m_BrowserWidth = 580;
        private static int m_BrowserHeight = 10;
        private static string showHtml = "";

        public HtmlThumbnail(string HtmlContent, int BrowserWidth, int BrowserHeight)
        {
            showHtml = HtmlContent;
            m_BrowserHeight = BrowserHeight;
            m_BrowserWidth = BrowserWidth;
        }

        public static Bitmap GetWebSiteThumbnail(string HtmlContent, int BrowserWidth, int BrowserHeight)
        {
            HtmlThumbnail thumbnailGenerator = new HtmlThumbnail(HtmlContent, BrowserWidth, BrowserHeight);
            return thumbnailGenerator.GenerateWebSiteThumbnailImage();
        }

        public Bitmap GenerateWebSiteThumbnailImage()
        {
            Thread m_thread = new Thread(new ThreadStart(_GenerateWebSiteThumbnailImage));
            m_thread.SetApartmentState(ApartmentState.STA);
            m_thread.Start();
            m_thread.Join();
            return m_Bitmap;
        }
        private void _GenerateWebSiteThumbnailImage()
        {
            WebBrowser m_WebBrowser = new WebBrowser();
            m_WebBrowser.ScrollBarsEnabled = false;
            m_WebBrowser.Navigate("about:blank");
            m_WebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser_DocumentCompleted);
            while (m_WebBrowser.ReadyState != WebBrowserReadyState.Interactive)
                Application.DoEvents();
            m_WebBrowser.Dispose();
        }
        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser m_WebBrowser = (WebBrowser)sender;
            m_WebBrowser.Document.Write(showHtml);
            
            /*
            m_WebBrowser.ClientSize = new Size(m_BrowserWidth, m_BrowserHeight);
            m_WebBrowser.ScrollBarsEnabled = false;
            m_Bitmap = new Bitmap(m_WebBrowser.Bounds.Width, m_WebBrowser.Bounds.Height);
            m_WebBrowser.BringToFront();
            m_WebBrowser.DrawToBitmap(m_Bitmap, m_WebBrowser.Bounds);
            */
            m_WebBrowser.ClientSize = new Size(m_WebBrowser.Document.Body.ScrollRectangle.Width, m_WebBrowser.Document.Body.ScrollRectangle.Height);
            m_WebBrowser.ScrollBarsEnabled = false;
            m_Bitmap = new Bitmap(m_WebBrowser.Document.Body.ScrollRectangle.Width, m_WebBrowser.Document.Body.ScrollRectangle.Height);
            m_WebBrowser.BringToFront();
            m_WebBrowser.DrawToBitmap(m_Bitmap, m_WebBrowser.Document.Body.ScrollRectangle);


            m_Bitmap = (Bitmap)m_Bitmap.GetThumbnailImage(360, 200, null, IntPtr.Zero);
        }
    }
}