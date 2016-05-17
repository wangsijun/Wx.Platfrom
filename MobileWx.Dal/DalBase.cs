using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Utility;

namespace MobileWx.Dal
{
    public class DalBase
    {
        protected static string SqlConnectString = ConfigurationManager.AppSettings["SqlconnectionString"];

        /// <summary>
        /// 209活动专用库链接
        /// </summary>
        protected static string conn_activity = ConfigurationManager.ConnectionStrings["activity"].ToString();

        /// <summary>
        /// 加强版218用户库链接
        /// </summary>
        protected static string conn_platform_db = ConfigurationManager.ConnectionStrings["platform"].ToString();

        protected static string conn_platform218_db = ConfigurationManager.ConnectionStrings["platform218"].ToString();

    }


}
