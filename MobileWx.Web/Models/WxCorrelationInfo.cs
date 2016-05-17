using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Web.Models
{
    public class WxCorrelationInfo
    {
        /// <summary>
        /// 是否绑定微信号
        /// </summary>
        public string IsBind { get; set; }
        /// <summary>
        /// 加强版账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 自选股id
        /// </summary>
        public Int64? ZxgId { get; set; }
    }
}