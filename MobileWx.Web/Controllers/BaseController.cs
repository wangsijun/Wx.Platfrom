using MobileWx.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class BaseController : Controller
    {
        public JSONResult JSON(object obj)
        {
            JSONResult result = new JSONResult();
            result.Data = obj;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return result;
        }

        public JSONResult JSON(object obj, params string[] ignores)
        {
            JSONResult result = new JSONResult();
            result.Data = obj;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.ignoreproperties = ignores;

            return result;
        }

        public JSONResult JSON<T>(IList<T> list, int resstauts, DateTime? time = null)
        {
            var result = new
            {
                updatetime = time ?? DateTime.Now,
                status = resstauts,
                items = list
            };

            return JSON(result);
        }

        public JSONResult JSON(int status, string message = "", DateTime? time = null)
        {
            var result = new
            {
                updatetime = time ?? DateTime.Now,
                status = status
            };

            return JSON(result);
        }


        public ActionResult Error()
        {
            return RedirectToAction("error", "system", null);
        }
    }
}