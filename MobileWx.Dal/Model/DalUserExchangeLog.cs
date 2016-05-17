using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Dal.Model
{
    /// <summary>
    /// 用户兑换记录
    /// </summary>
    public class DalUserExchangeLog
    {
        public string itemname { get; set; }
        public string date_weixin { get; set; }
        public string typename { get; set; }
    }
}
