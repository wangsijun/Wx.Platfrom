using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileWx.Dal;
using MobileWx.Model;
using System.Data;
using Sys.Utility;
namespace MobileWx.Bll
{
    public class BllNewsTab:BllBase
    {
        private static  BllNewsTab _me;
        private DalNewsTab _dalnewtab = new DalNewsTab();
        public static BllNewsTab Get()
        {
            if (_me == null) _me = new BllNewsTab();
            return _me;
        }

        public ModelNewsTab getById(int? id)
        {
            DataSet ds = _dalnewtab.getById(id);
            if (ds == null) return null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ModelNewsTab obj = new ModelNewsTab();
                obj.id = StringUtility.ToInt64(dr["n_id"]);
                obj.title = StringUtility.ToString(dr["n_title"]);
                obj.imgUrl = StringUtility.ToString(dr["imgUrl"]);
                obj.corSymbol = StringUtility.ToString(dr["corSymbol"]);
                obj.content = StringUtility.ToString(dr["n_content"]);
                obj.createDate = StringUtility.ToDateTime(dr["n_createDate"]);
                return obj;
            }
            return null;
        }
        public List<ModelNewsTab> GetZXYWList()
        { 
            List<ModelNewsTab> rtn = new List<ModelNewsTab>();
            DataSet ds = _dalnewtab.GetZXYWList();
            if (ds == null) return rtn;
            for (int i = 0; i < ds.Tables.Count;i++ )
            {
                foreach (DataRow dr in ds.Tables[i].Rows)
                {
                    ModelNewsTab obj = new ModelNewsTab();
                    obj.id = StringUtility.ToInt64(dr["n_id"]);
                    obj.title = StringUtility.ToString(dr["n_title"]);
                    obj.imgUrl = StringUtility.ToString(dr["imgUrl"]);
                    obj.corSymbol = StringUtility.ToString(dr["corSymbol"]);
                    obj.subClass = i.ToString();
                    rtn.Add(obj);
                }
            }
            return rtn;
        }
        public ModelNewsTab GetDaShi()
        {
            DataSet ds = _dalnewtab.GetDaShi();
            if (ds == null) return null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ModelNewsTab obj = new ModelNewsTab();
                obj.id = StringUtility.ToInt64(dr["n_id"]);
                obj.title = StringUtility.ToString(dr["n_title"]);
                obj.imgUrl = StringUtility.ToString(dr["imgUrl"]) + "?t=" + DateTime.Now.ToString("yyMMddHHmmss");
                obj.content = StringUtility.ToString(dr["n_content"]);
                obj.createDate = StringUtility.ToDateTime(dr["n_createdate"]);
                return obj;
            }
            return null;
        }


    }
}
