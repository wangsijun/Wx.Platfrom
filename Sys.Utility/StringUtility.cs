using System;
using System.Net;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;
using System.IO;
using System.Security.Cryptography;
namespace Sys.Utility
{
    public class USER_TYPE
    {
        /// <summary>
        /// 0：系统登录处理超时
        /// </summary>
        public const short UT_OVERTIME_USER = 0;
        /// <summary>
        /// 1：手机收费用户
        /// </summary>
        public const short UT_CHARGE_USER = 1;
        /// <summary>
        /// 2：手机免费用户
        /// </summary>
        public const short UT_FREE_USER = 2;
        /// <summary>
        /// 3：手机匿名用户
        /// </summary>
        public const short UT_ANONYMITY_USER = 3;
    };

    //定义服务类型
    public class SERVICE_TYPE
    {
        /// <summary>
        /// 0:过期
        /// </summary>
        public const short ST_OVERDUE = 0;
        /// <summary>
        /// 1:未到期
        /// </summary>
        public const short ST_ONSERVICE = 1;
        /// <summary>
        /// 2:快到期(差7天到期)
        /// </summary>
        public const short ST_WILL_EXPIRED = 2;
        /// <summary>
        /// 3:未体验
        /// </summary>
        public const short ST_UNEXPER = 3;
    };
    public  class StringUtility
    {
        public static string NewId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        /// <summary>
        /// 站点虚拟路径
        /// </summary>
        public static string RootPath
        {
            get
            {
                if (HttpRuntime.AppDomainAppVirtualPath != "/")
                {
                    return HttpRuntime.AppDomainAppVirtualPath;
                }
                return "";
            }
        }
        /// <summary>
        /// 应用程序物理路径
        /// </summary>
        public static string AppPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }
        public static string ExPath
        {
            get { return Request.CurrentExecutionFilePath.ToLower().Substring(Request.CurrentExecutionFilePath.ToLower().IndexOf(Request.ApplicationPath.ToLower()) + Request.ApplicationPath.Length); }
        }
        public static HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }
        public static string GetMessage(Exception e)
        {
            string rtn = e.Message;
            if (e.InnerException != null)
            {
                rtn += e.InnerException.Message;
                if (e.InnerException.InnerException != null)
                {
                    rtn += e.InnerException.InnerException.Message;
                }
                rtn += e.InnerException.StackTrace;
            }
            rtn += e.StackTrace;
            return rtn;
        }
        public static bool HasString(string src, string tag)
        {
            bool rtn = false;
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(tag)) return rtn;
            if (("," + src + ",").IndexOf("," + tag + ",") >= 0) return true;
            return rtn;
        }
        public static bool LikeString(string src, string tag)
        {
            bool rtn = false;
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(tag)) return rtn;
            if (src.IndexOf(tag) >= 0) return true;
            return rtn;
        }
        public static string RemoveHTML(object str)
        {
            if (str + "" == "") return "";
            string s = str + "";
            Regex regScript = new Regex(@"<script[^>]+>[^<]+</script>");
            MatchCollection ms = regScript.Matches(s);
            if (ms != null)
            {
                foreach (Match m in ms)
                {
                    s = s.Replace(m.Value, "");
                }
            }
            Regex reg1 = new Regex(@"<[^>]+>");
            MatchCollection mc = reg1.Matches(s);
            if (mc != null)
            {
                foreach (Match m in mc)
                {
                    s = s.Replace(m.Value, "");
                }
            }
            
            return s;
        }
        /// <summary>
        /// 处理输入的特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterStr(string str)
        {
            string rtn = str + "";
            rtn = rtn.Replace("'", "''");
            return rtn;
        }
        public static string ToString(object obj)
        {
            if (obj == DBNull.Value || obj == null) return null;
            return Convert.ToString(obj);
        }
        public static Int32? ToInt32(object obj)
        {
            if (obj == DBNull.Value || obj == null || obj.ToString().Trim()=="") return null;
            return Convert.ToInt32(obj);
        }
        public static long? ToInt64(object obj)
        {
            if (obj == DBNull.Value || obj == null || obj.ToString().Trim() == "") return null;
            return Convert.ToInt64(obj);
        }
        public static decimal? ToDecimal(object obj)
        {
            if (obj == DBNull.Value || obj == null || obj.ToString().Trim() == "") return null;
            return Convert.ToDecimal(obj);
        }
        public static DateTime? ToDateTime(object obj,string format=null)
        {
            if (obj == DBNull.Value || obj == null || obj.ToString().Trim() == "") return null;
            if (format != null)
            {
                DateTimeFormatInfo f = new DateTimeFormatInfo();
                Regex regY=new Regex(@"y+");
                Regex regM=new Regex(@"M+");
                Regex regD = new Regex(@"d+");
                string y = obj.ToString().Substring(regY.Match(format).Index, regY.Match(format).Length);
                string M = obj.ToString().Substring(regM.Match(format).Index, regM.Match(format).Length);
                string d = obj.ToString().Substring(regD.Match(format).Index, regD.Match(format).Length);
                return Convert.ToDateTime(y+"-"+M+"-"+d);;
            }
            else {
                return Convert.ToDateTime(obj);
            }
        }
        public static bool? ToBool(object obj)
        {
            if (obj == DBNull.Value || obj == null || obj.ToString().Trim() == "") return null;
            return Convert.ToBoolean(obj);
        }
        public static string EnString(object str)
        {
            string rtn = str + "";
            //rtn = rtn.Replace("<", "&lt;");
            //rtn = rtn.Replace(">", "&gt;");
            return rtn;
        }
        public static string DeString(object str)
        {
            string rtn = str + "";
            //rtn = rtn.Replace("&lt;", "<");
            //rtn = rtn.Replace("&gt;", ">");
            return rtn;
        }
        public static string ToHtml(object str)
        {
            string rtn = str + "";
            rtn = rtn.Replace("  ", "&nbsp;&nbsp;");
            rtn = rtn.Replace("\r\n", "<br/>");
            rtn = rtn.Replace("\n", "<br/>");
            return rtn;
        }
        public static string UnQuot(string str)
        {
            if (str + "" == "") return "";
            string rtn = str;
            if (!rtn.StartsWith("'") || !rtn.EndsWith("'"))
            {
                return rtn;
            }
            else
            {
                rtn = rtn.Replace("','", ",");
                rtn = rtn.Substring(1);
                rtn = rtn.Substring(0, rtn.Length - 1);
            }
            return rtn;
        }
        public static string Quot(List<string> str)
        {
            return Quot(String.Join(",", str.ToArray()), false);
        }
        public static string Quot(string str)
        {
            return Quot(str, false);
        }
        public static string Quot(string str, bool both)
        {
            if (str + "" == "") return "''";
            string rtn = str;
            if (!rtn.StartsWith("'") || !rtn.EndsWith("'"))
            {
                if (!both) rtn = rtn.Replace(",", "','");
                rtn = "'" + rtn + "'";
            }
            else
            {
                return rtn;
            }
            return rtn;
        }
        public static string MD5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile((str + "").Trim(), "md5");
        }
        public static string sha1(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile((str + "").Trim(), "sha1");
        }
        /// <summary>
        /// 对字符串进行编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeBase64(string str)
        {
            if (str == "") return "";
            string returnValue = "";
            returnValue = Convert.ToBase64String(Encoding.Default.GetBytes(str));
            return returnValue;
        }

        /// <summary>
        /// 对编码的字符串进行解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecodeBase64(string str)
        {
            if (str == "") return "";
            string returnValue = "";
            returnValue = Encoding.Default.GetString(Convert.FromBase64String(str));
            return returnValue;
        }

        public static string EnStock(string stockCode)
        {
            if (string.IsNullOrEmpty(stockCode)) return stockCode;
            string _C = stockCode;
            if (_C.Length == 6 && !_C.StartsWith("6"))
            {
                _C = "1" + _C;
            }else if (_C.Length == 7 && _C.StartsWith("06"))
            {
                _C = _C.Substring(1);
            }
            else if (_C.StartsWith("000"))
            {
                _C = Convert.ToInt32(_C).ToString();
            }
            return _C;
        }
        public static string DeStock(string stockCode)
        {
            if (string.IsNullOrEmpty(stockCode)) return stockCode;
            return stockCode.Substring(stockCode.Length - 6);
        }
        /// <summary>
        /// 查询会话用户类型
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public static short GetSessionUserType(uint SessionId)
        {
            short UserType = (short)((SessionId & 0x0000000c) >> 2);

            if (UserType < USER_TYPE.UT_OVERTIME_USER || UserType > USER_TYPE.UT_ANONYMITY_USER)
            {
                UserType = USER_TYPE.UT_OVERTIME_USER;
            }
            //0为超时，超时则给1（付费未到期）
            if (UserType == 0) UserType = 1;
            return UserType;
        }
        /// <summary>
        /// 查询会话服务类型
        /// </summary>
        /// <param name="SessionId"></param>
        /// <returns></returns>
        public static short GetSessionServiceType(uint SessionId)
        {
            short ServiceType = (short)(SessionId & 0x00000003);

            return ServiceType;
        }
        #region
        public static string Encode(string s,int key=17)
        {
            string rtn = "";
            string str = "3"+s;
            for (int i = 0; i < str.Length; i++)
            {
                rtn = rtn + (char)(str[i] + key);
            }
            return Uri.EscapeUriString(rtn);
        }

        public static string Decode(string s, int key=17)
        {
            string rtn = "";
            string str = Uri.UnescapeDataString(s);
            for (int i = 0; i < str.Length; i++)
            {
                rtn = rtn + (char)(str[i] - key);
            }
            return rtn.Remove(0,1);
        }
        #endregion


    }
}
