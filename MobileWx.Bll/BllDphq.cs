using MobileWx.Model; 
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Bll
{
    public class BllDphq:BllBase,IBllMenuResponse
    {
        public void SetResponse(Model.WxResponse resp, string menuKey)
        {

            //resp.Content = "<table><tr><td>青岛啤酒</td><td>600600</td></tr><tr><td>浦发银行</td><td>600000</td></tr></table>";
        } 
    }
}
