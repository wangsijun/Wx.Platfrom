using MobileWx.Bll;
using MobileWx.Model;
using Sys.Controller;
using Sys.Spring;
using Sys.SysCache;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class WxController : SysController
    {

        public static BllWxResponse bllWxResponse = (BllWxResponse)SysSpring.GetByName("BllWxResponse");
        public static BllDphq bllDphq = (BllDphq)SysSpring.GetByName("BllDphq");
        public static BllGgzd bllGgzd = (BllGgzd)SysSpring.GetByName("BllGgzd");
        public static BllYjcg bllYjcg = (BllYjcg)SysSpring.GetByName("BllYjcg");
        public static MCacheClient sdstockscoreex = (MCacheClient)SysSpring.GetByName("sdstockscoreex");//评级
        public static MCacheClient pgfirst_y = (MCacheClient)SysSpring.GetByName("pgfirst_y");//综述
        public static MCacheClient sddquotes = (MCacheClient)SysSpring.GetByName("sddquotes");//行情
        public static string SiteUrl = ConfigurationManager.AppSettings["SiteUrl"];

        public ContentResult Index(string signature, string timestamp, string nonce, string echostr)
        {
            DateTime startTime = DateTime.Now;
            Loger.Debug(string.Format("signature={0},timestamp={1},nonce={2},echostr={3}", signature, timestamp, nonce, echostr));
            List<string> lst = new List<string>() { wx_token, timestamp ?? "", nonce ?? "" };
            string[] arr = (from q in lst orderby q.ToLower() select q).ToArray();
            Loger.Debug("signature=" + StringUtility.sha1(string.Join("", arr)));

            if (Request.HttpMethod == "POST")
            {
                try
                {
                    byte[] b = new byte[Request.InputStream.Length];
                    Request.InputStream.Read(b, 0, b.Length);
                    string s = Encoding.UTF8.GetString(b);
                    Loger.Debug(s);
                    WxRequest req = XmlUtility.DeSerialize<WxRequest>(s);
                    //Loger.Debug(XmlUtility.Serialize(GetResponse(req)));
                    return Str(XmlUtility.Serialize(GetResponse(req)));
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
            Loger.Debug("响应时间：" + (DateTime.Now - startTime).TotalMilliseconds.ToString());
            return Str(echostr);
        }
        public WxResponse GetResponse(WxRequest req)
        {
            WxResponse rtn = new WxResponse(req.ToUserName, req.FromUserName);
            //if (req.FromUserName != "oyfqejjY-odPA645FE1-DunFpKGs") return rtn;
            switch (req.MsgType)
            {
                case ModelWx.MsgType_event:
                    if (req.Event == ModelWx.Event_subscribe)//订阅
                    {

                        Message welcome = Bll.BllMessage.Get().GetLatestMessage(MessageTypeEnum.Welcome);
                        if (welcome != null)
                        {
                            if (!string.IsNullOrWhiteSpace(welcome.PicName))
                            {
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                WxArticle article = new WxArticle()
                                {
                                    Description = welcome.Summary,
                                    Title = welcome.Title,
                                    Url = SiteUrl + "/News/Message/" + welcome.Id,
                                    PicUrl = SiteUrl + "/Content/docimage/" + GetThumbImageName(welcome.PicName, 360, 200)
                                };
                                rtn.Articles.Add(article);
                            }
                            else
                            {
                                rtn.Content = welcome.Summary;
                            }
                        }
                        else
                        {
                            rtn.Content = bllWxResponse.subscribeMessage;
                        }
                        try
                        {
                            //只保存openid
                            BllSubscribeUser.Get().SaveUser(new WxSubscribeUser() { openid = req.FromUserName, subscribetime = DateTime.Now });

                            //Loger.Debug("subscribe:" + req.FromUserName);

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
                    else if (req.Event == ModelWx.Event_unsubscribe)//取消订阅
                    {
                        try
                        {
                            BllSubscribeUser.Get().DeleteUser(req.FromUserName);
                            //Loger.Debug("unsubscribe:" + req.FromUserName);
                        }
                        catch (Exception err)
                        {
                            Loger.Error(err);
                        }
                    }
                    else if (req.Event == ModelWx.Event_CLICK)//自定义菜单点击
                    {
                        //req.EventKey  //根据点击的key值处理相关逻辑
                        rtn.Content = "自定义菜单(" + req.EventKey + ")功能正在开发中，敬请期待...";
                        switch (req.EventKey)
                        {
                            case ModelWx.MenuKeys_zxyw:
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = FileUtility.ReadJson<List<WxArticle>>(StringUtility.AppPath + @"\Json\zxyw.json");
                                break;
                            case ModelWx.MenuKeys_dphq:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = FileUtility.ReadJson<List<WxArticle>>(StringUtility.AppPath + @"\Json\daShi.json");
                                break;
                            case ModelWx.MenuKeys_ggcx:
                                bllGgzd.SetResponse(rtn, req.EventKey);
                                break;
                            case ModelWx.MenuKeys_mmcx:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "发短信“A”至106909990909 ",
                                    Title = "查询密码",
                                    Url = SiteUrl + "/Ywcx/mscx?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/cxmm.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;
                            case ModelWx.MenuKeys_dqr:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "查看软件使用到期日",
                                    Title = "到期日查询",
                                    Url = SiteUrl + "/Ywcx/dqr?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/dqr.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;
                            case ModelWx.MenuKeys_jfdh:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "积分查询和兑换",
                                    Title = "积分服务",
                                    Url = SiteUrl + "/Ywcx/jfdh?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/jffw.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;

                            case ModelWx.MenuKeys_rjxz:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "手机登录   wap.emoney.cn",
                                    Title = "软件下载",
                                    Url = "http://wap2.emoney.cn/iphone.html?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/rjxz.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;
                            case ModelWx.MenuKeys_zxgm:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "软件特色:判大势、选好股、稳操作最新优惠活动:买一年送3个月",
                                    Title = "在线购买",
                                    Url = SiteUrl + "/Ywcx/recharge?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/zxgm.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;



                            case ModelWx.MenuKeys_gnjs:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "因股票操作者而生，11年积累上交所L2最大用户群。开创了投资工具智能化、服务化先河。",
                                    Title = "迄今为止，最专业的移动证券软件",
                                    Url = SiteUrl + "/html/gnjs_2_1.html?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/gnjs.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;

                            case ModelWx.MenuKeys_szjq:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);   
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "累计益盟研究团队多年炒股经验，而练就的经典战法，结合软件功能操作，简单易学，有效。",
                                    Title = "实战技巧:经典战法应对万变行情",
                                    Url = SiteUrl + "/html/szjq_1_1.html?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/szjq.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;

                            case ModelWx.MenuKeys_tjhy:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "推荐的好友越多，送的积分越多",
                                    Title = "好软件要分享，现在就去告诉Ta",
                                    Url = SiteUrl + "/Ywcx/tjhy2?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/tjhy.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;

                            case ModelWx.MenuKeys_yjfk:
                                //bllDphq.SetDphqResponse(rtn,sddquotes);
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "点击进入填写意见反馈内容。",
                                    Title = "意见反馈",
                                    Url = SiteUrl + "/Ywcx/yjfk2?t=" + DateTime.Now.ToString("yyMMddHHmmss"),
                                    PicUrl = SiteUrl + "/Css/def/image/yjfk.png?t=" + DateTime.Now.ToString("yyMMddHHmmss")
                                });
                                break;
                            case ModelWx.MenuKeys_ggzd:
                                bllGgzd.SetResponse(rtn, req.EventKey);
                                break;
                            case ModelWx.MenuKeys_ywcx:
                                //rtn.Content = "<a href=\"" + SiteUrl + "/ywcx\">点击进入业务查询...</a>";
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = new List<WxArticle>();
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "业务查询",
                                    Title = "业务查询",
                                    Url = SiteUrl + "/ywcx",
                                    PicUrl = SiteUrl + "/css/def/images/ywcx.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "密码查询",
                                    Title = "密码查询 : 直接发送短信查询",
                                    Url = SiteUrl + "/Ywcx/mscx"
                                    ,
                                    PicUrl = SiteUrl + "/css/def/images/01.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "到期日查询",
                                    Title = "到期日查询 : 查看软件到期日",
                                    Url = SiteUrl + "/Ywcx/dqr",
                                    PicUrl = SiteUrl + "/css/def/images/02.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "积分服务",
                                    Title = "积分服务 : 积分查询和兑换",
                                    Url = SiteUrl + "/Ywcx/jfdh",
                                    PicUrl = SiteUrl + "/css/def/images/03.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "购买充值",
                                    Title = "购买充值 : 开通特色功能",
                                    Url = SiteUrl + "/Ywcx/recharge",
                                    PicUrl = SiteUrl + "/css/def/images/04.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "我要优惠",
                                    Title = "我要优惠 : 买1年送3个月",
                                    Url = SiteUrl + "/Ywcx/wyyh",
                                    PicUrl = SiteUrl + "/css/def/images/05.png"
                                });
                                rtn.Articles.Add(new WxArticle()
                                {
                                    Description = "软件下载",
                                    Title = "软件下载\n手机登录wap.emoney.cn",
                                    Url = "http://wap.emoney.cn",
                                    PicUrl = SiteUrl + "/css/def/images/06.png"
                                });
                                break;
                            case ModelWx.MenuKeys_yjcg:
                                bllYjcg.SetResponse(rtn, req.EventKey);
                                break;
                            case ModelWx.MenuKeys_zskf:
                                rtn.MsgType = ModelWx.MsgType_transfer_customer_service;
                                break;
                            case ModelWx.MenuKeys_zqrz:
                                {
                                    try
                                    {
                                        Message msg = Bll.BllMessage.Get().GetLatestMessage(MessageTypeEnum.Zqrz);
                                        if (msg != null)
                                        {
                                            if (!string.IsNullOrWhiteSpace(msg.PicName))
                                            {
                                                rtn.MsgType = ModelWx.MsgType_news;
                                                rtn.Articles = new List<WxArticle>();
                                                WxArticle article = new WxArticle()
                                                {
                                                    Description = msg.Summary,
                                                    Title = msg.Title,
                                                    Url = SiteUrl + "/News/Message/" + msg.Id,
                                                    PicUrl = SiteUrl + "/Content/docimage/" + GetThumbImageName(msg.PicName, 360, 200)
                                                };
                                                rtn.Articles.Add(article);
                                            }
                                            else
                                            {
                                                rtn.Content = msg.Summary;
                                            }
                                        }
                                    }
                                    catch (Exception err)
                                    {
                                        Loger.Error(err);
                                    }
                                }
                                break;
                            case ModelWx.MenuKeys_ygrzf:
                                {
                                    try
                                    {
                                        rtn.MsgType = ModelWx.MsgType_news;
                                        rtn.Articles = GetArticles(MessageTypeEnum.Ygrzf);
                                    }
                                    catch (Exception err)
                                    {
                                        Loger.Error(err);
                                    }
                                }
                                break;
                            case ModelWx.MenuKeys_cjybhb:
                                {
                                    try
                                    {
                                        rtn.MsgType = ModelWx.MsgType_news;
                                        rtn.Articles = GetArticles(MessageTypeEnum.Cjybhb);
                                    }
                                    catch (Exception err)
                                    {
                                        Loger.Error(err);
                                    }
                                }
                                break;
                            case ModelWx.MenuKeys_jlcjgng:
                                {
                                    try
                                    {
                                        rtn.MsgType = ModelWx.MsgType_news;
                                        rtn.Articles = GetArticles(MessageTypeEnum.Jlcjgng);
                                    }
                                    catch (Exception err)
                                    {
                                        Loger.Error(err);
                                    }
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
                            string s = req.Content.Replace(" ", "");
                            if (BllBase.stocks.ContainsKey(s))
                            {
                                bllGgzd.SetGeGuResponse(rtn, BllBase.stocks[s], sdstockscoreex, pgfirst_y, sddquotes);
                            }
                            //else if (s == "大盘" || s == "大盘走势" || s == "大盘行情")
                            //{
                            //    bllDphq.SetDphqResponse(rtn, sddquotes);
                            //}
                            else if (s == "大势" || s == "大盘" || s == "大盘走势" || s == "大盘行情")
                            {
                                rtn.MsgType = ModelWx.MsgType_news;
                                rtn.Articles = FileUtility.ReadJson<List<WxArticle>>(StringUtility.AppPath + @"\Json\daShi.json");
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

        private string GetThumbImageName(string filename, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(filename) || filename.IndexOf('.') == -1)
            {
                return filename;
            }
            filename = filename.Substring(0, filename.LastIndexOf('.'));
            return string.Format("{0}_{1}_{2}.jpg", filename, width, height);
        }

        private List<WxArticle> GetArticles(MessageTypeEnum msgType)
        {
            List<WxArticle> result = new List<WxArticle>();
            List<Message> msgs = Bll.BllMessage.Get().GetLatestMessages(msgType, 10);
            if (msgs != null && msgs.Count > 0)
            {
                int index = 0;
                foreach (var msg in msgs)
                {
                    if (!string.IsNullOrWhiteSpace(msg.PicName))
                    {
                        WxArticle article = new WxArticle()
                        {
                            Description = msg.Summary,
                            Title = msg.Title,
                            Url = SiteUrl + "/News/Message/" + msg.Id,
                            PicUrl = SiteUrl + "/Content/docimage/" + (index == 0 ? GetThumbImageName(msg.PicName, 360, 200) : GetThumbImageName(msg.PicName, 200, 200))
                        };
                        index++;
                        result.Add(article);
                    }
                }
            }
            return result;
        }

        public JsResult GetWxServerList()
        {
            SysResult rtn = new SysResult();
            rtn.msg = GetContentByUrl("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + Bll.BllWxBase.Get().GetAccessToken("EMONEY"), Encoding.UTF8);
            return Js(rtn);
        }

        public ActionResult TestWelcome()
        {
            ResultData rtn = new ResultData();
            try
            {
                Message welcome = Bll.BllMessage.Get().GetLatestMessage(MessageTypeEnum.Welcome);
                if (welcome != null)
                {
                    if (!string.IsNullOrWhiteSpace(welcome.PicName))
                    {
                        WxArticle article = new WxArticle()
                        {
                            Description = welcome.Summary,
                            Title = welcome.Title,
                            Url = SiteUrl,
                            PicUrl = SiteUrl + "/Content/docimage/" + welcome.PicName
                        };
                        rtn.Data = new List<WxArticle>() { article };
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
    }
}
