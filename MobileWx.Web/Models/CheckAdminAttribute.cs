using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Models
{
    public class CheckAdminAttribute : ActionFilterAttribute
    {

        public const string ADMIN_COOKIE = "wxadmin";
        public CheckAdminAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request.Cookies[ADMIN_COOKIE] == null)
            {
                filterContext.Result = new RedirectResult("~/Admin/Login?ReturnUrl=" + HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.RawUrl));
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }

    }
}