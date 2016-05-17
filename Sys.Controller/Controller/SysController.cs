using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Sys.Utility;
using MobileWx.Model;
namespace Sys.Controller
{
    public class SysController : System.Web.Mvc.Controller
    {
        public static string WsServer = ConfigurationManager.AppSettings["WsServer"];
        public const string wx_token = "fjbs_mobile";
        private bool endAction = false;
        protected override void  OnActionExecuting(ActionExecutingContext filterContext)
        {
            object[] ws = filterContext.ActionDescriptor.GetCustomAttributes(typeof(WsAttribute),true);
            if (ws.Length > 0 && !string.IsNullOrEmpty(WsServer))
            {
                endAction=true;
                filterContext.Result = Js(new SysResult());
                return;
            }
            base.OnActionExecuting(filterContext);
        }
        protected virtual SysResult OnWsExecuted(SysResult result)
        {
            return result;
        }
        protected virtual string OnWsExecuting()
        {
            return null;
        }
        protected string GetRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            WebResponse resp = request.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            string s = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return s;
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (endAction)
            {
                filterContext.Cancel = true;
                try
                {
                    string s = GetContentByUrl(WsServer + Request.RawUrl.Substring(Request.RawUrl.ToLower().IndexOf(Request.ApplicationPath) + Request.ApplicationPath.Length) + OnWsExecuting()
                        , Encoding.UTF8);
                    SysResult rtn = OnWsExecuted(JsonUtility.DeserializeByNewton<SysResult>(s));
                    filterContext.HttpContext.Response.ContentType = ResponseContentType.text;
                    filterContext.HttpContext.Response.Write(JsonUtility.SerializerByNewton(rtn));
                    filterContext.HttpContext.Response.End();
                    return;
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
            base.OnResultExecuting(filterContext);
        }
        #region Method
        protected JsResult Js(object data)
        {
            JsResult js = new JsResult();
            js.Data = data;
            js.ContentEncoding = Encoding.UTF8;
            return js;
        }
        protected ContentResult Str(string content)
        {
            ContentResult s = new ContentResult();
            s.Content = content;
            s.ContentEncoding = Encoding.UTF8;
            s.ContentType = ResponseContentType.text;
            return s;
        }
        protected string GetContentByUrl(string url,Encoding encode,string postData=null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (postData != null)
            {
                request.Method = "POST";
                Stream sw = request.GetRequestStream();
                List<byte> reqlst = new List<byte>();
                reqlst = encode.GetBytes(postData).ToList();
                foreach (byte b in reqlst)
                {
                    sw.WriteByte(b);
                }
            }

            WebResponse resp = request.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream(), encode);
            string s = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return s;
        }
       
        #endregion
    }
   
    public class ResponseContentType
    {
        /// <summary>
        /// text/html
        /// </summary>
        public const string text = "text/html";
        public const string json = "application/json";
    }
}
