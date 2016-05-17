using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Model
{
    public class ModelNewsTab:ModelBase
    {
        public string title
        {
            get;
            set;
        }
        public string content
        {
            get;
            set;
        }
        public string imgUrl
        {
            get;
            set;
        }
        public string subClass
        {
            get;
            set;
        }
        public string corSymbol
        {
            get;
            set;
        }
    }
}
