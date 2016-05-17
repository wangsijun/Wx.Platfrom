using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using MobileWx.Dal.Model;
using Sys.Utility;

namespace MobileWx.Dal
{
    public class DalWx : DalBase
    {
        public string GetAccessToken()
        {
            var p = new DynamicParameters();
            p.Add("@config_name", "Access_Token", dbType: DbType.AnsiString, size: 100);

            using (var connection = new SqlConnection(SqlConnectString))
            {
                dynamic d =
                    connection.Query("sp_weixin_config_get", p, commandType: CommandType.StoredProcedure)
                        .FirstOrDefault();
                if (d != null)
                {
                    return d.config_value;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取用户等级及金币
        /// </summary> 
        /// <returns></returns>
        public DBCommoninfo platform_user_get_commoninfo_byweixin(string openId)
        {
            try
            {
                using (var connection = new SqlConnection(conn_platform218_db))
                {
                    var p = new DynamicParameters();

                    p.Add("@weixin", openId, dbType: DbType.String);
                    p.Add("@rtnMsg", size: 50, dbType: DbType.String, direction: ParameterDirection.Output);
                    p.Add("@returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                    var result = connection.Query<DBCommoninfo>("platform_user_get_commoninfo_byweixin", p, commandType: CommandType.StoredProcedure).ToList();

                    DBCommoninfo info = new DBCommoninfo();

                    if (result.Count > 0)
                    {
                        info.point = result[0].point;
                        info.userexp = result[0].userexp;
                        info.userlevel = result[0].userlevel;
                        info.account = result[0].account;
                        info.zxg_cur_pid = result[0].zxg_cur_pid;
                        info.zxg_customsID = result[0].zxg_customsID;
                        info.zxg_last_pid = result[0].zxg_last_pid;
                    }

                    return info;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return new DBCommoninfo();
            }
        }
        /// <summary>
        /// 获取用户兑换记录
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="searchtype"></param>
        /// <returns></returns>
        public List<DalUserExchangeLog> platform_user_get_exchange_byweixin(string openId)
        {
            using (var connection = new SqlConnection(conn_platform218_db))
            {
                var p = new DynamicParameters();

                p.Add("@weixin", openId, dbType: DbType.String);
                p.Add("@rtnMsg", size: 50, dbType: DbType.String, direction: ParameterDirection.Output);
                p.Add("@returnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                var result = connection.Query<DalUserExchangeLog>("platform_user_get_exchange_byweixin", p, commandType: CommandType.StoredProcedure).ToList();
                return result;
            } 
        }

        /// <summary>
        /// 绑定微信号
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="weixin">微信号</param>
        /// <param name="isbind">0是解绑，1是绑定</param>
        /// <returns></returns>
        public int sp_platform_user_bind_weixin(string userName, string pwd, string weixin, bool isbind)
        {
            using (var connection = new SqlConnection(conn_platform218_db))
            {

                var p = new DynamicParameters();
                p.Add("@username", userName);
                p.Add("@cpPasswd", pwd);
                p.Add("@weixin", weixin);
                p.Add("@isbind", isbind);
                p.Add("@rtnMsg", size: 1024, dbType: DbType.String, direction: ParameterDirection.Output);
                p.Add("@rtnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                connection.Execute("sp_platform_user_bind_weixin", p, commandType: CommandType.StoredProcedure);

                var b = p.Get<string>("@rtnMsg");
                int c = p.Get<int>("@rtnVal");
                return c;
            }
        }
    }
}
