using Dapper;
using MobileWx.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MobileWx.Dal
{
    public class DalMessage : DalBase
    {
        private static DalMessage _me;
        public static DalMessage Get()
        {
            if (_me == null) _me = new DalMessage();
            return _me;
        }

        public Message GetMessage(int id)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id, dbType: DbType.Int32);

            using (var connection = new SqlConnection(SqlConnectString))
            {
                return connection.Query<Message>("sp_wx_message_get", p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }

        public List<Message> GetList(int pageSize, int pageIndex, string where, out int rowCount)
        {
            var p = new DynamicParameters();
            p.Add("@Id", 0, dbType: DbType.Int32);
            p.Add("@pageSize", pageSize, dbType: DbType.Int32);
            p.Add("@pageIndex", pageIndex, dbType: DbType.Int32);
            p.Add("@where", where, dbType: DbType.String);
            p.Add("@RowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(SqlConnectString))
            {
                List<Message> result = connection.Query<Message>("sp_wx_message_get", p, commandType: CommandType.StoredProcedure).ToList();
                rowCount = p.Get<int>("@RowCount");
                return result;
            }
        }

        public void Save(Message data)
        {
            var p = new DynamicParameters();
            p.Add("@Id", data.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            p.Add("@Type", data.Type, dbType: DbType.Int32);
            p.Add("@Title", data.Title, dbType: DbType.String, size: 200);
            p.Add("@Summary", data.Summary, dbType: DbType.String, size: 500);
            p.Add("@Content", data.Content, dbType: DbType.String);
            p.Add("@PicName", data.PicName, dbType: DbType.AnsiString, size: 50);
            p.Add("@LinkUrl", data.LinkUrl, dbType: DbType.AnsiString, size: 500);

            using (var connection = new SqlConnection(SqlConnectString))
            {
                connection.Execute("sp_wx_message_save", p, commandType: CommandType.StoredProcedure);
                if (data.Id == 0)
                {
                    data.Id = p.Get<int>("@Id");
                }
            }
        }


    }
}
