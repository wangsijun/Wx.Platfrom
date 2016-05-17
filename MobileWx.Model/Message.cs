using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{
    public class Message
    {
        public int Id { get; set; }

        /// <summary>
        /// 消息栏目Id
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 页面正文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 图片名
        /// </summary>
        public string PicName { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime RecTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModTime { get; set; }
    }
}
