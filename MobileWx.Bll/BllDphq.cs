using MobileWx.Model;
using Sys.SysCache;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Bll
{
    public class BllDphq:BllBase,IBllMenuResponse
    {
        public void SetResponse(Model.WxResponse resp, string menuKey)
        {

            //resp.Content = "<table><tr><td>青岛啤酒</td><td>600600</td></tr><tr><td>浦发银行</td><td>600000</td></tr></table>";
        }
        public string SetPadding(object v,int len=7)
        {
            if (v == null) return "".PadLeft(len);
            return v.ToString().PadLeft(len);
        }
        public void SetDphqResponse(Model.WxResponse resp, MCacheClient sddquotes)
        { 
            resp.MsgType = ModelWx.MsgType_news;
            string sh000001 = "sh000001";
            Dictionary<string, string> stocks = new Dictionary<string, string>();
            stocks.Add("sz399001","深证成指");
            stocks.Add("sz399006", "创业板指 ");
            stocks.Add("sz399101", "中小板综 ");
            stocks.Add("sz399300", "沪深300 ");
            string hq = sddquotes.Get("EMONEY_SDD_QUOTES_" + sh000001);
            if (!string.IsNullOrEmpty(hq))
            {
                List<ModelHq> lsts = JsonUtility.DeserializeByNewton<List<ModelHq>>(hq);
                if (lsts != null && lsts.Count > 0)
                {
                    hq = string.Format("{0} {1}\n涨额 {2} 涨幅 {3}%", "上证指数", lsts[0].P, lsts[0].D.Value.ToString("F2"), lsts[0].F);
                }
            }
            List<string> des = new List<string>();
            foreach (string k in stocks.Keys)
            {
                string s= sddquotes.Get("EMONEY_SDD_QUOTES_" + k);
                if (!string.IsNullOrEmpty(s))
                {
                    List<ModelHq> lsts = JsonUtility.DeserializeByNewton<List<ModelHq>>(s);
                    if (lsts != null && lsts.Count > 0)
                    {
                        s = string.Format("{0} {1} {2} {3}%", stocks[k], SetPadding(lsts[0].P,8), SetPadding(lsts[0].D.Value.ToString("F2")), SetPadding(lsts[0].F));
                        des.Add(s);
                    }
                }            
            }
                resp.Articles = new List<WxArticle>() { 
                new WxArticle(){
                    PicUrl="http://www.ymcps.com/images/zhishunew/000001.jpg?t="+DateTime.Now.ToString("yyMMddHHmmss"),
                    Title=hq,
                    Description=string.Join("\n\n",des),
                    Url="http://wap.emoney.cn"
                }
            };
        }
    }
}
