using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Configuration;
using log4net;
namespace Sys.Utility
{
   public class Loger
   {
       private static ILog logInfo = LogManager.GetLogger("loginfo");
       private static ILog logDebug = LogManager.GetLogger("logdebug");
       static Loger()
       {           

       }
       public Loger()
       { 
       
       }
       public static void Error(Exception e,object request=null)
       {
           string rtn = StringUtility.GetMessage(e);
           if (request != null)
           {
               List<string> frms = new List<string>();
               foreach (string k in ((HttpRequestBase)request).Form.AllKeys)
               {
                   frms.Add(string.Format("{0}={1}", k, ((HttpRequestBase)request).Form[k]));
               }
               rtn += "\r\n   " + string.Format("{0}\r\n    {1}", ((HttpRequestBase)request).RawUrl, string.Join(",", frms.ToArray()));
           }
           Loger.Error(rtn);
       }
       public static void Error(string msg)
       {
           logInfo.Error(msg);
       }
       public static void Info(object msg)
       {
           logInfo.Info(msg);
       }
       public static void Debug(object msg)
       {
           logDebug.Debug(msg);
       }

       public static void ConsoleLine(string module, string msg)
       {
           Console.WriteLine("{0}>{1} {2}", module, DateTime.Now, msg);
       }
   }
}
