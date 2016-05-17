using MobileWx.Bll;
using MobileWx.Bll.FileTransServiceReference;
using MobileWx.Model;
using MobileWx.Web.Models;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Controllers
{
    public class AdminController : BaseController
    {
        public static string[] logins = new string[] { "admin" };
        public static string[] pwds = new string[] { "1qaz=[;." };

        [CheckAdmin]
        public ActionResult Index()
        {
#if DEBUG
            ViewBag.DEBUG = "true";
#endif
            return View();
        }
        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            for (int i = 0; i < logins.Length; i++)
            {
                if (logins[i] == login && pwds[i] == password)
                {
                    Response.Cookies.Add(new HttpCookie(CheckAdminAttribute.ADMIN_COOKIE, login));
                    return Content("success");
                }
            }
            return Content("fail");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = Request.Cookies[CheckAdminAttribute.ADMIN_COOKIE];
            if (cookie != null)
            {
                Response.Cookies[CheckAdminAttribute.ADMIN_COOKIE].Expires = DateTime.Now.AddYears(-1);
            }
            return RedirectToAction("Login");
        }

        [CheckAdmin]
        public ActionResult UploadImage(string subfolder = "docimage")
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    if (subfolder.IndexOfAny("/\\".ToCharArray()) != -1)
                    {
                        subfolder = "other";
                    }
                    string folderpath = Server.MapPath("~/Content/" + subfolder + "/");
                    if (!System.IO.Directory.Exists(folderpath))
                    {
                        System.IO.Directory.CreateDirectory(folderpath);
                    }
                    string filename = Request.Files[0].FileName;
                    filename = Guid.NewGuid().ToString("n") + filename.Substring(filename.LastIndexOf('.'));
                    string filepath = folderpath + filename;
                    Request.Files[0].SaveAs(filepath);
                    new Thread(SyncFiles).Start(filepath);
                    return Json(new { filename = filename }, "text/html");
                }
            }
            catch (Exception err)
            {
                Loger.Error(err);
            }
            return Content("error");
        }

        private void SyncFiles(object obj)
        {
            string filename = obj as string;
            List<string> files = new List<string> { filename };
            if (ImgUtility.Resize(filename, 360, 200))
            {
                files.Add(filename.Substring(0, filename.LastIndexOf('.')) + "_360_200.jpg");
            }
            if (ImgUtility.Resize(filename, 200, 200))
            {
                files.Add(filename.Substring(0, filename.LastIndexOf('.')) + "_200_200.jpg");
            }
            try
            {
                FileServerClient wcf = new FileServerClient("localfileserver");
                foreach (var file in files)
                {
                    wcf.SyncFile(file);
                }

                if (wcf.State == CommunicationState.Faulted)
                {
                    wcf.Abort();
                }
                else
                {
                    wcf.Close();
                }
            }
            catch (Exception err)
            {

                Loger.Error(err);
            }
        }

        [CheckAdmin]
        public ActionResult MessageListData(MessageTypeEnum msgtype, int page = 1, int pageSize = 20)
        {
            ResultData result = new ResultData();

            int rowcount;
            List<object> datalist = new List<object>();
            int index = 0;
            foreach (Message item in Bll.BllMessage.Get().GetMessageList(pageSize, page - 1, " where Type = " + (int)msgtype + " ", out rowcount))
            {
                datalist.Add(new
                {
                    Id = item.Id,
                    Title = item.Title,
                    item.PicName,
                    CreateTime = item.RecTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Index = ++index
                });
            }
            result.Data = new
            {
                datalist = datalist,
                itemcount = rowcount
            };
            result.Status = 0;
            return JSON(result);
        }

        #region 赚钱日志

        /// <summary>
        /// 赚钱日志
        /// </summary>
        /// <returns></returns>
        [CheckAdmin]
        public ActionResult Zqrz()
        {
            return View();
        }
        /// <summary>
        /// 赚钱日志
        /// </summary>
        /// <returns></returns>
        [CheckAdmin]
        public ActionResult ZqrzEdit(int id = 0)
        {
            Message doc = null;
            if (id > 0)
            {
                doc = Bll.BllMessage.Get().GetMessage(id);
            }
            if (doc == null)
            {
                doc = new Message()
                {
                    Type = (int)MessageTypeEnum.Zqrz
                };
            }
            return View(doc);
        }

        #endregion

        #region 杨国荣老师战法集锦
        [CheckAdmin]
        public ActionResult Ygrzf()
        {
            return View();
        }


        #endregion

        #region 春节研报红包

        [CheckAdmin]
        public ActionResult Cjybhb()
        {
            return View();
        }

        #endregion

        #region 接力柴静

        [CheckAdmin]
        public ActionResult Jlcj()
        {
            return View();
        }
        #endregion

        #region 稿件编辑/dialog

        [CheckAdmin]
        public ActionResult Edit(MessageTypeEnum msgtype, int id = 0)
        {
            ViewBag.closeArg = msgtype.ToString().ToLower();
            Message doc = null;
            if (id > 0)
            {
                doc = Bll.BllMessage.Get().GetMessage(id);
            }
            if (doc == null)
            {
                doc = new Message()
                {
                    Type = (int)msgtype
                };
            }
            return View(doc);
        }
        #endregion


        /// <summary>
        /// 新用户关注，欢迎词
        /// </summary>
        /// <returns></returns>
        [CheckAdmin]
        public ActionResult Welcome()
        {
            Message model = Bll.BllMessage.Get().GetLatestMessage(MessageTypeEnum.Welcome) ??
                new Message()
                {
                    Type = (int)MessageTypeEnum.Welcome
                };
            return View(model);
        }

        [CheckAdmin]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveMessage(Message msg)
        {
            Bll.BllMessage.Get().Save(msg);

            return Json(new { Success = true, Id = msg.Id });
        }

        #region 维护项目

        public ActionResult CreateThumbImages()
        {
            ResultData result = new ResultData();
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Server.MapPath("~/content/docimage"));
                foreach (System.IO.FileInfo item in dir.GetFiles())
                {
                    if (item.FullName.IndexOf("200_200") == -1 && item.FullName.IndexOf("360_200") == -1)
                    {
                        ImgUtility.Resize(item.FullName, 360, 200);
                        ImgUtility.Resize(item.FullName, 200, 200);
                    }
                }
            }
            catch (Exception err)
            {

                result.Message = err.Message;
            }
            return JSON(result);
        }

        /// <summary>
        /// 到微信服务器获取订阅用户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ReloadUserList()
        {
            List<string> openidList = Bll.BllWxBase.Get().GetSubscribeUsers("EMONEY");
            foreach (string openid in openidList)
            {
                BllSubscribeUser.Get().UpdateUser(openid);
            }
            return Content("ok");
        }

        public ActionResult GetUserList()
        {
            List<string> openidList = Bll.BllWxBase.Get().GetSubscribeUsers("EMONEY");
            StringBuilder buff = new StringBuilder();
            foreach (string openid in openidList)
            {
                buff.Append(openid + ";");
            }
            return Content(buff.ToString());
        }
        #endregion
    }
}
