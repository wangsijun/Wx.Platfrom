using Dapper;
using MobileWx.Model;
using News_Issue.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MobileWx.Dal
{
    public class DalSubscribUser : DalBase
    {
        private static DalSubscribUser _me;
        public static DalSubscribUser Get()
        {
            if (_me == null)
                _me = new DalSubscribUser();
            return _me;
        }


        public void Save(WxSubscribeUser data)
        {
            var p = new DynamicParameters();

            p.Add("@Id", data.Id, dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("@subscribe", data.subscribe, dbType: DbType.Int32);
            p.Add("@openid", data.openid, dbType: DbType.AnsiString, size: 200);
            p.Add("@nickname", data.nickname, dbType: DbType.String, size: 50);
            p.Add("@sex", data.sex, dbType: DbType.Int32);
            p.Add("@city", data.city, dbType: DbType.AnsiString, size: 50);
            p.Add("@country", data.country, dbType: DbType.AnsiString, size: 50);
            p.Add("@province", data.province, dbType: DbType.AnsiString, size: 50);
            p.Add("@language", data.language, dbType: DbType.AnsiString, size: 50);
            p.Add("@headimgurl", data.headimgurl, dbType: DbType.AnsiString, size: 500);
            p.Add("@subscribetime", data.subscribetime, dbType: DbType.DateTime);
            using (var connection = new SqlConnection(SqlConnectString))
            {
                connection.Open();
                connection.Execute("sp_wx_subscribeuser_save", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void Delete(string openid)
        {
            SqlHelper.ExecuteNonQuery(SqlConnectString, CommandType.StoredProcedure, "sp_wx_subscribeuser_delete", new SqlParameter("@OpenId", openid));
        }

        public WxSubscribeUser GetSubscribeUser(string openid)
        {
            var p = new DynamicParameters();
            p.Add("@openid", openid, dbType: DbType.AnsiString, size: 200);

            using (var connection = new SqlConnection(SqlConnectString))
            {
                connection.Open();
                return connection.Query<WxSubscribeUser>("sp_wx_subscribeuser_get", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        /// <summary>
        /// 绑定加强版的用户
        /// </summary>
        /// <param name="openid">微信用户openid</param>
        /// <param name="userid">加强版的用户bindid</param>
        /// <returns></returns>
        public OperResult SubscribeUserBindProUser(string openid, long bindid)
        {
            using (var connection = new SqlConnection(conn_activity))
            {
                var p = new DynamicParameters();
                p.Add("@openid", openid, dbType: DbType.String);
                p.Add("@bindid", bindid, dbType: DbType.Int64);
                p.Add("@bindtype", 1, dbType: DbType.Int32);
                p.Add("@rtnMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                p.Add("@rtnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                connection.Execute("sp_weixin_userbind",
                    p, commandType: CommandType.StoredProcedure);

                int? returnVal = p.Get<int?>("@rtnVal");
                string message = p.Get<string>("@rtnMsg");


                return new OperResult() { Message = message, Status = returnVal ?? -999 };
            }
        }

        /// <summary>
        /// 检验bindid是否有效，返回绑定用户的用户名
        /// </summary>
        /// <param name="bindid">绑定id</param>
        /// <returns></returns>
        public OperResult CheckProUserBindid(long bindid)
        {
            using (var connection = new SqlConnection(conn_platform_db))
            {
                var p = new DynamicParameters();
                p.Add("@bindid", bindid, dbType: DbType.Int64);
                p.Add("@username", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                p.Add("@rtnMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                p.Add("@rtnVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                connection.Execute("sp_platform_newactivity_checkbindid",
                    p, commandType: CommandType.StoredProcedure);

                int? returnVal = p.Get<int?>("@rtnVal");
                string message = p.Get<string>("@rtnMsg");
                string username = p.Get<string>("@username");

                return new OperResult()
                {
                    Data = username,
                    Message = message,
                    Status = returnVal ?? -999
                };
            }
        }
    }
}
