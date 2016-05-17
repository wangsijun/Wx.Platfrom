using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
namespace MobileWx.Model
{
    public class ModelWx : ModelBase
    {
        #region MsgType
        /// <summary>
        /// text
        /// </summary>
        public const string MsgType_text = "text";
        /// <summary>
        /// image
        /// </summary>
        public const string MsgType_image = "image";
        /// <summary>
        /// location
        /// </summary>
        public const string MsgType_location = "location";
        /// <summary>
        /// link
        /// </summary>
        public const string MsgType_link = "link";
        /// <summary>
        /// news
        /// </summary>
        public const string MsgType_news = "news";
        /// <summary>
        /// event
        /// </summary>
        public const string MsgType_event = "event";
        /// <summary>
        /// 多客服
        /// </summary>
        public const string MsgType_transfer_customer_service = "transfer_customer_service";

        #endregion

        #region Event
        /// <summary>
        /// subscribe(订阅)
        /// </summary>
        public const string Event_subscribe = "subscribe";
        /// <summary>
        /// unsubscribe(取消订阅)
        /// </summary>
        public const string Event_unsubscribe = "unsubscribe";
        /// <summary>
        /// CLICK(自定义菜单点击事件) 
        /// </summary>
        public const string Event_CLICK = "CLICK";
        #endregion

        #region MenuType
        /// <summary>
        /// click
        /// </summary>
        public const string MenuType_click = "click";
        #endregion

        #region MenuKeys
        /// <summary>
        /// 资讯要闻zxyw
        /// </summary>
        public const string MenuKeys_zxyw = "zxyw";
        /// <summary>
        /// 个股查询ggcx
        /// </summary>
        public const string MenuKeys_ggcx = "ggcx";
        /// <summary>
        /// 自选股
        /// </summary>
        public const string MenuKeys_zxg = "zxg";
        /// <summary>
        /// 个股诊断ggzd
        /// </summary>
        public const string MenuKeys_ggzd = "ggzd";
        /// <summary>
        /// 大盘行情dphq
        /// </summary>
        public const string MenuKeys_dphq = "dphq";
        /// <summary>
        /// 业务查询ywcx
        /// </summary>
        public const string MenuKeys_ywcx = "ywcx";
        /// <summary>
        /// 有奖闯关yjcg
        /// </summary>
        public const string MenuKeys_yjcg = "yjcg";

        /// <summary>
        /// 有奖闯关view
        /// </summary>
        public const string MenuKeys_view = "view";

        /// <summary>
        /// 密码查询 mmcx
        /// </summary>
        public const string MenuKeys_mmcx = "mmcx";

        /// <summary>
        /// 到期日 dqr
        /// </summary>
        public const string MenuKeys_dqr = "dqr";

        /// <summary>
        /// 积分兑换 jfdh
        /// </summary>
        public const string MenuKeys_jfdh = "jfdh";

        /// <summary>
        /// 软件下载 rjxz
        /// </summary>
        public const string MenuKeys_rjxz = "rjxz";

        /// <summary>
        /// 在线购买 zxgm
        /// </summary>
        public const string MenuKeys_zxgm = "zxgm";

        /// <summary>
        /// 功能介绍 gnjs
        /// </summary>
        public const string MenuKeys_gnjs = "gnjs";


        /// <summary>
        /// 实战技巧 szjq
        /// </summary>
        public const string MenuKeys_szjq = "szjq";

        /// <summary>
        /// 推荐好友 tjhy
        /// </summary>
        public const string MenuKeys_tjhy = "tjhy";

        /// <summary>
        /// 意见反馈 yjfk
        /// </summary>
        public const string MenuKeys_yjfk = "yjfk";


        /// <summary>
        /// 专属客服，转到多客服
        /// </summary>
        public const string MenuKeys_zskf = "zskf";


        /// <summary>
        /// 股市机会－赚钱日志
        /// </summary>
        public const string MenuKeys_zqrz = "zqrz";

        /// <summary>
        /// 股市机会－接力柴静概念股的是谁？
        /// </summary>
        public const string MenuKeys_jlcjgng = "jlcjgng";

        /// <summary>
        /// 股市机会－杨国荣老师战法集锦
        /// </summary>
        public const string MenuKeys_ygrzf = "ygrzf";

        /// <summary>
        /// 股市机会－春节研报红包
        /// </summary>
        public const string MenuKeys_cjybhb = "cjybhb";
        /// <summary>
        /// 加强版-我的 - 预警消息
        /// </summary>
        public const string MenuKeys_yjxx = "yjxx";
        /// <summary>
        /// 加强版-我的 - 等级服务
        /// </summary>
        public const string MenuKeys_djfw = "djfw";
        /// <summary>
        /// 加强版-我的 - 兑换记录
        /// </summary>
        public const string MenuKeys_dhjl = "dhjl";
        public const string MenuKeys_hd = "hd";
        /// <summary>
        /// 加强版-我的 - 绑定账号
        /// </summary>
        public const string MenuKeys_bdzh = "bdzh";
        /// <summary>
        /// 加强版-我的 - 账号管理
        /// </summary>
        public const string MenuKeys_zhgl = "zhgl";
        /// <summary>
        /// 加强版-我的 - 注册
        /// </summary>
        public const string MenuKeys_zc = "zc";
        /// <summary>
        /// 加强版-我的 - 忘记密码
        /// </summary>
        public const string MenuKeys_wjmm = "wjmm";
        #endregion
        public ModelWx()
        {
            MsgType = MsgType_text;
        }
        public void SetCreateTime(DateTime d)
        {
            CreateTime = (d - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
        public string ToUserName
        {
            get;
            set;
        }
        public string FromUserName
        {
            get;
            set;
        }
        public string CreateTime
        {
            get;
            set;
        }
        public string MsgType
        {
            get;
            set;
        }

        public int ArticleCount
        {
            get; set;
        }

        public string Content
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        } 
        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(ToUserName))
            {
                writer.WriteStartElement("ToUserName");
                writer.WriteCData(ToUserName);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(FromUserName))
            {
                writer.WriteStartElement("FromUserName");
                writer.WriteCData(FromUserName);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(CreateTime))
            {
                writer.WriteStartElement("CreateTime");
                writer.WriteString(CreateTime);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(MsgType))
            {
                writer.WriteStartElement("MsgType");
                writer.WriteCData(MsgType);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(Content))
            {
                writer.WriteStartElement("Content");
                writer.WriteCData(Content);
                writer.WriteEndElement();
            }

        }

    }
}
