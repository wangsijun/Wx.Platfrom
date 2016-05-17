using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spring.Context;
using Spring.Context.Support;

namespace Sys.Spring
{
    public class SysSpring
    {
        private static IApplicationContext _ctx;
        /// <summary>
        /// Spring.net 容器对象
        /// </summary>
        public static IApplicationContext SpringAppContext
        {
            get
            {
                if (_ctx == null)
                {
                    _ctx = ContextRegistry.GetContext();
                }
                return _ctx;
            }
        }

        public static object GetByName(string objName)
        {
            object result = SpringAppContext.GetObject(objName);
            return result;
        }
    }
}
