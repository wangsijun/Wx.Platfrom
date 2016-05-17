using MobileWx.Model;
using Sys.Spring;
using Sys.SysCache;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using MobileWx.Bll.DeserializeModel;
using Newtonsoft.Json;

namespace MobileWx.Bll
{
    public class BllGgzd : BllBase, IBllMenuResponse
    {

        public void SetResponse(WxResponse resp, string menuKey)
        {
            resp.Content = "请输入股票名称或代码\n如：'青岛啤酒'或'600600'";
        }
        /// <summary>
        /// 设置按股票查诊股的回复
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="stock"></param>
        public void SetZhenGuResponse(Model.WxResponse resp, Stock stock, MCacheClient sdstockscoreex, MCacheClient pgfirst_y, MCacheClient sddquotes)
        {

            resp.MsgType = ModelWx.MsgType_news;

            string secucode = (stock.s.StartsWith("6") ? "sh" : "sz") + stock.s;


            string pj = sdstockscoreex.Get("sdstockscoreex" + stock.s);//取评级
            //600600,7.3,5.0,5.9,7.5,7.0,6.9,5.8,4.4,A,2,2,90%,资金正在持续流出 筹码趋于分散,2,0,平稳,优秀,恭喜，该股战胜了90%的股票,
            if (!string.IsNullOrEmpty(pj)) pj = pj.Split(',')[9];

            string zs = pgfirst_y.Get("pgfirst_y" + stock.s7);//体检综述            
            if (!string.IsNullOrEmpty(zs))
            {
                List<Pgfirst_yEntity> lsts = JsonUtility.DeserializeByNewton<List<Pgfirst_yEntity>>("[" + zs + "]");
                zs = lsts[0].root[0].top[0].data[0].HowToDo + "\n";
            }
            string hq = sddquotes.Get("EMONEY_SDD_QUOTES_" + secucode);
            if (!string.IsNullOrEmpty(hq))
            {
                List<ModelHq> lsts = JsonUtility.DeserializeByNewton<List<ModelHq>>(hq);
                if (lsts != null && lsts.Count > 0)
                {
                    hq = string.Format("最新价{0}\n涨跌额{1}   涨跌幅{2}%\n成交量{3}手   成交额{4}", lsts[0].P, lsts[0].D, lsts[0].F, lsts[0].V, "");
                }
            }
            resp.Articles = new List<WxArticle>() {
                new WxArticle(){
                    PicUrl=string.Format("http://static.emoney.cn/sixangle/SOSO_SixAngle_{0}.PNG?r={1}",secucode, DateTime.Now.ToString("yyyyMMddHHMM")),
                    Title=string.Format("{0}体验评级:{1} {2}",stock.n,pj,zs),
                    Description=hq,
                    Url=string.Format("http://mt.emoney.cn/html/weixin1/WebApp/Home/OneStock.html?S={0}",stock.s)
                    //Url=string.Format("http://m.emoney.cn/sosoSD/test_pub.html?sc={0}&returnf=1",stock.s)
                }
            }; 
        }

        public void SetGeGuResponse(Model.WxResponse resp, Stock stock, MCacheClient sdstockscoreex, MCacheClient pgfirst_y, MCacheClient sddquotes)
        {

            resp.MsgType = ModelWx.MsgType_news;

            //var resultStr = GetHttpPostResponse($"http://emwxgpys.emoney.cn/dyh/common/Index?code={stock.s}");

            //var resultJson = JsonConvert.DeserializeObject<List<ProGgcx>>(resultStr);

            resp.Articles = GgcxList(stock.s).Select(re => new WxArticle()
            {
                PicUrl = re.ImageUrl,
                Title = re.title,
                Url = re.LinkUrl
            }).ToList(); ;
        }

        public List<ProGgcx> GgcxList(string stock)
        {
            var resultStr = GetHttpResponse($"http://emwxgpys.emoney.cn/dyh/common/Index?code={stock}");

            return JsonConvert.DeserializeObject<List<ProGgcx>>(resultStr);

            
        }
        ///  <summary>
        /// HttpGet
        ///  </summary>
        ///  <param name="url"></param>
        /// <returns></returns>
        private string GetHttpResponse(string url)
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var client = new HttpClient(handler);
            var result = client.GetAsync(url).Result;
            return result.Content.ReadAsStringAsync().Result;
        }
    }
}
