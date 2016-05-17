using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileWx.Dal;
using MobileWx.Bll;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class Test_SubscribeUser
    {
        [TestMethod]
        public void DalSave()
        {
            DalSubscribUser.Get().Save(new MobileWx.Model.WxSubscribeUser() { subscribetime = DateTime.Now, openid = "12121212112" });
        }


        [TestMethod]
        public void GetSubscribeUsers()
        {
            List<string> users = BllWxBase.Get().GetSubscribeUsers("EMONEY");

        }

        [TestMethod]
        public void GetAccessToken()
        {
            string token = new DalWx().GetAccessToken();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(token));

        }

        [TestMethod]
        public void Test_CheckBindCode()
        {
            long bindid = 0;
            bool result = BllSubscribeUser.Get().CheckBindCode("EM20000000041", out bindid);

            Assert.IsTrue(result);

        }

        [TestMethod]
        public void Test_BindProUser()
        {
            long bindid = 0;
            BllSubscribeUser.Get().CheckBindCode("EM20000000041", out bindid);
            bool result = BllSubscribeUser.Get().BindProUser("oyfqejkNQyZJ1hmOijmhQJqikXZk", bindid);

            Assert.IsTrue(result);
        }


    }
}
