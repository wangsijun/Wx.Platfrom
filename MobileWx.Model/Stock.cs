using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Model
{
    public class Stock
    {
        public string s
        {
            get;
            set;
        }
        public string n
        {
            get;
            set;
        }
        public string c
        {
            get;
            set;
        }
        [JsonIgnore]
        public string s7
        {
            get {
                return (s.StartsWith("6") ? "0" : "1") + s;
            }
            set { }
        }
    }
}