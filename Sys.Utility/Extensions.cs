using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Sys.Utility
{
    public static class Extensions
    {
        public static void WriteJson(this HttpResponse resp, object o,string dateFormat= SysConstUtility.FullTime)
        {
            resp.ContentType = "text/json";
            resp.Write(JsonUtility.SerializerByNewton(o, dateFormat));
            resp.End();
        }
        public static void WriteJsonString(this HttpResponse resp, string s)
        {
            resp.ContentType = "text/json";
            resp.Write(s);            
        }
        public static byte[][] ToUtf8Bytes(this string[] arr)
        {
            byte[][] rtn = new byte[arr.Length][];
            for (int i = 0; i < arr.Length; ++i)
            {
                rtn[i] = Encoding.UTF8.GetBytes(arr[i]);
            }
            return rtn;
        }
        public static void AppendAttr(this StringBuilder s, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                s.Append(" ");
                s.Append(key);
                s.Append("=\"");
                s.Append(value);
                s.Append("\"");
            }
        }
    }
}
