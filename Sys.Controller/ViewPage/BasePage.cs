using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sys.Utility;

namespace Sys.Controller
{
    public class BasePage : System.Web.Mvc.ViewPage
    {
        public string Root
        {
            get { return StringUtility.RootPath; }
        }
    }
}
