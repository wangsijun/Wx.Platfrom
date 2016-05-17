using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Classic()
        {
            string useragent = Request.UserAgent;
            if (!string.IsNullOrWhiteSpace(useragent))
            {
                Loger.Debug(useragent);
                if (Regex.IsMatch(useragent, "iPhone", RegexOptions.IgnoreCase))
                {
                    return Redirect("http://wap2.emoney.cn/iphone.html");
                }
            }
            return Redirect("http://wap2.emoney.cn/android.html");
        }

    }
}
