using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Dal.Model
{
    public class DBCommoninfo
    {
        public int? point { get; set; }
        public int? userlevel { get; set; }
        public int? userexp { get; set; }
        public string account { get; set; }
        public Int64? bitmap_newapp { get; set; }
        public Int64? zxg_customsID { get; set; }
        public Int64? zxg_cur_pid { get; set; }
        public Int64? zxg_last_pid { get; set; }
    }
}
