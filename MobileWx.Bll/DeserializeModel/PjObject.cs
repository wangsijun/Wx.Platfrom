using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Bll.DeserializeModel
{
    /// <summary>
    /// 评级反序列化Model
    /// </summary>
    public class PjObject
    {
        public int success { get; set; }
        public string pj { get; set; }
        public string r { get; set; }
        public string rd { get; set; }
        public string d { get; set; }
        public List[] list { get; set; }
        public class List
        {
            public string t { get; set; }
            public float s { get; set; }
            public string c { get; set; }
        }
    }
}
