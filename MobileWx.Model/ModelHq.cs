using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{
    [Serializable]
    public class ModelHq
    {
        /// <summary>
        /// 7位股票代码0600600
        /// </summary>
        public string C1
        {
            get;
            set;
        }
        /// <summary>
        /// 股票代码600600
        /// </summary>
        public string C
        {
            get;
            set;
        }
        /// <summary>
        /// 股票名称青岛啤酒
        /// </summary>
        public string N
        {
            get;
            set;
        }
        /// <summary>
        /// 现价43.23
        /// </summary>
        public decimal? P
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌幅-0.78
        /// </summary>
        public decimal? F
        {
            get;
            set;
        }
        /// <summary>
        /// 开盘价43.8
        /// </summary>
        public decimal? O
        {
            get;
            set;
        }
        /// <summary>
        /// 昨日收盘价43.57
        /// </summary>
        public decimal? Y
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌（额）-0.34
        /// </summary>
        public decimal? D
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌（额）15.93
        /// </summary>
        public decimal? FF0
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌（额）189.36
        /// </summary>
        public decimal? FF1
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌（额）38.07
        /// </summary>
        public decimal? FF2
        {
            get;
            set;
        }
        /// <summary>
        /// 涨跌（额）9.83
        /// </summary>
        public decimal? FF3
        {
            get;
            set;
        }
        /// <summary>
        /// 换手率0.2
        /// </summary>
        public decimal? I
        {
            get;
            set;
        }
        /// <summary>
        /// 最高价44.15
        /// </summary>
        public decimal? H
        {
            get;
            set;
        }
        /// <summary>
        /// 最低价43
        /// </summary>
        public decimal? L
        {
            get;
            set;
        }
        /// <summary>
        /// 成交量（总手）13627
        /// </summary>
        public decimal? V
        {
            get;
            set;
        }
        /// <summary>
        /// 5日涨幅3.67
        /// </summary>
        public decimal? F5
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? GY
        {
            get;
            set;
        }
        /// <summary>
        /// 市盈率 29.93
        /// </summary>
        public decimal? SY
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? SYR
        {
            get;
            set;
        }
        public decimal? A
        {
            get;
            set;
        }
    }
}
