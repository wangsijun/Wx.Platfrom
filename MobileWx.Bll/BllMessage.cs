using MobileWx.Model;
using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MobileWx.Bll
{
    public class BllMessage
    {
        const string HASH_WX_ONE_MESSAGE = "mobile_wx_onemessage";

        private static BllMessage _me;
        public new static BllMessage Get()
        {
            if (_me == null) _me = new BllMessage();
            return _me;
        }


        public void Save(Message doc)
        {
            Dal.DalMessage.Get().Save(doc);

            Message rmsg = RedisClientService.Instance.JsonGet<Message>(HASH_WX_ONE_MESSAGE, ((MessageTypeEnum)doc.Type).ToString());
            if (rmsg == null || rmsg.Id <= doc.Id)
            {
                RedisClientService.Instance.JsonSet<Message>(HASH_WX_ONE_MESSAGE, ((MessageTypeEnum)doc.Type).ToString(), doc);
            }
            RedisClientService.Instance.JsonSet<Message>(HASH_WX_ONE_MESSAGE, doc.Id.ToString(), doc);

        }

        public Message GetLatestMessage(MessageTypeEnum t)
        {
            Message msg = RedisClientService.Instance.JsonGet<Message>(HASH_WX_ONE_MESSAGE, t.ToString());
            if (msg == null)
            {
                int rowCount;
                string where = "where Type=" + (int)t;
                msg = Dal.DalMessage.Get().GetList(1, 0, where, out rowCount).FirstOrDefault();
                if (msg != null)
                {
                    RedisClientService.Instance.JsonSet<Message>(HASH_WX_ONE_MESSAGE, ((MessageTypeEnum)msg.Type).ToString(), msg);
                }
            }
            return msg;
        }

        public List<Message> GetLatestMessages(MessageTypeEnum t, int pageSize)
        {
            int rowCount;
            string where = "where Type=" + (int)t;
            List<Message> result = Dal.DalMessage.Get().GetList(pageSize, 0, where, out rowCount);
            return result;
        }

        public Message GetCachedMessage(int id)
        {
            Message msg = RedisClientService.Instance.JsonGet<Message>(HASH_WX_ONE_MESSAGE, id.ToString());
            if (msg == null)
            {
                msg = Dal.DalMessage.Get().GetMessage(id);
                if (msg != null)
                {
                    RedisClientService.Instance.JsonSet<Message>(HASH_WX_ONE_MESSAGE, id.ToString(), msg);
                }
            }
            return msg;
        }

        public Message GetMessage(int id)
        {
            return Dal.DalMessage.Get().GetMessage(id);
        }

        public List<Message> GetMessageList(int pageSize, int pageIndex, string where, out int rowcount)
        {
            return Dal.DalMessage.Get().GetList(pageSize, pageIndex, where, out rowcount);
        }
    }
}
