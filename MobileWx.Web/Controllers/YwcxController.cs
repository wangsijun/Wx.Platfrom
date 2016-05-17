using Sys.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using Sys.Utility;
using System.Data.SqlClient;
using MobileWx.Web.Models;
using System.Security.Cryptography;
using System.IO;

namespace MobileWx.Web.Controllers
{
    public class YwcxController : SysController
    {
        //

        private mobilewbs.mobilewbs wbs = new mobilewbs.mobilewbs();

        //业务查询首页
        public ActionResult Index()
        {
            return View();
        }

        //密码查询
        public ActionResult mscx()
        {
            return View();
        }

        //手机登录
        public ActionResult userLogin()
        {

            return View();
        }

        //PC登录
        public ActionResult userLoginPC()
        {

            return View();
        }

        //用户选择手机或PC
        public ActionResult userLoginPCorPhone()
        {

            return View();
        }

        //到期日查询
        public ActionResult dqr()
        {
            string cpnumber = string.Empty;
            if (Request.Cookies["userName"] != null)
            {
                cpnumber = Request.Cookies["userName"].Value;
            }

            if (string.IsNullOrEmpty(cpnumber))
            {
                Response.Cookies["pageType"].Value = "1";
                Response.Cookies["pageType"].Expires = DateTime.Now.AddHours(1);
                Response.Redirect("login");
            }
            else
            {
                @ViewBag.cpnumber = cpnumber;
                bind(cpnumber, "000000");
            }

            return View();
        }

        /// <summary>
        /// 验证手机号登录
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Stock(FormCollection fc)
        {
            if (!string.IsNullOrEmpty(fc["action"].ToString()))
            {
                string name = fc["txt_name"].ToString();
                string pwd = fc["txt_pwd"].ToString();
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。手机号或密码不能为空，请从新填写！'); window.location.href='{0}';</script>", "login"));
                    Response.Write("<script>document.location=document.location;</script>");
                }
                else
                {
                    string returnStr = wbs.User_login_forweixin(name, pwd);
                    string[] arry = returnStr.Split('|');
                    string str = arry[0].ToString();
                    string str2 = arry[1].ToString();//0 是老用户，1是新用户
                    if (str == "1")
                    {

                        Response.Cookies["userName"].Value = name;
                        Response.Cookies["userName"].Expires = DateTime.Now.AddHours(1);


                        Response.Cookies["userType"].Value = str2;
                        Response.Cookies["userType"].Expires = DateTime.Now.AddHours(1);

                        string pageType = string.Empty;
                        if (Request.Cookies["pageType"] != null)
                        {
                            pageType = Request.Cookies["pageType"].Value;
                        }

                        if (pageType == "1")
                        {
                            Response.Redirect("dqr");
                        }
                        else if (pageType == "2")
                        {
                            Response.Redirect("jfdh");
                        }
                        else
                        {
                            Response.Redirect("recharge");
                        }

                    }
                    else
                    {
                        Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。用户名或密码错误！'); window.location.href='{0}';</script>", "login"));
                        Response.Write("<script>document.location=document.location;</script>");
                    }
                }
            }
            return View();
        }


