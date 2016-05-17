using Mobile.NewsToHTML.NewsServiceWs;
using MobileWx.Bll;
using MobileWx.Model;
using Sys.Spring;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Mobile.NewsToHTML
{
    class Program
    {
        public static long? maxNewsId = 0;
        //普通资讯模板
        public static string htmlTemplet = "";
        public static string prohtmlTemplet = "";
        static INewsService newsWs = (INewsService)SysSpring.GetByName("INewsService");
        static ServiceHost selfHost;
        public static Thread CheckWcfWsThread = null;
        public static Thread MainThread = null;
        public const string logModel = "WxWcf";
        static string CopyToServers = ConfigurationManager.AppSettings["CopyToServers"];
        static void Main(string[] args)
        {
            Console.Title = Application.ExecutablePath;
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
            ConsoleWin32Helper.DisableCloseButton(Console.Title);
            maxNewsId = StringUtility.ToInt64(FileUtility.ReadText(StringUtility.AppPath + "Configure/DataConfig.txt")) ?? 0;
            htmlTemplet = StringUtility.ToString(FileUtility.ReadText(StringUtility.AppPath + "Configure/htmlTemplet.html")) ?? "";
            prohtmlTemplet = StringUtility.ToString(FileUtility.ReadText(StringUtility.AppPath + "Configure/prohtmlTemplet.html")) ?? "";
            try
            {
                BllBase.GetStocks();
                //StartWCF();
                //CheckWcfWsThread = new Thread(new ThreadStart(CheckWcfWs));
                //CheckWcfWsThread.Start();
                //if (!string.IsNullOrEmpty(CopyToServers))
                //{
                MainThread = new Thread(new ThreadStart(LoadData));
                MainThread.Start();
                //}
            }
            catch (Exception ex)
            {
                Loger.ConsoleLine(logModel, ex.Message);
                Loger.Error(ex);
            }
            ReadLine();
        }
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (CheckWcfWsThread != null) CheckWcfWsThread.Abort();
            if (MainThread != null) MainThread.Abort();
            if (selfHost != null) selfHost.Close();
        }
        public static void CheckWcfWs()
        {
            while (true)
            {
                Thread.Sleep(1000 * 30);
                try
                {
                    if (selfHost != null && (selfHost.State != CommunicationState.Opened))
                    {
                        StartWCF();
                    }
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
        }
        public static void StartWCF()
        {
            selfHost = new ServiceHost(typeof(NewsService), new Uri(newsWs.httpUrl));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas()
            {
                MaxStringContentLength = 10240000
            };
            selfHost.AddServiceEndpoint(typeof(INewsService), new NetTcpBinding(SecurityMode.None)
            {
                ReaderQuotas = quotas,
                MaxReceivedMessageSize = 10240000,
                MaxConnections = 1000
            }, new Uri(newsWs.tcpUrl));
            selfHost.Description.Behaviors.Add(smb);
            ((ServiceDebugBehavior)selfHost.Description.Behaviors[typeof(ServiceDebugBehavior)]).IncludeExceptionDetailInFaults = true;

            selfHost.Open();

            Console.WriteLine(newsWs.httpUrl);
        }
        public static void ReadLine()
        {
            //Application.DoEvents();
            string s = Console.ReadLine();
            if (s == "clear")
            {
                Console.Clear();
            }
            else if (s == "exit")
            {
                if (CheckWcfWsThread != null) CheckWcfWsThread.Abort();
                if (MainThread != null) MainThread.Abort();
                if (selfHost != null) selfHost.Close();
                Application.ExitThread();
                return;
            }

            ReadLine();
        }
        static void LoadData()
        {
            while (true)
            {
                try
                {

                    ToZxywHtml();
                    ToDaShiHtml();

                    ToMessageHtml();

                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                    Loger.ConsoleLine(logModel, ex.Message);
                }
                Thread.Sleep(1000 * 60 * 2);
            }
        }

        /// <summary>
        /// 生成欢迎、赚钱日志等信息正文
        /// </summary>
        private static void ToMessageHtml()
        {
        }


        static NewsToHTMLClient client = null;
        public static void ToZxywHtml()
        {
            BllNewsTab newsTab = new BllNewsTab();

            List<WxArticle> Articles = new List<WxArticle>();
            List<ModelNewsTab> news = newsTab.GetZXYWList();
            long? maxid = maxNewsId;
            foreach (ModelNewsTab obj in news)
            {
                WxArticle art = new WxArticle()
                {
                    Description = " ",
                    Title = obj.title,
                    Url = string.Format(newsWs.zxywHtmlUrl, obj.id),
                    PicUrl = obj.imgUrl
                };

                Articles.Add(art);
                //主力资金播报              
                if (obj.subClass == "2")
                {
                    art.Url = string.Format("http://mt.emoney.cn/wx/News/ZJBB?id={0}", obj.id);
                    continue;
                }
                //领涨和个股点评              
                if (obj.subClass == "3")
                {
                    art.Url = string.Format("http://mt.emoney.cn/wx/News/GGDP?id={0}", obj.id);
                    continue;
                }
                if (!GenHtmlById(Convert.ToInt32(obj.id))) continue;
                if (obj.id > maxid) maxid = obj.id;
            }
            if (maxid > maxNewsId)
            {
                maxNewsId = maxid;
                FileUtility.WriteText(StringUtility.AppPath + "Configure/DataConfig.txt", maxNewsId.ToString());
                CopyFileToServers(StringUtility.AppPath + "Configure/DataConfig.txt", maxNewsId.ToString());
            }
            FileUtility.WriteJson(newsWs.zxywJsonPath, Articles);
            CopyFileToServers(newsWs.zxywJsonPath, JsonUtility.SerializerByNewton(Articles));
            news.Clear();
            news = null;
            Loger.ConsoleLine(logModel, newsWs.zxywJsonPath);
        }
        static void ToDaShiHtml()
        {
            List<WxArticle> Articles = new List<WxArticle>();
            ModelNewsTab obj = BllNewsTab.Get().GetDaShi();
            if (obj != null)
            {
                int len = (obj.content.IndexOf("。") > 0) ? obj.content.IndexOf("。") : obj.content.IndexOf("\n");
                if (len < 0) len = 0;
                WxArticle art = new WxArticle()
                {
                    Description = obj.content.Substring(0, len) + "...",
                    Title = obj.title,
                    Url = string.Format(newsWs.zxywHtmlUrl, obj.id),
                    PicUrl = obj.imgUrl
                };

                Articles.Add(art);
                if (!GenHtmlById(obj)) return;
            }
            FileUtility.WriteJson(newsWs.daShiJsonPath, Articles);
            CopyFileToServers(newsWs.daShiJsonPath, JsonUtility.SerializerByNewton(Articles));
            Loger.ConsoleLine(logModel, newsWs.daShiJsonPath);
        }
        static bool GenHtmlById(int? id)
        {
            ModelNewsTab obj = BllNewsTab.Get().getById(id);
            return GenHtmlById(obj);
        }
        static bool GenHtmlById(ModelNewsTab obj)
        {

            Task<bool> task = new Task<bool>(() =>
            {
                return GenerationTemplate(obj, prohtmlTemplet, newsWs.zxyProwHtmlPath);
            });
            task.Start();

            GenerationTemplate(obj, htmlTemplet, newsWs.zxywHtmlPath);
          
            return true;
        }

        private static bool GenerationTemplate(ModelNewsTab obj, string timelateContent, string path)
        {
            if (obj == null) return false;
            if (File.Exists(string.Format(newsWs.zxywHtmlPath, obj.id))) return true;
            string content = timelateContent.Replace("$title$", obj.title);
            content = content.Replace("$createDate$", obj.createDate.ToString());
            content = content.Replace("$content$", BllBase.FilterContent(obj.content));

            if (!string.IsNullOrEmpty(obj.imgUrl))
            {
                content = content.Replace("$imgUrl$", $"background-image:url({obj.imgUrl})");
            }
            else
            { 
                content = content.Replace("$imgUrl$", "display:none;");
            }

            FileUtility.WriteText(string.Format(path, obj.id), content);
            //CopyFileToServers(string.Format(newsWs.zxywHtmlPath, obj.id), content); 

            Loger.ConsoleLine(logModel, string.Format(newsWs.zxywHtmlPath, obj.id));
            return true;
        }

        public static void CopyFileToServers(string filePath, string fileContent)
        {
            if (string.IsNullOrEmpty(CopyToServers)) return;
            try
            {
                if (client == null || client.State != CommunicationState.Opened) client = new NewsToHTMLClient(CopyToServers);
                client.FileCopy(filePath, fileContent);
            }
            catch (Exception ex)
            {
                Loger.Error(filePath + StringUtility.GetMessage(ex));
                System.Timers.Timer t = new System.Timers.Timer(45 * 1000);
                t.Elapsed += new System.Timers.ElapsedEventHandler((p1, p2) =>
                {
                    try
                    {
                        if (client == null || client.State != CommunicationState.Opened) client = new NewsToHTMLClient(CopyToServers);
                        client.FileCopy(filePath, fileContent);
                        Loger.Error(filePath + " succeed");
                        t.Stop();
                        t.Dispose();
                    }
                    catch (Exception ex2)
                    {
                        Loger.Error(filePath + StringUtility.GetMessage(ex2));
                    }
                });
                t.Start();
            }
        }
    }

}
