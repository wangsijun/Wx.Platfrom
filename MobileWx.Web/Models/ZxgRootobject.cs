using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Web.Models
{
    public class ZxgRootobject
    {
        public int RtState { get; set; }
        public string Msg { get; set; }
        public Datum[] Data { get; set; }
        public long TimeSpan { get; set; }
        public class Datum
        {
            public string pid { get; set; }
            public string group { get; set; }
            public string stocks { get; set; }
            public string uid { get; set; }
            public long TimeSpan { get; set; }
        }
    }
}