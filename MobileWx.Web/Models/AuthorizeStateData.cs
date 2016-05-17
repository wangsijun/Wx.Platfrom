using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Web.Models
{
    /// <summary>
    /// 验证关注者时使用的数据处理对象
    /// </summary>
    public class AuthorizeStateData
    {
        /// <summary>
        /// 有结果后的跳转处理地址
        /// </summary>
        public string redirect_uri { get; set; }

        /// <summary>
        /// 关注者的openid
        /// </summary>
        public string openid { get; set; }

        public string errorcode { get; set; }
    }
}