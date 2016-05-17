using MobileWx.Bll;
using MobileWx.Model;
using MobileWx.Web.Models;
using MobileWx.Web.StockSynServiceMob;
using Newtonsoft.Json;
using Sys.Controller;
using Sys.Spring;
using Sys.SysCache;
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
        public static BllDphq bllDphq = (BllDphq)SysSpring.GetByName("BllDphq");
        public static BllWxResponse bllWxResponse = (BllWxResponse)SysSpring.GetByName("BllPlatformWxResponse");
        public static BllYjcg bllYjcg = (BllYjcg)SysSpring.GetByName("BllYjcg");
        private static MCacheClient pgfirst_y = (MCacheClient)SysSpring.GetByName("pgfirst_y");
        private static MCacheClient sdstockscoreex = (MCacheClient)SysSpring.GetByName("sdstockscoreex"); //评级
                                                                                                          //综述

        private static string url =
            "http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key=PRO&url=http://mt.emoney.cn/html/weixin1/code/bind.html";

        private readonly BllProWx _bllProWx = new BllProWx();
        private readonly RCacheClient _cache = new RCacheClient();
        public static BllGgzd BllGgzd { get; set; } = (BllGgzd)SysSpring.GetByName("BllGgzd");
        private static MCacheClient Sddquotes { get; set; } = (MCacheClient)SysSpring.GetByName("sddquotes");
        private static string SiteUrl { get; set; } = ConfigurationManager.AppSettings["SiteUrl"];
        private static string TokeKey { get; } = "PRO";

        private static string Welcomes { get; } = "亲，您还没有绑定操盘手加强版账号，请先绑定账号  " +
                                                  "\n\n" +
                                                  $"　　　　<a href='http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key=PRO&url=http://mt.emoney.cn/html/weixin1/code/bind.html'>点击这里，立即绑定</a>"
            ;

        private const string zxgMessage = "您还没有添加自选哦，快到软件内添加第一只股票吧~";
        private const string WxBindKey = "Customer.WxCorrelationInfo";
        /// <summary>
        /// 获取自选股行情
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public JsResult GetZxgList(string openId)
        {
            if (string.IsNullOrEmpty(openId))
                return Js(new { status = -1, message = "参数不能为空" });
            try
            {
                var correla = RedisClientService.Instance.JsonGet<WxCorrelationInfo>(WxBindKey, openId); //自选股id
                var resultList = GetStockPrice(correla.ZxgId.Value);
                return Js(new
                {
                    status = 0,
                    message = resultList.Count > 0 ? "只提供沪深A股和创业板数据,其他自选请在软件内查看" : "",
                    data = resultList

                });
            }
            catch (Exception e)
            {
                return Js(new { status = -1, message = e.Message });
            }
        }

        public JsResult ChinaIndexMs(string stock)
        {
            if (string.IsNullOrEmpty(stock))
                return Js(new { status = -1, message = "参数不能为空" });

            var chinaIndexList = BllGgzd.GgcxList(stock);
            return Js(new
            {
                status = 0,
                message = "",
                data = new { likeUrl = chinaIndexList.Count != 0 ? chinaIndexList[0].LinkUrl : "" }

            });
        }

        public WxResponse GetResponse(WxRequest req)
        {
            var rtn = new WxResponse(req.ToUserName, req.FromUserName);

            //if (req.FromUserName != "oyfqejjY-odPA645FE1-DunFpKGs") return rtn;
            switch (req.MsgType)
            {
                case ModelWx.MsgType_event:
                    if (req.Event == ModelWx.Event_subscribe) //订阅
                    {
                        rtn.Content = Welcomes; 
                        try
                        {
                            //只保存openid
                            BllSubscribeUser.Get().SaveUser(new WxSubscribeUser { openid = req.FromUserName, subscribetime = DateTime.Now });

                            new Thread(openid =>
                            {
                                //从微信服务器取数据，保存用户详细信息
                                BllSubscribeUser.Get().UpdateUser(openid as string);
                            }).Start(req.FromUserName);
                        }
                        catch (Exception err)
                        {
                            Loger.Error(err);
                        }
                    }
                    else if (req.Event == ModelWx.Event_unsubscribe) //取消订阅
                    {
                        try
                        {
                            RedisClientService.Instance.HDel(WxBindKey, req.FromUserName);

                            var status = _bllProWx.RelieveBind("", "", req.FromUserName, false); //解绑微信公众号
                            if (status == 0)
                            {
                                //var result = RedisClientService.Instance.JsonGet<WxCorrelationInfo>(WxBindKey, req.FromUserName);
                                //result.IsBind = "0";
                                //RedisClientService.Instance.JsonSet(WxBindKey, req.FromUserName, result);

                                Loger.Info($"#{req.FromUserName}# 解除绑定");
                            }
                            BllSubscribeUser.Get().DeleteUser(req.FromUserName);
                        }
                        catch (Exception err)
                        {
                            Loger.Error(err);
                        }
                    }
                    else if (req.Event == ModelWx.Event_CLICK) //自定义菜单点击
                    {
                        //req.EventKey  //根据点击的key值处理相关逻辑
                        rtn.Content = "自定义菜单(" + req.EventKey + ")功能正在开发中，敬请期待...";

                        var wxList = FileUtility.ReadJson<List<WxArticle>>(@"D:\web\wx.mt.emoney.cn\web\Json\daShi.json");
                        foreach (var item in wxList)
                        {
                            item.Url =
                                $"http://mt.emoney.cn/wx/news/pro{item.Url.Remove(0, item.Url.LastIndexOf("/", StringComparison.Ordinal))}?loadcss=weixin1.1";
                            item.PicUrl = "";
                        }
                        var zxywList =
                            FileUtility.ReadJson<List<WxArticle>>(@"D:\web\wx.mt.emoney.cn\web\Json\zxyw.json");
                        foreach (var item in zxywList)
                        {
                            if (Regex.IsMatch(item.Url, @"news/\d+\.html", RegexOptions.IgnoreCase))
                            {
                                item.Url =
                                    $"http://mt.emoney.cn/wx/news/pro{item.Url.Remove(0, item.Url.LastIndexOf("/", StringComparison.Ordinal))}?loadcss=weixin1.1";
                            }
                            else
                            {
                                item.Url = $"{item.Url}&loadcss=weixin1.1";
                            }
                        }
                        //获取当前openid是否绑定加强版账号
                        var correlationInfo = _cache.JsonGet<WxCorrelationInfo>(WxBindKey, req.FromUserName); //oDzmZt8c9w8KiEbCDs1BxGFv5F1g
                        string getUserStatus = correlationInfo != null ? correlationInfo.IsBind : "0";
                        string promptMessage = "亲，您还没有绑定操盘手加强版账号，请先绑定账号  ";
                        switch (req.EventKey)
                        {
                            case ModelWx.MenuKeys_zxyw:
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = zxywList;
                                break;

                            case ModelWx.MenuKeys_dphq:
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = wxList;
                                break;

                            case ModelWx.MenuKeys_ggcx:
                                BllGgzd.SetResponse(rtn, req.EventKey);
                                break;
                            case ModelWx.MenuKeys_djfw: //等级服务
                                /*
                                测试openid: oDzmZt3b9f6PGDIMl7m0mm0Ap9x0
                                判断用户是否已经绑定微信公众账号
                                */
                                if (getUserStatus == "1")
                                {
                                    var info = _bllProWx.GetUserInfo(req.FromUserName);
                                    rtn.Content =
                                        $"亲，您绑定的账号为{info.UserName}，您当前的等级为{info.UserLevel}级，拥有金币数{info.UserPoint}";
                                }
                                else
                                {
                                    rtn.Content = $"{promptMessage}\n\n" +
                                                  $"　　　　<a href='http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key={TokeKey}&url=http://mt.emoney.cn/html/weixin1/code/binddjfw.html'>点击这里，立即绑定</a>";
                                }

                                break;

                            case ModelWx.MenuKeys_dhjl: //兑换记录
                                Loger.Info($"FromUserName:{req.FromUserName}|ToUserName:{req.ToUserName}|URL:{req.Url}|Title:{req.Title}|EventKey:{req.EventKey}|Description:{req.Description}|Content:{req.Content}|MsgType:{req.MsgType}|MsgId:{req.MsgId}|id:{req.id}");
                                if (getUserStatus == "1")
                                {
                                    rtn.Content = DhjlMessage(req.FromUserName);
                                }
                                else
                                {
                                    rtn.Content = $"{promptMessage}\n\n" +
                                                  $"　　　　<a href='http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key={TokeKey}&url=http://mt.emoney.cn/html/weixin1/code/binddhjl.html'>点击这里，立即绑定</a>";
                                }
                                break;

                            case ModelWx.MenuKeys_zxg:
                                if (getUserStatus == "1")
                                {
                                    var correla = RedisClientService.Instance.JsonGet<WxCorrelationInfo>(WxBindKey, req.FromUserName); //自选股id
                                    string zxgText = GetProWxUserZxG(correla.ZxgId);
                                    rtn.MsgType = ModelWx.MsgType_news;
                                    rtn.ArticleCount = 1;
                                    rtn.Articles = new List<WxArticle>() {
                                        new WxArticle() {
                                          Title="自选行情",
                                          Url=zxgText==zxgMessage?"http://a.app.qq.com/o/simple.jsp?pkgname=cn.emoney.pf":$"http://mt.emoney.cn/html/weixin1/code/zxg.html?openid={req.FromUserName}",
                                          Description=GetProWxUserZxG(correla.ZxgId)
                                        }};
                                }
                                else
                                {
                                    rtn.Content = $"{promptMessage}\n\n" +
                                              $"　　　　<a href='http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key={TokeKey}&url=http://mt.emoney.cn/html/weixin1/code/bindzxg.html'>点击这里，立即绑定</a>";
                                }
                                break;

                            default:
                                rtn.Content = "自定义菜单(" + req.EventKey + ")功能正在开发中，敬请期待...";
                                break;
                        }
                    }

                    break;

                case ModelWx.MsgType_text:
                    //req.Content  //根据发送的内容处理相关逻辑
                    switch (req.Content)
                    {
                        default:
                            var s = req.Content.Replace(" ", "");

                            if (BllBase.stocks.ContainsKey(s))
                            {
                                BllGgzd.SetGeGuResponse(rtn, BllBase.stocks[s], sdstockscoreex, pgfirst_y, Sddquotes);
                            }
                            else if (s == "大势" || s == "大盘" || s == "大盘走势" || s == "大盘行情")
                            {
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles =
                                    FileUtility.ReadJson<List<WxArticle>>(StringUtility.AppPath + @"\Json\daShi.json");
                            }
                            else
                            {
                                Loger.Debug(s);
                                rtn.MsgType = ModelWx.MsgType_transfer_customer_service;
                            }
                            break;
                    }
                    break;

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

        public JsResult GetWxServerList()
        {
            var rtn = new SysResult
            {
                msg =
                    GetContentByUrl(
                        "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" +
                        BllWxBase.Get().GetAccessToken(TokeKey), Encoding.UTF8)
            };
            return Js(rtn);
        }

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
            var info = bllProWx.GetUserInfo(openId);
            if (clickEvent == "djfw")//等级服务
            {
                senMessage = $"亲，您绑定的账号为{info.UserName}，您当前的等级为{info.UserLevel}级，拥有金币数{info.UserPoint}";
            }
            else if (clickEvent == "dhjl")//兑换记录
            {
                senMessage = DhjlMessage(openId);
            }
            else if (clickEvent == "zxg")//自选股
            {

                var correla = RedisClientService.Instance.JsonGet<WxCorrelationInfo>(WxBindKey, openId); //自选股id
                string zxgText = GetProWxUserZxG(correla.ZxgId);
                var articles = new List<articles>()
                {
                    new articles()
                    {
                        title = "自选股行情",
                        url =zxgText == zxgMessage
                            ? "http://a.app.qq.com/o/simple.jsp?pkgname=cn.emoney.pf"
                            : $"http://mt.emoney.cn/html/weixin1/code/zxg.html?openid={openId}",
                        description = $"亲，绑定账号\n{info.UserName}成功\n\n{zxgText}"
                    },
                };
                var zxgCustomerMessage = new CustomerMessage
                {
                    msgtype = "news",
                    touser = openId,
                    news = new { articles },

                };
                return Content(SendWxCustomerMessage(accessToken, JsonConvert.SerializeObject(zxgCustomerMessage)));
            }


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

        public ActionResult TestWelcome()
        {
            var rtn = new ResultData();
            try
            {
                var welcome = BllMessage.Get().GetLatestMessage(MessageTypeEnum.Welcome);
                if (welcome != null)
                {
                    if (!string.IsNullOrWhiteSpace(welcome.PicName))
                    {
                        var article = new WxArticle
                        {
                            Description = welcome.Summary,
                            Title = welcome.Title,
                            Url = SiteUrl,
                            PicUrl = SiteUrl + "/Content/docimage/" + welcome.PicName
                        };
                        rtn.Data = new List<WxArticle> { article };
                    }
                    else
                    {
                        rtn.Data = welcome.Summary;
                    }
                }
                else
                {
                    rtn.Status = 1;
                }
            }
            catch (Exception err)
            {
                Loger.Error(err);
                rtn.Status = -1;
                while (err.InnerException != null)
                {
                    err = err.InnerException;
                }
                rtn.Message = err.Message;
            }
            return Js(rtn);
        }

        /// <summary>
        ///兑换记录-文本消息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        private string DhjlMessage(string openId)
        {
            var exchangeResult = _bllProWx.GetUserExchangeLog(openId);
            var phoneBill = exchangeResult.Where(p => p.typename == "话费").ToList(); //话费
            var function = exchangeResult.Where(p => p.typename == "功能").ToList(); //功能
            var sb = new StringBuilder();
            if (exchangeResult.Count == 0)
            {
                sb.Append("亲，您还没有兑换过功能或话费哦，快点击详情进入加强版来兑换吧~  " +
                          "\n\n" +
                          "<a href='http://a.app.qq.com/o/simple.jsp?pkgname=cn.emoney.pf'>详细</a>");
            }
            if (function.Count > 0)
            {
                sb.AppendLine("亲，你兑换的功能如下：");
                var index = 1;
                foreach (var item in function)
                {
                    sb.AppendLine($"{index}. {item.itemname}，到期:{DateTime.Parse(item.date_weixin).ToString("yyyy-MM-dd")} ");
                    index++;
                }
            }

            if (phoneBill.Count > 0)
            {
                sb.AppendLine("\n");
                sb.AppendLine("您兑换的话费记录如下（最近3次）：");
                var index = 1;

                foreach (var item in phoneBill.OrderByDescending(p => DateTime.Parse(p.date_weixin)))
                {
                    if (index <= 3)
                        sb.AppendLine($"{index}.{DateTime.Parse(item.date_weixin).ToString("MM月dd日")}，{item.itemname}");

                    index++;
                }
            }
            return sb.ToString();
        }
        ///  <summary>
        /// 自选股- 文本消息
        ///  </summary>
        ///  <param name="zxgId"></param>
        /// <returns></returns>
        private string GetProWxUserZxG(Int64? zxgId)
        {
            var sm = new MyStockSynServiceMobSoapClient();
            var zxgAppend = new StringBuilder();
            if (zxgId.HasValue)
            {
                var zxgRtnJson = sm.GetMyStock(zxgId.Value.ToString(), "710000000", ""); //读取自选股

                var stockCodeResult = JsonConvert.DeserializeObject<ZxgRootobject>(zxgRtnJson.ToString()); //获取自选股列表
                var stockPriceList = GetStockPrice(zxgId);
                if (stockCodeResult.Msg == "用户无自选股记录" || stockPriceList.Count == 0)
                {
                    return zxgMessage;
                }

                int index = 0;
                string title = "名称                      ";
                string price = "价格                      ";
                string zdfName = "涨跌幅";
                zxgAppend.Append($"{title}{price}{zdfName}\n");
                foreach (var stockInfo in stockPriceList)
                {
                    if (index > 10)
                        break;

                    //获取股票代码字节，根据字节判断需要减去多少个空格
                    int byteLength = Encoding.GetEncoding("gb2312").GetBytes(stockInfo.N).Length;
                    int placeholder = 0;
                    if (byteLength == 6 || byteLength == 5)
                        placeholder = 1;
                    else if (byteLength == 8)
                        placeholder = 3;

                    decimal? p = Math.Round(stockInfo.P ?? 0, 2);
                    int zdfLength = Encoding.GetEncoding("gb2312").GetBytes(p.Value.ToString(CultureInfo.InvariantCulture)).Length;
                    int zdfPlaceholder = 0;
                    if (zdfLength == 7)
                        zdfPlaceholder = 4;
                    else if (zdfLength == 4)
                        zdfPlaceholder = 1;

                    var zf = stockInfo.F == 0 ? null : stockInfo.F;
                    decimal zdf = zf ?? stockInfo.D.Value;
                    var zdfVal = zdf.ToString() == "0" ? "0.00" : zdf.ToString();
                    //if (p.Value.ToString() == "0")
                    //{
                    //}
                    string stockNameString = PadLeftEx(stockInfo.N, Encoding.GetEncoding("gb2312").GetBytes(title).Length - placeholder, ' ');
                    string priceString = PadLeftEx(p.Value.ToString() == "0" ? "停牌" : p.Value.ToString(), Encoding.GetEncoding("gb2312").GetBytes(price).Length - zdfPlaceholder, ' ');
                    string zdfString = PadLeftEx($"{zdfVal }% ", Encoding.GetEncoding("gb2312").GetBytes(zdfName).Length, ' ');

                    zxgAppend.Append($"{stockNameString}{priceString}{zdfString}\n");

                    index++;
                }
            }
            else
            {
                zxgAppend.Append("网络异常，请稍后再试！");
            }

            return zxgAppend.ToString();
        }
        /// <summary>
        /// 获取自选股列表
        /// </summary>
        /// <param name="zxgId"></param>
        /// <returns></returns>
        private List<StockPrice> GetStockPrice(Int64? zxgId)
        {
            var sm = new MyStockSynServiceMobSoapClient();
            List<StockPrice> resultPrice = new List<StockPrice>();
            var zxgRtnJson = sm.GetMyStock(zxgId.Value.ToString(), "710000000", ""); //读取自选股

            var stockCodeResult = JsonConvert.DeserializeObject<ZxgRootobject>(zxgRtnJson.ToString()); //获取自选股列表
            if (stockCodeResult.Msg == "用户无自选股记录" || stockCodeResult.Data.Length == 0)
            {
                return new List<StockPrice>();
            }

            var zxgDatum = stockCodeResult.Data.FirstOrDefault(p => p.pid == "710000000");

            string[] stockList = zxgDatum.stocks.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in stockList)
            {
                string stockCode;

                if (item.Length == 6)
                    stockCode = (item.StartsWith("6", StringComparison.Ordinal) ? $"0{item}," : $"{item},");
                else
                    stockCode = item;

                var stockInfoList = BllProWx.GetStockInfoList(stockCode);
                if (stockInfoList.Count > 0)
                {
                    var stockInfo = stockInfoList.FirstOrDefault();
                    if (stockInfo.C.StartsWith("0", StringComparison.Ordinal) ||
                        stockInfo.C.StartsWith("6", StringComparison.Ordinal) ||
                        stockInfo.C.StartsWith("3", StringComparison.Ordinal) &&
                        !stockInfo.C.StartsWith("399", StringComparison.Ordinal))
                        resultPrice.Add(stockInfo);
                }
            }

            return resultPrice;
        }

        /// <summary>
        /// 中英文字符串对齐
        /// </summary>
        /// <param name="str"></param>
        /// <param name="totalByteCount"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string PadLeftEx(string str, int totalByteCount, char c)
        {
            Encoding coding = Encoding.GetEncoding("gb2312");
            int dcount = 0;
            foreach (char ch in str.ToCharArray())
            {
                if (coding.GetByteCount(ch.ToString()) == 2)
                    dcount++;
            }
            string w = str.PadRight(totalByteCount - dcount, c);
            return w;
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