using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sys.Spring;
using Sys.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MobileWx.Bll
{
    public class BllWxBase
    {
        public BllWxResponse bllWxResponse = (BllWxResponse)SysSpring.GetByName("BllWxResponse");
        protected const string API_BASE = "https://api.weixin.qq.com/cgi-bin";
        private const string HASH_WX_BASE = "mobile_wx_base";
        private const string KEY_ACCESS_TOKEN = "access_token";
        private const string KEY_ACCESS_TOKEN_EXPIRETIME = "access_token_expiretime";
        private static BllWxBase _me;
        private string _responseId = string.Empty;

        public BllWxBase(string responseId)
        {
            _responseId = responseId;
        }

        public static BllWxBase Get(string responseId = "BllWxResponse")
        {
            if (_me == null) _me = new BllWxBase(responseId);
            return _me;
        }

        public string GetAccessToken(string key)
        {
            string url = string.Format("http://mt.emoney.cn/weixin/tencent/accesstoken?key={0}", key);

            string result = GetContentByUrl(url, Encoding.UTF8);

            var jsonResult = JsonConvert.DeserializeObject<Rootobject>(result);
            if (jsonResult.status == 0)
            {
                return jsonResult.data;
            }
            return string.Empty;
            //return new Dal.DalWx().GetAccessToken();
            //DateTime? expireTime = RedisClientService.Instance.HGet<DateTime?>(HASH_WX_BASE, KEY_ACCESS_TOKEN_EXPIRETIME);
            //string access_token = RedisClientService.Instance.HGet<string>(HASH_WX_BASE, KEY_ACCESS_TOKEN);
            //if (expireTime == null || expireTime < DateTime.Now.AddMinutes(10))
            //{
            //    try
            //    {
            //        string apiurl = string.Format("{0}/token?grant_type=client_credential&appid={1}&secret={2}",
            //            API_BASE, bllWxResponse.appid, bllWxResponse.secret);
            //        Loger.Debug("GetAccessToken apiurl=" + apiurl);

            //        JObject jobject = (JObject)JsonConvert.DeserializeObject(GetContentByUrl(apiurl, Encoding.UTF8));
            //        if (jobject != null)
            //        {
            //            if (jobject["access_token"] != null && jobject["expires_in"] != null)
            //            {
            //                access_token = jobject["access_token"].Value<string>();
            //                expireTime = DateTime.Now.AddSeconds(jobject["expires_in"].Value<int>());
            //                RedisClientService.Instance.HSet<DateTime?>(HASH_WX_BASE, KEY_ACCESS_TOKEN_EXPIRETIME, expireTime);
            //                RedisClientService.Instance.HSet<string>(HASH_WX_BASE, KEY_ACCESS_TOKEN, access_token);
            //            }
            //            else if (jobject["errcode"] != null)
            //            {
            //                Loger.Error(jobject.ToString());
            //            }
            //        }
            //    }
            //    catch (Exception err)
            //    {
            //        Loger.Error(err);
            //    }
            //}
            //return access_token;
        }

        /// <summary>
        /// 请求URL获取请求结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string GetContentByUrl(string url, Encoding encode, string postData = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            if (postData != null)
            {
                request.Method = "POST";
                Stream sw = request.GetRequestStream();
                List<byte> reqlst = new List<byte>();
                reqlst = encode.GetBytes(postData).ToList();
                foreach (byte b in reqlst)
                {
                    sw.WriteByte(b);
                }
            }

            WebResponse resp = request.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream(), encode);
            string s = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return s;
        }

        /// <summary>
        /// 获取关注者列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetSubscribeUsers(string typeName)
        {
            Loger.Debug("user/get");
            List<string> result = new List<string>();
            string nextopenid = "";
            while (nextopenid != null)
            {
                string apiurl = string.Format("{0}/user/get?access_token={1}&next_openid={2}", API_BASE, GetAccessToken(typeName), nextopenid);
                Loger.Debug("GetSubscribeUsers apiurl=" + apiurl);
                JObject jobject = (JObject)JsonConvert.DeserializeObject(GetContentByUrl(apiurl, Encoding.UTF8));
                if (jobject["total"] != null)
                {
                    nextopenid = null;
                    int total = jobject["total"].Value<int>();
                    int count = jobject["count"].Value<int>();
                    if (count > 0)
                    {
                        JArray ja = (JArray)jobject["data"]["openid"];
                        foreach (var item in ja)
                        {
                            result.Add(item.Value<string>());
                        }
                        if (jobject["next_openid"] != null && !string.IsNullOrWhiteSpace(jobject["next_openid"].Value<string>()))
                        {
                            nextopenid = jobject["next_openid"].Value<string>();
                            Loger.Debug("user/get nextopenid" + nextopenid);
                        }
                    }
                }
            }
            return result;
        }
    }

    public class Rootobject
    {
        public string data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}