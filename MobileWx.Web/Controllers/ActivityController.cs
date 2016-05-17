using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Msdy(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                Model.WxAuthorizeAccessToken token = Bll.BllSubscribeUser.Get().GetAccessTokenByAuthorizeCode(code);
                if (token != null)
                {
                    string openid = token.openid;

                    return Content(openid);
                }
            }
            return RedirectToAction("WXError", "User");
        }

    }
}
