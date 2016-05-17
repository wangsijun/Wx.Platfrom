using MobileWx.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MobileWx.Model;
using MobileWx.Web.Models;
using Sys.Utility;
using System.Text;

namespace MobileWx.Web.Controllers
{

    public class UserController : Controller
    {
        /// <summary>
        /// 网页授权获取用户的openid后，验证关注者是否关注微信
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult WxAuth(string code = "", string state = "")
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return RedirectToAction("wxerror");
            }

            //获取
            WxAuthorizeAccessToken token = BllSubscribeUser.Get().GetAccessTokenByAuthorizeCode(code);
            if (token == null)
            {
                Loger.Error(" 取回token失败；code=" + code);
                return RedirectToAction("wxerror", new { code = token.errcode });
            }
            else if (!string.IsNullOrWhiteSpace(token.errcode))
            {
                Loger.Debug(" 取回token返回错误；errcode=" + token.errcode);
                return RedirectToAction("wxerror", new { code = token.errcode });
            }


            string openid = token.openid;
            Loger.Debug(" 取回token；" + openid);
            string url = HttpUtility.UrlDecode(state);
            StringBuilder urlredirect = new StringBuilder();
            urlredirect.Append(url);
            if (!url.Contains('?'))
            {
                urlredirect.Append("?");
            }
            else
            {
                urlredirect.Append("&");
            }
            //获取code成功后跳转到指定的链接，参数中添加openid和错误代码
            urlredirect.AppendFormat("openid={0}", openid);

            Loger.Debug("成功跳转链接：" + urlredirect.ToString());
            return Redirect(urlredirect.ToString());

        }

        public ActionResult WXError(string code = "")
        {
            return View();
        }
    }
}
