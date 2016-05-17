using MobileWx.Model;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileWx.Bll
{
    public class BllBase
    {
        private static Dictionary<string, Stock> _stocks;
        /// <summary>
        /// 包含了以股票编号或股票名称为key的数据
        /// </summary>
        public static Dictionary<string, Stock> stocks
        {
            get
            {
                if (_stocks == null) _stocks = new Dictionary<string, Stock>();
                return _stocks;
            }
            set { _stocks = value; }
        }
        public static Dictionary<string, Stock> GetStocks()
        {
            Dictionary<string, Stock> rtn = new Dictionary<string, Stock>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings["StockUrl"]);

                WebResponse resp = request.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.Default);
                string s = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();

                s = s.Replace("_C", "s");
                s = s.Replace("_S", "c");
                s = s.Replace("_N", "n");
                List<Stock> stocks = JsonUtility.DeserializeByNewton<List<Stock>>(s);
          
                foreach (Stock stock in stocks)
                { 
                    stock.s = StringUtility.EnStock(stock.s);
                    if (stock.s.Length < 6) continue;
                    if (!stock.s.StartsWith("60") && !stock.s.StartsWith("100") && !stock.s.StartsWith("130")) continue;
                    stock.s = stock.s.Substring(stock.s.Length - 6);
                    if (rtn.ContainsKey(stock.s)) continue;
                    rtn[stock.s] = stock;
                    rtn[stock.n.Replace(" ", "")] = stock;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            if (rtn.Count == 0 && stockRetry < 10)
            {
                Thread.Sleep(1000 * 10);
                stockRetry++;
                Loger.Debug(string.Format("GetStocks：{0}", stockRetry));
                GetStocks();
            }
            else
            {
                Loger.Debug(string.Format("Stocks Count：{0}", rtn.Count));
                stockRetry = 0;
            }
            if (rtn.Count > 0)
            {
                BllBase.stocks = rtn;
            }
            return rtn;
        }
        static int stockRetry = 0;
        #region ##过滤股票名称
        public static string StockLink = "<a href='javascript:goods.showgoods({0},\"{1}\");void(0);'>{2}</a>";
        public static string FilterContent(string infoContent)
        {
            string rtn = StringUtility.ToHtml(infoContent);
            /*
            var q=stocks.Values.Distinct().ToList();
            foreach (Stock obj in q)
            {
                rtn = rtn.Replace(obj.s, string.Format(StockLink, obj.s7, "<N》", obj.s));
                rtn = rtn.Replace(obj.n, string.Format(StockLink, obj.s7, "<N》", "<N》"));
                rtn = rtn.Replace("<N》", obj.n);
            }
             * */
            return rtn;
        }
        #endregion 
    }
}
