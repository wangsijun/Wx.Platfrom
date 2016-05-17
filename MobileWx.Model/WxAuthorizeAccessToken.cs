﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MobileWx.Model
{
    public class WxAuthorizeAccessToken
    {
        /// <summary>
        /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同 
        /// </summary>
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒） 
        /// </summary>
        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
        /// <summary>
        /// 用户刷新access_token 
        /// </summary>
        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        /// <summary>
        /// 用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID 
        /// </summary>
        [JsonProperty("openid")]
        public string openid { get; set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔 
        /// </summary>
        [JsonProperty("scope")]
        public string scope { get; set; }

        
        public string errcode { get; set; }
    }
}
