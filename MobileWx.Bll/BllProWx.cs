using MobileWx.Dal;
using MobileWx.Dal.Model;
using MobileWx.Model;
using MobileWx.Model.ProWx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MobileWx.Bll
{
    public class BllProWx
    {
        private readonly DalWx _proWx = new DalWx();

        /// <summary>
        /// 获取个股行情、价格、涨跌幅、名称等
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<StockPrice> GetStockInfoList(string code)
        {
            //LogManager.DefaultLogger.Info("GetStockInfoList code=" + code);
            //MyTrace.get().add("GetStockInfoList code=" + code);
            //string url = System.Configuration.ConfigurationManager.AppSettings["StockPrice"].ToString();
            string url = "http://cmsservice.emoney.cn/hangqing/GetLiveHangQingKeys.ashx";
            // 实时计算价格
            WebClient client = new WebClient();
            var u = url + "?sn=" + code;
            //MyTrace.get().add("GetStockInfoList2" + u);
            var s = client.OpenRead(new Uri(u));
            var t = new System.IO.StreamReader(s, Encoding.Default).ReadToEnd();
            //LogManager.DefaultLogger.Info("GetStockInfoList code33=" + t);
            //MyTrace.get().add("GetStockInfoList3" + t);
            var jsa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockPrice>>(t);
            return jsa;
        }

        public List<UserExchangeLog> GetUserExchangeLog(string openId)
        {
            var dbReult = _proWx.platform_user_get_exchange_byweixin(openId);
            return dbReult.Select(item => new UserExchangeLog
            {
                typename = item.typename,
                itemname = item.itemname,
                date_weixin = item.date_weixin
            }).ToList();
        }

        public UserInfo GetUserInfo(string openId)
        {
            DBCommoninfo commonInfo = _proWx.platform_user_get_commoninfo_byweixin(openId);

            UserInfo info = new UserInfo
            {
                UserPoint = commonInfo.point,
                UserLevel = commonInfo.userlevel,
                UserExp = commonInfo.userexp,
                UserName = commonInfo.account
            };

            return info;
        }

        public int RelieveBind(string userName, string pwd, string weixin, bool isbind)
        {
            return _proWx.sp_platform_user_bind_weixin(userName, pwd, weixin, isbind);
        }
    }
}