        /// <summary>
        /// 验证PC登录
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult StockPC(FormCollection fc)
        {
            if (!string.IsNullOrEmpty(fc["action"].ToString()))
            {
                string name = fc["txt_name"].ToString();
                string pwd = fc["txt_pwd"].ToString();
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。手机号或密码不能为空，请从新填写！'); window.location.href='{0}';</script>", "userLoginPC"));
                    Response.Write("<script>document.location=document.location;</script>");
                }
                else
                {

                    string cpnumber = string.Empty;
                    cpnumber = Request.Cookies["userName"].Value;
                    if (string.IsNullOrEmpty(cpnumber))
                    {
                        Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。手机号或密码不能为空，请从新填写！'); window.location.href='{0}';</script>", "login"));
                        Response.Write("<script>document.location=document.location;</script>");
                    }
                    mobilePay.mobilePay mp = new mobilePay.mobilePay();
                    int str = mp.VerifyPCUser(cpnumber, name, pwd);
                    if (str >= 0)
                    {
                        //用户类型

                        Response.Cookies["userType"].Value = "2";//PC老用户
                        Response.Cookies["userType"].Expires = DateTime.Now.AddHours(1);
                    }
                    else
                    {
                        Response.Cookies["userType"].Value = "3";//PC验证过新用户
                        Response.Cookies["userType"].Expires = DateTime.Now.AddHours(1);
                    }
                    Response.Redirect("recharge");

                }
            }
            return View();
        }

        //积分兑换使用期产品列表
        public ActionResult jfdh()
        {
            string cpnumber = string.Empty;
            if (Request.Cookies["userName"] != null)
            {
                cpnumber = Request.Cookies["userName"].Value;
            }

            if (string.IsNullOrEmpty(cpnumber))
            {
                Response.Cookies["pageType"].Value = "2";
                Response.Cookies["pageType"].Expires = DateTime.Now.AddHours(1);
                Response.Redirect("login");
            }

            mobilewbs.mobilewbs ws = new mobilewbs.mobilewbs();
            string retStr = ws.sp_qry_mobile_loyalty_points_weixin(cpnumber);
            string[] arry = retStr.Split('|');
            ViewBag.jifen = arry[0];

            DataSet ds = new DataSet();
            ds = Get_productList();

            if (ds.Tables[0].Rows.Count > 0)
            {
                StringBuilder build = new StringBuilder();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    build.Append(" <tr><td id=\"td-item\"><div id=\"div-item\"><img width=\"88px\" height=\"57px\" alt=\"img\" src=\"../Css/def/images/");
                    build.Append((i + 1).ToString());
                    build.Append(".png\"><div><span class=\"content content-title\">");
                    build.Append(ds.Tables[0].Rows[i]["cardName"].ToString());
                    build.Append("</span><span class=\"content content-point\">");
                    build.Append(ds.Tables[0].Rows[i]["needIntegral"].ToString());
                    build.Append("积分</span><span class=\"content content-remain\">剩余数量:");
                    build.Append(ds.Tables[0].Rows[i]["surplus"].ToString());
                    build.Append("  </span></div><a href=\"jfdh2?type=" + ds.Tables[0].Rows[i]["cardType"].ToString() + "\" class=\"item-button ui-link\" id=\"item-button\" rowindex=\"1\">兑换 </a></div></td></tr>");

                }
                ViewBag.quote = Convert.ToString(build);
            }
            return View();
        }

        /// <summary>
        /// 积分兑换使用期
        /// </summary>
        /// <returns></returns>
        public ActionResult jfdh2()
        {
            string cpnumber = string.Empty;
            if (Request.Cookies["userName"] != null)
            {
                cpnumber = Request.Cookies["userName"].Value;
            }

            if (string.IsNullOrEmpty(cpnumber))
            {
                Response.Cookies["pageType"].Value = "2";
                Response.Cookies["pageType"].Expires = DateTime.Now.AddHours(1);
                Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。请先登录再查询！'); window.location.href='{0}';</script>", "login"));
                Response.Write("<script>document.location=document.location;</script>");

            }
            else
            {

                string query = Request.QueryString["type"].ToString();
                if (string.IsNullOrEmpty(query) == false)
                {
                    int integral = 0;
                    switch (query)
                    {
                        case "1":
                            integral = 2;
                            break;
                        case "2":
                            integral = 6;
                            break;
                        case "3":
                            integral = 12;
                            break;
                        case "4":
                            integral = 24;
                            break;
                        default:
                            integral = 0;
                            break;
                    }
                    if (integral > 0)
                    {
                        string str = wbs.Exchange_mobile_loyalty_points(cpnumber, integral);
                        string[] arry = str.Split('|');
                        string quote = string.Empty;
                        if (arry[1] == "0")
                        {
                            quote = "尊敬的用户，您好！积分兑换使用期成功！";
                        }
                        else
                        {
                            quote = "尊敬的用户，您好！" + arry[0];
                        }
                        Response.Write(String.Format("<script type='text/javascript'>alert('" + quote + "'); window.location.href='{0}';</script>", "jfdh"));
                        Response.Write("<script>document.location=document.location;</script>");
                    }
                    else
                    {
                        Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。服务器忙，请重新选择！'); window.location.href='{0}';</script>", "jfdh"));
                        Response.Write("<script>document.location=document.location;</script>");
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 购买卡
        /// </summary>
        /// <returns></returns>
        public ActionResult recharge()
        {
            // 从选择手机新用户和PC老用户  页面跳过来，1是新用户，0 是未选择手机新用户
            string id = string.Empty;
            try
            {
                id = Request.QueryString["id"].ToString();
            }
            catch
            {
                id = "0";
            }

            string cpnumber = string.Empty;
            if (Request.Cookies["userName"] != null)
            {
                cpnumber = Request.Cookies["userName"].Value;
            }

            if (string.IsNullOrEmpty(cpnumber))
            {
                Response.Cookies["pageType"].Value = "3";
                Response.Cookies["pageType"].Expires = DateTime.Now.AddHours(1);
                Response.Redirect("login");
            }
            else
            {
                string userType = string.Empty;
                if (Request.Cookies["userType"] != null)
                {
                    userType = Request.Cookies["userType"].Value;
                }

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                if (id == "1")
                {
                    ds = GetProduct(cpnumber, 0, 1);
                    bindProduct(ds);
                }
                else
                {
                    if (userType == "0")//手机版老用户
                    {
                        ds = GetProduct(cpnumber, 0, 1);
                        bindProduct(ds);
                    }
                    else if (userType == "2")//pc验证过老用户
                    {
                        ds = GetProduct(cpnumber, 1, 1);
                        bindProduct(ds);
                    }
                    else if (userType == "3")//pc验证过新用户
                    {
                        ds = GetProduct(cpnumber, 0, 1);
                        bindProduct(ds);
                    }
                    else//新用户且未经过pc验证
                    {
                        Response.Redirect("userLoginPCorPhone");
                    }
                }
            }
            return View();
        }

        //绑定产品
        public void bindProduct(DataSet ds)
        {
            if (ds != null)
            {
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                string listStr = "<a class=\"proitem \" href=\"buy?id=" + dt.Rows[0][0].ToString() + "\"><span class=\"p_i\">" + dt.Rows[0][1].ToString()
                    + " / " + "<span class=\"pprice\">" + dt.Rows[0][3].ToString().Replace(".00", "") + "</span>元</span></a>";

                StringBuilder build = new StringBuilder();
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    build.Append("<li><a href=\"buy?id=" + dt.Rows[i][0].ToString() + "\" class=\"proitem\"><span class=\"p_i\">");
                    build.Append(dt.Rows[i][1].ToString());
                    build.Append("/");
                    build.Append("<span class=\"pprice\">" + dt.Rows[i][3].ToString().Replace(".00", "") + "</span>");
                    build.Append("元</span></a></li>");
                }
                string listStr2 = Convert.ToString(build);
                ViewBag.listStr = listStr;
                ViewBag.listStr2 = listStr2;
            }
        }

        //绑定到期日数据
        public void bind(string cpnumber, string pwd)
        {
            DataSet ds = wbs.Get_Enddate(cpnumber, pwd);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string cupnumber = cpnumber.Replace(cpnumber.Substring(4, 7).ToString(), "****");
                DataTable dt = new DataTable(); 
                dt = ds.Tables[0];

                if (dt.Rows.Count == 1)
                {
                    if (dt.Rows[0][0].ToString() == "主力版")
                    {
                        DateTime dtime = DateTime.Parse(dt.Rows[0][1].ToString());
                        ViewBag.quote1 = string.Format("{0:yyyy-MM-dd}", dtime);
                        ViewBag.quote2 = "未使用该产品！";
                    }
                    else
                    {
                        DateTime dtime2 = DateTime.Parse(dt.Rows[0][1].ToString());
                        ViewBag.quote2 = string.Format("{0:yyyy-MM-dd}", dtime2);
                        ViewBag.quote1 = "未使用该产品！";
                    }
                }
                else if (dt.Rows.Count == 2)
                {
                    DateTime dtime = DateTime.Parse(dt.Rows[0][1].ToString());
                    ViewBag.quote2 = string.Format("{0:yyyy-MM-dd}", dtime);

                    DateTime dtime2 = DateTime.Parse(dt.Rows[1][1].ToString());
                    ViewBag.quote1 = string.Format("{0:yyyy-MM-dd}", dtime2);
                }
                else
                {
                    ViewBag.quote1 = "未使用该产品！";
                    ViewBag.quote2 = "未使用该产品！";
                }
            }

            else
            {
                ViewBag.quote = "<center>当前数据位空！</center>";
            }
        }

        //@cpNumber varchar(50),--手机号码
        //@flag int,--请求标记,1:有rtx信息 0：无rtx信息(pc验证通过就传1，其它传0)
        //@retCode int output,--返回错误码
        //@retMsg varchar(255) output,--返回信息
        //@versionFlag int = 0--0:老版本,1：新版本 (传1,新版本有小额支付产品)
        public DataSet GetProduct(string cpnumber, int flag, int versionFlag)
        {
            DataSet ds = new DataSet();
            mobilePay.mobilePay mp = new mobilePay.mobilePay();
            ds = mp.GetProductList(cpnumber, flag, versionFlag);
            return ds;
        }

        //积分兑换产品列表
        public DataSet Get_productList()
        {
            string Source = "server=172.28.2.209;database=emweb_new;uid=ym_web;password=web20091021;Connection Reset=FALSE";
            SqlConnection con = new SqlConnection(Source);
            SqlCommand myCommand = new SqlCommand("baike_Get_product", con);
            myCommand.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(myCommand);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            con.Close();
            con.Dispose();
            return ds;
        }

        /// <summary>
        /// 我要优惠
        /// </summary>
        /// <returns></returns>
        public ActionResult wyyh()
        {
            return View();
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <returns></returns>
        public ActionResult buy()
        {

            string id = this.Request.QueryString["id"].ToString();
            string cpnumber = string.Empty;
            if (Request.Cookies["userName"] != null)
            {
                cpnumber = Request.Cookies["userName"].Value;
            }
            else
            {
                Response.Cookies["pageType"].Value = "3";
                Response.Cookies["pageType"].Expires = DateTime.Now.AddHours(1);
                Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。请先登录！'); window.location.href='{0}';</script>", "login"));
                Response.Write("<script>document.location=document.location;</script>");
            }

            string cpnumberxx = RSAEncryption(cpnumber);
            // Sys.Utility.Loger.Error("cpnumberxx" + cpnumberxx);
            string cp = HttpUtility.UrlEncode(cpnumberxx);

            string zhifu = "<a href=\"http://m.emoney.cn/WxZhiFu/index.aspx?type=1&id=" + id + "&phone=" + HttpUtility.UrlEncode(cpnumberxx) + "\">";
            string cft = "<a href=\"http://m.emoney.cn/WxZhiFu/index.aspx?type=2&id=" + id + "&phone=" + HttpUtility.UrlEncode(cpnumberxx) + "\">";
            //string zhifu = "<a href=\"http://localhost:18166/CSharpForRSA/index.aspx?type=1&id=" + id + "&phone=" + HttpUtility.UrlEncode(cpnumberxx) + "\">";
            //string cft = "<a href=\"http://localhost:18166/CSharpForRSA/index.aspx?type=2&id=" + id + "&phone=" + HttpUtility.UrlEncode(cpnumberxx) + "\">";

            ViewBag.zhifu = zhifu;
            ViewBag.cft = cft;

            return View();
        }

        /// <summary>
        /// 手机号加密
        /// </summary>
        /// <param name="originString"></param>
        /// <returns></returns>
        private string RSAEncryption(string cpnumber)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string keyPath = Server.MapPath("key.pem").Replace("\\Ywcx", "");
            string strKey = System.IO.File.ReadAllText(keyPath);
            strKey = strKey.Replace("-----BEGIN RSA PRIVATE KEY-----", "");
            strKey = strKey.Replace("-----END RSA PRIVATE KEY-----", "");
            rsa.ImportCspBlob(Convert.FromBase64String(strKey));

            byte[] plaindata = System.Text.Encoding.Default.GetBytes(cpnumber);
            byte[] encryptdata = rsa.Encrypt(plaindata, false);
            string encryptstring = Convert.ToBase64String(encryptdata);
            return encryptstring;
        }

        //----------------------------------------------------------------------------------------
        // 五个子首页新定
        //----------------------------------------------------------------------------------------

        //功能介绍
        public ActionResult csse()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        //实战技巧
        public ActionResult szjq()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 密码查询
        /// </summary>
        /// <returns></returns>
        public ActionResult mc()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 查看软件使用到期日
        /// </summary>
        /// <returns></returns>
        public ActionResult dq()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 积分服务
        /// </summary>
        /// <returns></returns>
        public ActionResult jf()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 在线购买
        /// </summary>
        /// <returns></returns>
        public ActionResult zg()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 软件下载
        /// </summary>
        /// <returns></returns>
        public ActionResult rx()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 新定 登录界面 
        /// </summary>
        /// <returns></returns>
        public ActionResult login()
        {
            return View();
        }

        /// <summary>
        /// 推荐好友
        /// </summary>
        /// <returns></returns>
        public ActionResult tjhy()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 推荐好友二级内容
        /// </summary>
        /// <returns></returns>
        public ActionResult tjhy2()
        {
            return View();
        }

        /// <summary>
        /// 推荐好友3级内容
        /// </summary>
        /// <returns></returns>
        public ActionResult tjhy3()
        {
            return View();
        }

        /// <summary>
        /// 推荐好友控制器
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult StockPhone(FormCollection fc)
        {
            if (!string.IsNullOrEmpty(fc["action"].ToString()))
            {
                string phoneOld = fc["txt_phoneOld"].ToString();
                string phoneNew = fc["txt_phoneNew"].ToString();
                if (string.IsNullOrEmpty(phoneOld) || string.IsNullOrEmpty(phoneNew))
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。手机号或密码不能为空，请从新填写！'); window.location.href='{0}';</script>", "login"));
                    Response.Write("<script>document.location=document.location;</script>");
                }
                else
                {
                    mobilewbs.mobilewbs mb = new mobilewbs.mobilewbs();
                    string retStr = mb.sp_add_mobile_referral(phoneOld, phoneNew);
                    string[] arry = retStr.Split('|');
                    if (Convert.ToInt32(arry[1]) == 0)
                    {
                        return RedirectToAction("tjhy3");
                    }
                    else
                    {
                        Response.Write(String.Format("<script type='text/javascript'>alert('" + arry[0] + "'); window.location.href='{0}';</script>", "tjhy2"));
                        Response.Write("<script>document.location=document.location;</script>");
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 意见反馈
        /// </summary>
        /// <returns></returns>
        public ActionResult yjfk()
        {
            ViewBag.quote = Get_DateTime();
            return View();
        }

        /// <summary>
        /// 意见反馈2
        /// </summary>
        /// <returns></returns>
        public ActionResult yjfk2()
        {
            return View();
        }

        /// <summary>
        /// 意见反馈提交表单处理
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult StockYk(FormCollection fc)
        {
            if (!string.IsNullOrEmpty(fc["action"].ToString()))
            {
                string type = fc["name1"].ToString();
                string content = fc["txt_content"].ToString();
                string phone = fc["txt_phone"].ToString();
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(phone))
                {
                    Response.Write(String.Format("<script type='text/javascript'>alert('尊敬的用户，您好。手机号或密码不能为空，请从新填写！'); window.location.href='{0}';</script>", "login"));
                    Response.Write("<script>document.location=document.location;</script>");
                }
                else
                {

                    mobilewbs.mobilewbs mb = new mobilewbs.mobilewbs();
                    string retStr = mb.InsertBack(phone, Convert.ToInt32(type), content, "6");
                    Response.Write(String.Format("<script type='text/javascript'>alert('" + retStr + "'); window.location.href='{0}';</script>", "yjfk2"));
                    Response.Write("<script>document.location=document.location;</script>");
                }
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        public string Get_DateTime()
        {
            DateTime dtime = DateTime.Now;
            string dateStr = string.Format("{0:MM-dd}", dtime);
            return dateStr.Replace("-", "月") + "日";
        }
    }
}
