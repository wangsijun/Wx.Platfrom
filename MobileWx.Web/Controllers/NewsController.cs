using MobileWx.Bll;
using MobileWx.Model;
using Sys.Controller;
using Sys.Spring;
using Sys.SysCache;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class NewsController : SysController
    {
        //
        // GET: /News/
        public static MCacheClient zlbb = (MCacheClient)SysSpring.GetByName("zlbb");
        public ActionResult Index()
        {
            return View();
        }
        //资金播报
        public ActionResult ZJBB()
        {
            ModelNewsTab obj = BllNewsTab.Get().getById(StringUtility.ToInt32(Request["id"]));
            if (obj != null)
            {
                try
                {
                    ViewData["zlbb"] = zlbb.Get(string.Format("EMONEY_SDD_WX_ZJJL_{0}", obj.createDate.Value.ToString("HH")));//WX_ZJJL_%d
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                    ViewData["zlbb"] = zlbb.Get(string.Format("EMONEY_SDD_WX_ZJJL_{0}", obj.createDate.Value.ToString("HH")));//WX_ZJJL_%d
                }
                if (ViewData["zlbb"] == null || ViewData["zlbb"].ToString() == "")
                {
                    ViewData["zlbb"] = "[]";
                }
            }
            return View(obj ?? new ModelNewsTab());
        }
        //领涨板块和个股点评
        public ActionResult GGDP()
        {
            ModelNewsTab obj = BllNewsTab.Get().getById(StringUtility.ToInt32(Request["id"]));
            return View(obj ?? new ModelNewsTab());
        }

        public ActionResult Message(int id)
        {
            Message msg = Bll.BllMessage.Get().GetMessage(id);
            if (msg != null)
            {
                return View(msg);
            }
            return HttpNotFound();
        }
    }
}
