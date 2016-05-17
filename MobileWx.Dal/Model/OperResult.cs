using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileWx.Dal
{
    public class OperResult
    {
        public object Data { get; set; }

        public int Status { get; set; }

        public string Message { get; set; }
    }
}
