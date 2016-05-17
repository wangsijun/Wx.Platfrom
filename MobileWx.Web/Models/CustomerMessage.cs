using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Web.Models
{
    public class CustomerMessage
    {
        public string touser { get; set; }
        public string msgtype { get; set; }
        public object text { get; set; }
        public object news { get; set; } 
    }

    public class articles
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string picurl { get; set; }
    }
}