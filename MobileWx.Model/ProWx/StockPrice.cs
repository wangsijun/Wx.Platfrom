using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model.ProWx
{
    /// <summary>
    /// 个股行情模型
    /// </summary>
    [Serializable]
    public class StockPrice
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string N { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string C { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public long TT { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public long T { get; set; }

        /// <summary>
        /// 最新
        /// </summary>
        public decimal? P { get; set; }

        /// <summary>
        /// 总手
        /// </summary>
        public long V { get; set; }

        /// <summary>
        /// 涨幅
        /// </summary>
        public decimal? F { get; set; }

        public decimal? F5 { get; set; }

        /// <summary>
        /// 涨跌
        /// </summary>
        public decimal? D { get; set; }

        /// <summary>
        /// 最高
        /// </summary>
        public decimal? H { get; set; }

        /// <summary>
        /// 最低
        /// </summary>
        public decimal? L { get; set; }

        /// <summary>
        /// 昨收
        /// </summary>
        public decimal? Y { get; set; }

        /// <summary>
        /// 开盘
        /// </summary>
        public decimal? O { get; set; }

        /// <summary>
        /// 外盘
        /// </summary>
        public long? WP { get; set; }

        /// <summary>
        /// 内盘
        /// </summary>
        public long? NP { get; set; }

        /// <summary>
        /// 涨停
        /// </summary>
        public decimal? ZT { get; set; }

        /// <summary>
        /// 跌停
        /// </summary>
        public decimal? DT { get; set; }

        /// <summary>
        /// 市盈率
        /// </summary>
        public decimal? SY { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        public decimal? A { get; set; }

        /// <summary>
        /// 总市值
        /// </summary>
        public decimal? ZSZ { get; set; }

        /// <summary>
        /// 振幅
        /// </summary>
        public decimal? ZF { get; set; }

        /// <summary>
        /// 净买
        /// </summary>
        public decimal? J { get; set; }

        /// <summary>
        /// BS点
        /// </summary>
        public int BC { get; set; }

        /// <summary>
        /// BS出现的天数
        /// </summary>
        public int NC { get; set; }

        /// <summary>
        /// 换手
        /// </summary>
        public decimal? I { get; set; }

        public decimal? I5 { get; set; }

        public decimal? LB { get; set; }

        /// <summary>
        /// 主力买入
        /// </summary>
        public decimal? BGB { get; set; }

        /// <summary>
        /// 主力卖出
        /// </summary>
        public decimal? BGS { get; set; }

        /// <summary>
        /// 散户买入 
        /// </summary>
        public decimal? SMB { get; set; }

        /// <summary>
        ///  散户卖出 
        /// </summary>
        public decimal? SMS { get; set; }

        /// <summary>
        /// 强弱
        /// </summary>
        public decimal? QR { get; set; }

        /// <summary>
        /// 零以下代表不正常
        /// </summary>
        public int flag
        {
            get
            {
                //string str = StockPrice.
                //if (T.ToString().StartsWith(str))
                //{
                //    return 0;
                //}
                //else
                //{
                //    return -1;
                //}
                return 0;
            }
        }

        public string DeleteCode { set; get; }
        /// <summary>
        /// 股票评级
        /// </summary>
        public string PJ { set; get; }
        /// <summary>
        /// bs点
        /// </summary>
        public string bs { set; get; }
        /// <summary>
        /// z20
        /// </summary>
        public string z20 { set; get; }
        /// <summary>
        /// isAlarm
        /// </summary>
        public string isAlarm { set; get; }
        /// <summary>
        /// alarmclass
        /// </summary>
        public string alarmclass { set; get; }

        /// <summary>
        /// 设置预警的股票代码,7位股票代码
        /// </summary>
        public string SettingAlarmCode { set; get; }
        public DateTime RT { get; set; }
    }
}
