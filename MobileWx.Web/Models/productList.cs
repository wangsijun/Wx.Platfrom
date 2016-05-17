using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileWx.Web.Models
{
    public class productList
    {

        public string integral { get; set; }
        public List<productList2> dataList { get; set; }
    }
    public class productList2
    {
        public string imageUrl { get; set; }
        public string cardType { get; set; }
        public string cardName { get; set; }
        public string needIntegral { get; set; }
        public string surplus { get; set; }
    }
}