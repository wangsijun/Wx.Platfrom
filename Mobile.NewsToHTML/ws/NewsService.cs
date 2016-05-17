using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Mobile.NewsToHTML
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“NewsService”。
    public class NewsService : INewsService
    {
        public void DoWork()
        {
        }
        /// <summary>
        /// 同步文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        public void FileCopy(string filePath, string fileContent)
        {
            string s = FileUtility.WriteText(filePath, fileContent);
            Loger.ConsoleLine(Program.logModel, filePath);
            if (s != "")
            {
                Loger.Error(filePath + "\r\n" + s);
                Loger.ConsoleLine(Program.logModel, s);
            }
        }
        #region 配置属性
        public string daShiJsonPath
        {
            get;
            set;
        }
        public string zxywJsonPath
        {
            get;
            set;
        }
        public string zxywHtmlPath
        {
            get;
            set;
        }
        public string zxywHtmlUrl
        {
            get;
            set;
        }
        public string httpUrl
        {
            get;
            set;
        }
        public string tcpUrl
        {
            get;
            set;
        }

        public string zxyProwHtmlPath
        {
            get; set;
        }
        #endregion
    }
}
