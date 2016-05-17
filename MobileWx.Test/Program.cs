using MobileWx.Bll;
using MobileWx.Model.ProWx;
using MobileWx.Test.ServiceReference1;
using Newtonsoft.Json;
using Sys.Spring;
using Sys.SysCache;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using MobileWx.Model;
using Sys.Utility;

namespace MobileWx.Test
{
    public class ResponseResult
    {
        public object data { get; set; }
        public string message { get; set; }
        public string miniProfiler { get; set; }
        public int status { get; set; }
        public int statusCode { get; set; }
        public bool success { get; set; }
        public DateTime updateTime { get; set; }
    }

    public class ZxgRootobject
    {
        public Datum[] Data { get; set; }
        public string Msg { get; set; }
        public int RtState { get; set; }
        public long TimeSpan { get; set; }

        public class Datum
        {
            public string group { get; set; }
            public string pid { get; set; }
            public string stocks { get; set; }
            public long TimeSpan { get; set; }
            public string uid { get; set; }
        }
    }





    internal class Program
    {
        public static BllGgzd bllGgzd = (BllGgzd)SysSpring.GetByName("BllGgzd");
        public static MCacheClient pgfirst_y = (MCacheClient)SysSpring.GetByName("pgfirst_y");
        public static MCacheClient sdstockscoreex = (MCacheClient)SysSpring.GetByName("sdstockscoreex");
        private static RCacheClient cache = new RCacheClient();
        public static MCacheClient Sddquotes { get; set; } = (MCacheClient)SysSpring.GetByName("sddquotes");


        public static void Main(string[] args)
        {
            var wxList = FileUtility.ReadJson<List<WxArticle>>(@"D:\web\wx.mt.emoney.cn\web\Json\daShi.json");
            MCacheClient mcache = (MCacheClient)SysSpring.GetByName("proggcq_y");

            var resultStr = GetHttpPostResponse($"http://emwxgpys.emoney.cn/dyh/common/Index?code={300059}");

            var resultJson = JsonConvert.DeserializeObject<List<ProGgcx>>(resultStr);
            var sssss = resultJson.Select(item => new WxArticle()
            {
                PicUrl = item.ImageUrl,
                Title = item.title,
                Url = item.LinkUrl
            }).ToList();




        }
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
        private static List<StockPrice> GetStockPrice(Int64? zxgId)
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
                        stockInfo.C != "399006")
                        resultPrice.Add(stockInfo);
                }
            }
            return resultPrice;
        }

        ///  <summary>
        /// HttpGet
        ///  </summary>
        ///  <param name="url"></param>
        /// <returns></returns>
        public static string GetHttpPostResponse(string url)
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


