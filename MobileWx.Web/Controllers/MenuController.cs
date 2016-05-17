using MobileWx.Bll;
using MobileWx.Model;
using Sys.Controller;
using Sys.Spring;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class MenuController : SysController
    {
        private const string PASSKEY = "1qaz=[;."; //自定义菜单命令的密码
        private const string snsapi_base_format = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_base&state=0&connect_redirect=1#wechat_redirect";
        //
        // GET: /Menu/
        public static BllWxResponse bllWxResponse = (BllWxResponse)SysSpring.GetByName("BllWxResponse");

        
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
                        }
                        //,
                        // new WxMenuItem(){
                        //    name="名师答疑",
                        //    type=ModelWx.MenuKeys_view,
                        //    url= string.Format(snsapi_base_format, bllWxResponse.appid, Server.UrlEncode(msdyurl) )
                        //}                     
                     }
            });
            menus.button.Add(new WxMenuItem()
            {

                //name = "社区",
                //type = ModelWx.MenuKeys_view,
                //url = "http://mp.wsq.qq.com/263753283"
                name = "自助服务",
                sub_button = new List<WxMenuItem>(){
                        new WxMenuItem(){
                            name="密码查询",
                            type=ModelWx.MenuKeys_view,
                            url= WxController.SiteUrl+ "/Ywcx/mscx"
                        },
                         new WxMenuItem(){
                            name="到期日查询",
                            type=ModelWx.MenuKeys_view,
                            url=WxController.SiteUrl+ "/Ywcx/dqr"

                        },
                          new WxMenuItem(){
                            name="积分服务",
                            type=ModelWx.MenuKeys_view,
                            url=WxController.SiteUrl+ "/Ywcx/jfdh"
                        }, 
                           new WxMenuItem(){
                            name="下载主力版",
                            //key=ModelWx.MenuKeys_rjxz,
                            type=ModelWx.MenuKeys_view,
                            url= WxController.SiteUrl + "/Download/Classic"

                        }, 
                           new WxMenuItem(){
                            name="下载加强版",                  
                            type=ModelWx.MenuKeys_view,
                            url= "http://wap.emoney.cn/pro/"
                        }
                     }

            });
            //menus.button.Add(new WxMenuItem()
            //{
            //    name = "股市机会",
            //    sub_button = new List<WxMenuItem>(){
            //            new WxMenuItem(){
            //                name="极速操作研报",
            //               key=ModelWx.MenuKeys_jlcjgng,
            //                type=ModelWx.MenuType_click
            //            },
            //            new WxMenuItem(){
            //                name="赚钱日志",
            //               key=ModelWx.MenuKeys_zqrz,
            //                type=ModelWx.MenuType_click
            //            },
                        
            //            //new WxMenuItem(){
            //            //    name="杨国荣老师战法集锦",
            //            //   key=ModelWx.MenuKeys_ygrzf,
            //            //    type=ModelWx.MenuType_click
            //            //},
                        
            //            new WxMenuItem(){
            //                name="春节研报红包",
            //               key=ModelWx.MenuKeys_cjybhb,
            //                type=ModelWx.MenuType_click
            //            }
            //         }
            //});

            string access_token = BllWxBase.Get().GetAccessToken("EMONEY");
            BllWxResponse resp = JsonUtility.DeserializeByNewton<BllWxResponse>(
                GetContentByUrl(string.Format(bllWxResponse.createMenuUrl, access_token), Encoding.UTF8, JsonUtility.SerializerByNewton(menus))
                );
            if (string.IsNullOrEmpty(resp.errcode))
            {
                rtn.msg = "操作成功";
            }
            else
            {
                rtn.idx = "-1";
                rtn.msg = resp.errmsg;
                Loger.Error(resp.errmsg + ";access_token=" + access_token);
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
                    GetContentByUrl(string.Format(bllWxResponse.delMenuUrl, BllWxBase.Get().GetAccessToken("EMONEY")), Encoding.UTF8)
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

            rtn.msg = GetContentByUrl(string.Format(bllWxResponse.getMenuUrl, BllWxBase.Get().GetAccessToken("EMONEY")), Encoding.UTF8);
            return Js(rtn);
        }
        public JsResult getFriend(string passkey)
        {
            SysResult rtn = new SysResult();
            if (passkey != PASSKEY)
            {
                return Js(rtn);
            }
            rtn.msg = GetContentByUrl(string.Format("https://mp.weixin.qq.com/cgi-bin/contactmanagepage?t=wxm-friend&token={0}&lang=zh_CN&pagesize=10&pageidx=0&type=0&groupid=0", BllWxBase.Get().GetAccessToken("EMONEY")), Encoding.UTF8);

            return Js(rtn);
        }
    }
}
