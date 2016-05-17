using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Controller
{
    public class ViewController:SysController
    {
        protected override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            //Response.AddHeader("Access-Control-Allow-Origin", "*");
            base.OnActionExecuted(filterContext);
        }
    }
}
