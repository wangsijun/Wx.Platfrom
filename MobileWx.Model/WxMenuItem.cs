using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{
    [Serializable]
    public class WxMenuItem
    {
        public string type
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public string key
        {
            get;
            set;
        }
        public string url
        {
            get;
            set;
        }
        public List<WxMenuItem> sub_button
        {
            get;
            set;
        }
    }
}
