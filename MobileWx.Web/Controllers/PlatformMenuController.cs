using MobileWx.Bll;
using MobileWx.Model;
using Sys.Controller;
using Sys.Spring;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MobileWx.Web.Models;
using Newtonsoft.Json;

namespace MobileWx.Web.Controllers
{
    public class PlatformMenuController : SysController
    {
        private const string PASSKEY = "1qaz=[;."; //自定义菜单命令的密码
        private const string snsapi_base_format = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state=0&connect_redirect=1#wechat_redirect";
        //
        // GET: /Menu/
        public static BllWxResponse BllWxResponse = (BllWxResponse)SysSpring.GetByName("BllWxResponse");

        public static string TokeKey { get; } = "PRO";
        //public static string TokeKey { get; } = "EMTEST";

        public ActionResult Index()
        {
            return View();
        }

        public JsResult create()
        {
            SysResult rtn = new SysResult();

            string msdyurl = WxController.SiteUrl + "/Activity/Msdy";
            BllWxResponse menus = new BllWxResponse();
            menus.button = new List<WxMenuItem>();
            menus.button.Add(new WxMenuItem()
            {
                name = "行情查询",
                sub_button = new List<WxMenuItem>(){
                         new WxMenuItem(){
                            name="资讯要闻",
                            key=ModelWx.MenuKeys_zxyw,
                            type=ModelWx.MenuType_click
                        },
                        new WxMenuItem(){
                            name="大盘行情",
                             key=ModelWx.MenuKeys_dphq,
                            type=ModelWx.MenuType_click
                        },
                         new WxMenuItem(){
                            name="个股查询",
                            key=ModelWx.MenuKeys_ggcx,
                            type=ModelWx.MenuType_click
                        },
                           new WxMenuItem(){
                            name="自选股",
                            key=ModelWx.MenuKeys_zxg,
                            type=ModelWx.MenuType_click
                        }
                     }
            });
            menus.button.Add(new WxMenuItem()
            {
                //name = "社区",
                //type = ModelWx.MenuKeys_view,
                //url = "http://mp.wsq.qq.com/263753283"
                name = "我的",
                sub_button = new List<WxMenuItem>(){
                         new WxMenuItem(){
                            name="等级服务",
                            key=ModelWx.MenuKeys_djfw,
                            type=ModelWx.MenuType_click
                        },
                          new WxMenuItem(){
                            name="下载软件",
                            type=ModelWx.MenuKeys_view,
                            url="http://wap.emoney.cn/pro/"
                        },
                             new WxMenuItem(){
                            name="在线开户",
                            type=ModelWx.MenuKeys_view,
                            url="http://mt.emoney.cn/html/weixin1/code/Kulb/index.html"
                        },
                            new WxMenuItem(){
                            name="账号管理",
                            type=ModelWx.MenuKeys_view,
                            url="http://mt.emoney.cn/weixin/tencent/RedirectOpenId?key=PRO&url=http://mt.emoney.cn/html/weixin1/code/rebind.html"
                        },
                            new WxMenuItem(){
                            name="兑换记录",
                            key=ModelWx.MenuKeys_dhjl,
                            type=ModelWx.MenuType_click
                        },
                }
            });
            menus.button.Add(new WxMenuItem()
            {
                name = "活动",
                sub_button = new List<WxMenuItem>(){
                        //new WxMenuItem(){
                        //    name="全民自救，翻身解套！",
                        //    type =ModelWx.MenuKeys_view,
                        //    url="http://mt.emoney.cn/matches/cdzt/index.html"
                        //},
                        new WxMenuItem(){
                            name="庆祝益盟挂牌新三板！",
                            key=ModelWx.MenuKeys_zhgl,
                            type=ModelWx.MenuKeys_view,
                            url="http://mt.emoney.cn/matches/px/xsb/index.html"
                        }
                     }
            });

            string accessToken = BllWxBase.Get().GetAccessToken(TokeKey);
            BllWxResponse resp = JsonUtility.DeserializeByNewton<BllWxResponse>(
                GetContentByUrl(string.Format(BllWxResponse.createMenuUrl, accessToken), Encoding.UTF8, JsonUtility.SerializerByNewton(menus))
                );
            if (string.IsNullOrEmpty(resp.errcode) || resp.errcode == "0")
            {
                rtn.msg = "操作成功";
            }
            else
            {
                rtn.idx = "-1";
                rtn.msg = resp.errmsg;
                Loger.Error(resp.errmsg + ";access_token=" + accessToken);
            }
            return Js(rtn);
        }
        public JsResult delete(string passkey)
        {
            SysResult rtn = new SysResult();
            if (passkey != PASSKEY)
            {
                return Js(rtn);
            }
            BllWxResponse resp = JsonUtility.DeserializeByNewton<BllWxResponse>(
                    GetContentByUrl(string.Format(BllWxResponse.delMenuUrl, BllWxBase.Get().GetAccessToken(TokeKey)), Encoding.UTF8)
                    );
            if (string.IsNullOrEmpty(resp.errcode))
            {
                rtn.msg = "操作成功";
            }
            else
            {
                rtn.idx = "-1";
                rtn.msg = resp.errmsg;
                Loger.Error(resp.errmsg);
            }
            return Js(rtn);
        }
        public JsResult get()
        {
            SysResult rtn = new SysResult();

            rtn.msg = GetContentByUrl(string.Format(BllWxResponse.getMenuUrl, BllWxBase.Get().GetAccessToken(TokeKey)), Encoding.UTF8);
            return Js(rtn);
        }
        public JsResult getFriend(string passkey)
        {
            SysResult rtn = new SysResult();
            if (passkey != PASSKEY)
            {
                return Js(rtn);
            }
            rtn.msg = GetContentByUrl(string.Format("https://mp.weixin.qq.com/cgi-bin/contactmanagepage?t=wxm-friend&token={0}&lang=zh_CN&pagesize=10&pageidx=0&type=0&groupid=0", BllWxBase.Get().GetAccessToken(TokeKey)), Encoding.UTF8);

            return Js(rtn);
        }

    }
}
