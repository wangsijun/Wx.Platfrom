using News_Issue.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MobileWx.Dal
{
    public class DalNewsTab:DalBase
    {
        //private static DalNewsTab _me;
        //public static DalNewsTab Get()
        //{
        //    if (_me == null) _me = new DalNewsTab();
        //    return _me;
        //}
        public DataSet GetDaShi()
        {
            List<SqlParameter> prms = new List<SqlParameter>()
            {

            };
            return SqlHelper.ExecuteDataset(SqlConnectString, CommandType.StoredProcedure, "sp_wx_getDaShi", prms.ToArray());
        }
        public DataSet GetZXYWList()
        {
            List<SqlParameter> prms = new List<SqlParameter>()
            {
               
            };
            return SqlHelper.ExecuteDataset(SqlConnectString, CommandType.StoredProcedure, "sp_wx_getZiXun", prms.ToArray());
        }
        public DataSet getById(int? id)
        {
            List<SqlParameter> prms = new List<SqlParameter>()
            {
                new SqlParameter("@id",id)
            };
            return SqlHelper.ExecuteDataset(SqlConnectString, CommandType.StoredProcedure, "sp_newsTab_getById", prms.ToArray());
        }
    }
}
