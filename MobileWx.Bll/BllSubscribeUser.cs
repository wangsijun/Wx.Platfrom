using MobileWx.Dal;
using MobileWx.Model;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MobileWx.Bll
{
    public class BllSubscribeUser : BllWxBase
    {
        private static BllSubscribeUser _me;

        public BllSubscribeUser(string responesId= "BllWxResponse") : base(responesId)
        {

        }

        public new static BllSubscribeUser Get()
        {
            if (_me == null)
                _me = new BllSubscribeUser();
            return _me;
        }

        private readonly static string URL_CODE_TO_ACCESS_TOKEN =
            "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";

        /// <summary>
        /// 从微信服务器取数据，新建或更新用户信息
        /// </summary>
        /// <param name="openid"></param>
        public void UpdateUser(string openid)
        {
            WxSubscribeUser user = DalSubscribUser.Get().GetSubscribeUser(openid);
            if (user == null || string.IsNullOrEmpty(user.nickname))
            {
                string apiurl = string.Format("{0}/user/info?access_token={1}&openid={2}", API_BASE, GetAccessToken("EMONEY"), openid);
                WxSubscribeUser wsu = JsonUtility.DeserializeByNewton<WxSubscribeUser>(GetContentByUrl(apiurl, Encoding.UTF8));
                if (wsu != null && wsu.errcode == null)
                {
                    DalSubscribUser.Get().Save(wsu);
                }
            }

        }

        /// <summary>
        /// 网页授权时的第二步骤，用户同意授权后的code获取access_token
        /// </summary>
        /// <param name="code"></param>
        public WxAuthorizeAccessToken GetAccessTokenByAuthorizeCode(string code)
        {
            //请求url
            string url = string.Format(URL_CODE_TO_ACCESS_TOKEN,
                bllWxResponse.appid, bllWxResponse.secret, code);

            try
            {
                string result = GetContentByUrl(url, Encoding.UTF8);

                WxAuthorizeAccessToken tokeninfo = JsonUtility.DeserializeByNewton<WxAuthorizeAccessToken>(result);

                if (tokeninfo != null)
                {
                    return tokeninfo;
                }

            }
            catch (Exception ex) { }

            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wxSubscribeUser"></param>
        public void SaveUser(WxSubscribeUser wxSubscribeUser)
        {
            DalSubscribUser.Get().Save(wxSubscribeUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openid"></param>
        public void DeleteUser(string openid)
        {
            DalSubscribUser.Get().Delete(openid);
        }

        /// <summary>
        /// 绑定加强版用户到微信帐号
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="bindid"></param>
        /// <returns></returns>
        public bool BindProUser(string openid, long bindid)
        {
            try
            {
                OperResult res = DalSubscribUser.Get().SubscribeUserBindProUser(openid, bindid);

                return res.Status == 0;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        /// <summary>
        /// 检验绑定code是否有效
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckBindCode(string code, out long bindid)
        {
            try
            {
                bindid = GetProBindidByCode(code);

                if (bindid > 0)
                {
                    OperResult res = DalSubscribUser.Get().CheckProUserBindid(bindid);
                    return res.Status == 0;
                }
            }
            catch (Exception ex)
            {

            }

            bindid = 0;
            return false;
        }

        #region Private
        private long GetProBindidByCode(string code)
        {
            long bindid = 0;
            if (!string.IsNullOrEmpty(code)
                && code.StartsWith("em", StringComparison.OrdinalIgnoreCase))
            {
                string idstr = code.Substring(2, code.Length - 2);

                if (long.TryParse(idstr, out bindid))
                {
                    return bindid;
                }
            }

            return bindid;
        }

        #endregion
    }
}
