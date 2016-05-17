using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace MobileWx.Model
{
     [Serializable]
    public class SysResult
    {
        public SysResult()
        {
            rows = new ArrayList();
        }
        public int? total
        {
            get;
            set;
        }
        public string idx
        {
            get;
            set;
        }

        public IList rows
        {
            get;
            set;
        }
        public string msg
        {
            get;
            set;
        }
        public bool hasObject()
        {
            if (rows == null) return false;
            return rows.Count > 0 ? true : false;
        }
        public void SetReturnObject(ModelBase obj)
        {
            rows = new ArrayList() { obj };
        }
    }


     public class WsAttribute : Attribute
     { 
        
     }
}
