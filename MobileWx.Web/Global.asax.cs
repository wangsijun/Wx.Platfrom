using MobileWx.Bll;
using MobileWx.Model;
using Sys.Spring; 
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MobileWx.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        //public static MCacheClient sdstockscoreex = (MCacheClient)SysSpring.GetByName("sdstockscoreex");
        //public static MCacheClient pgfirst_y = (MCacheClient)SysSpring.GetByName("pgfirst_y");
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        { 
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                 new { controller = "Ywcx", action = "Index", id = "" },
                 null
                );

        }
        System.Threading.Timer t1 = null;

        protected void Application_Error(object sender, EventArgs e)
        {
            Loger.Error(Server.GetLastError());
            Server.ClearError();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Loger.Debug("Application_Start");
            //定时取股票
            BllBase.GetStocks(); 
            t1 = new System.Threading.Timer((d) =>
            {
                BllBase.GetStocks();
            }, null, 60 * 1000 * 60 * 24, 60 * 1000 * 60 * 24);
        }
    }
}