using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Mobile.NewsToHTML
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“INewsService”。
    [ServiceContract]
    public interface INewsService
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        void FileCopy(string filePath, string fileContent);
        string daShiJsonPath
        {
            get;
            set;
        }
        string zxywJsonPath
        {
            get;
            set;
        }

        string zxyProwHtmlPath
        {
            get; set;
        }

        string zxywHtmlPath
        {
            get;
            set;
        }

        

        string zxywHtmlUrl
        {
            get;
            set;
        }
        string httpUrl
        {
            get;
            set;
        }
        string tcpUrl
        {
            get;
            set;
        }
    }
}
