using MobileWx.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWx.Bll
{
    public class BllWxResponse:BllBase
    {
        /// <summary>
        /// 订阅时的消息
        /// </summary>
         [JsonIgnore]
        public string subscribeMessage
        {
            get;
            set;
        }
         [JsonIgnore]
        public string appid
        {
            get;
            set;
        }
         [JsonIgnore]
        public string secret
        {
            get;
            set;
        }
        /// <summary>
        /// 取得的token
        /// </summary>
        public string access_token
        {
            get;
            set;
        }
        public string errcode
        {
            get;
            set;
        }
        public string errmsg
        {
            get;
            set;
        }
        /// <summary>
        /// 取token的地址https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}
        /// </summary>
        [JsonIgnore]
        public string getTokenUrl
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            }
            set { }
        }
        /// <summary>
        /// 创建菜单的地址https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}
        /// </summary>
         [JsonIgnore]
        public string createMenuUrl
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}";
            }
            set { }
        }
        /// <summary>
        /// 取菜单的地址https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}
        /// </summary>
         [JsonIgnore]
        public string getMenuUrl
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}";
            }
            set { }
        }
        /// <summary>
        /// 删除菜单的地址https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}
        /// </summary>
         [JsonIgnore]
        public string delMenuUrl
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}";
            }
            set { }
        }
        public List<WxMenuItem> button
        {
            get;
            set;
        }
    }
}
