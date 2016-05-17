using MobileWx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Bll
{
    public interface IBllMenuResponse
    {
        void SetResponse(WxResponse resp,string menuKey);

        
    }
}
