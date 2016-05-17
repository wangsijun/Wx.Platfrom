using MobileWx.Bll;
using MobileWx.Model;
using MobileWx.Web.Models;
using MobileWx.Web.StockSynServiceMob;
using Newtonsoft.Json;
using Sys.Controller;
using Sys.Spring;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using MobileWx.Model.ProWx;

namespace MobileWx.Web.Controllers
{
    public class PlatformWxController : SysController
    { 
          
        private static string TokeKey { get; } = "PRO";


        public ContentResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            var startTime = DateTime.Now;
            Loger.Debug($"signature={signature},timestamp={timestamp},nonce={nonce},echostr={echostr}");
            var lst = new List<string> { wx_token, timestamp ?? "", nonce ?? "" };
            var arr = (from q in lst orderby q.ToLower() select q).ToArray();
            Loger.Debug("signature=" + StringUtility.sha1(string.Join("", arr)));

            if (Request.HttpMethod == "POST")
            {
                try
                {
                    var b = new byte[Request.InputStream.Length];
                    Request.InputStream.Read(b, 0, b.Length);
                    var s = Encoding.UTF8.GetString(b);
                    Loger.Debug(s);
                    var req = XmlUtility.DeSerialize<WxRequest>(s);

                    return Str(XmlUtility.Serialize(GetResponse(req)));
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
            Loger.Debug("响应时间：" + (DateTime.Now - startTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            return Str(echostr);
        }
        public WxResponse GetResponse(WxRequest req)
        {
            var rtn = new WxResponse(req.ToUserName, req.FromUserName);
            switch (req.MsgType)
            {
                case ModelWx.MsgType_event:
                    if (req.Event == ModelWx.Event_subscribe) //订阅
                    {
                        rtn.Content = "您好！";
                        try
                        {
                            //只保存openid
                            //BllSubscribeUser.Get().SaveUser(new WxSubscribeUser { openid = req.FromUserName, subscribetime = DateTime.Now });

                            new Thread(openid =>
                            {
                                //从微信服务器取数据，保存用户详细信息
                                //BllSubscribeUser.Get().UpdateUser(openid as string);
                            }).Start(req.FromUserName);
                        }
                        catch (Exception err)
                        {
                            Loger.Error(err);
                        }
                    }
                    else if (req.Event == ModelWx.Event_unsubscribe) //取消订阅
                    {
                        Loger.Info($"#{req.FromUserName}# 解除绑定");
                    }
                    break;
                case ModelWx.MsgType_text:
                case ModelWx.MsgType_image:
                case ModelWx.MsgType_link:
                default:
                    //将消息转发到多客服
                    rtn.MsgType = ModelWx.MsgType_transfer_customer_service;
                    break;
            }

            rtn.SetCreateTime(DateTime.Now);
            return rtn;
        }

        /// <summary>
        ///微信通过客服接口发送消息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="clickEvent"></param>
        /// <returns></returns>
        public ActionResult SendCustomerMessage(string openId, string clickEvent)
        {
            var accessToken = BllWxBase.Get().GetAccessToken(TokeKey);

            if (string.IsNullOrEmpty(openId) || string.IsNullOrEmpty(clickEvent))
            {
                return Content(new { errcode = -1, errmsg = "参数为null" }.ToString());
            }

            var bllProWx = new BllProWx();
            var senMessage = string.Empty;


            var message = new CustomerMessage
            {
                msgtype = "text",
                touser = openId,
                text = new { content = senMessage }
            };
            var jsonResult = JsonConvert.SerializeObject(message);
            var result = SendWxCustomerMessage(accessToken, jsonResult);
            return Content(result);
        }



        /// <summary>
        /// 发送客服消息
        /// </summary>
        /// <param name="accessToken">微信公众号TOKEN</param>
        /// <param name="jsonVal">发送文本消息</param>
        /// <returns></returns>
        private string SendWxCustomerMessage(string accessToken, string jsonVal)
        {
            return
                GetHttpPostResponse(
                    $"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={accessToken}", jsonVal);
        }
        /// <summary>
        ///HTTPPOST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetHttpPostResponse(string url, string json)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            //handler.Proxy = new WebProxy("http://localhost:8888", false, new string[] { }); //本地代理测试
            //handler.UseProxy = true;

            var client = new HttpClient(handler);
            var result =
                client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"))
                    .Result.Content.ReadAsStringAsync().Result;

            return result;
        }
    }
}