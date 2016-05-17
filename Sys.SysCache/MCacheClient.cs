using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Memcached.ClientLibrary;

namespace Sys.SysCache
{
    public class MemcachedClientList : Dictionary<string, MemcachedClient>
    {
        #region 基础设置
        public void SetDefaultEncoding(string DefaultEncoding)
        {
            foreach (MemcachedClient c in this.Values)
            {
                c.DefaultEncoding = DefaultEncoding;
            }
        }
        public void SetEnableCompression(bool EnableCompression)
        {
            foreach (MemcachedClient c in this.Values)
            {
                c.EnableCompression = EnableCompression;
            }
        }
        public void SetPrimitiveAsString(bool PrimitiveAsString)
        {
            foreach (MemcachedClient c in this.Values)
            {
                c.PrimitiveAsString = PrimitiveAsString;
            }
        }
        #endregion

        #region 方法
        public bool KeyExistsAny(string key)
        {
            foreach (MemcachedClient c in this.Values)
            {
                if (c.KeyExists(key)) return true;
            }
            return false;
        }
        public bool KeyExistsEach(string key)
        {
            foreach (MemcachedClient c in this.Values)
            {
                if (!c.KeyExists(key)) return false;
            }
            return true;
        }
        public void Set(string key, object obj, DateTime? expiry = null)
        {
            if (expiry == null)
            {
                foreach (MemcachedClient c in this.Values)
                {
                    if (c.KeyExists(key))
                    {
                        c.Set(key, obj);
                    }
                    else
                    {
                        c.Add(key, obj);
                    }
                }
            }
            else
            {
                foreach (MemcachedClient c in this.Values)
                {
                    if (c.KeyExists(key))
                    {
                        c.Set(key, obj, expiry.Value);
                    }
                    else
                    {
                        c.Add(key, obj, expiry.Value);
                    }
                }
            }
        }
        public void Delete(string key)
        {
            foreach (MemcachedClient c in this.Values)
            {
                c.Delete(key);
            }
        }
        public string Get(string key)
        {
            foreach (MemcachedClient c in this.Values)
            {
                if (c.KeyExists(key)) return (string)c.Get(key);
            }
            return null;
        }
        public T Get<T>(string key)
        {
            foreach (MemcachedClient c in this.Values)
            {
                if (c.KeyExists(key)) return (T)c.Get(key);
            }
            return default(T);
        }
        public object[] GetMultipleArray(string[] keys)
        {
            foreach (MemcachedClient c in this.Values)
            {
                if (c.Get(MCacheClient.keyTest) == null) continue;
                object[] o = c.GetMultipleArray(keys);
                return o;
            }
            return null;
        }
        public void FlushAll()
        {
            foreach (MemcachedClient c in this.Values)
            {
                c.FlushAll();
            }
        }
        #endregion
    }
    public class MCacheClient
    {
        public const string keyTest = "keyTest";
        public MCacheClient()
        {
            DefaultEncoding = "utf-8";
        }
        private void Init()
        {
            if (string.IsNullOrEmpty(Servers))
            {
                throw new Exception("未指定Servers");
            }

            client = new MemcachedClientList();
            String[] serversList = Servers.Split(';');
            foreach (string s in serversList)
            {
                String[] serverlist = s.Split(',');
                SockIOPool pooltemp = SockIOPool.GetInstance(string.IsNullOrEmpty(PoolName)?s:PoolName);
                pooltemp.SocketTimeout = 10000;
                pooltemp.SocketConnectTimeout = 5000;
                pooltemp.SetServers(serverlist);
                pooltemp.Initialize();
                client[s] = new MemcachedClient();
                client[s].PoolName = (string.IsNullOrEmpty(PoolName) ? s : PoolName);  
                //client[s].Set(MCacheClient.keyTest, "valueTest");
            }
            client.SetDefaultEncoding(DefaultEncoding);
            client.SetEnableCompression(false);
            client.SetPrimitiveAsString(true);
        }
        private MemcachedClientList client = null;
        public MemcachedClientList Client
        {
            get
            {
                if (client == null) Init();
                return client;
            }
        }
        public string DefaultEncoding
        {
            get;
            set;
        }
        /// <summary>
        /// 分号分隔，分号间互备（逗号间负载）
        /// </summary>
        public string Servers
        {
            get;
            set;
        }
        public string PoolName
        {
            get;
            set;
        }
        /// <summary>
        /// 添加一个缓存
        /// </summary>
        /// <param name="obj">缓存对象</param>
        /// <param name="sj">缓存时间</param>
        public void Set(string key, object obj, DateTime? expiry = null)
        {
            Client.Set(key, obj, expiry);
        }

        public void Delete(string key)
        {
            Client.Delete(key);
        }
        public string Get(string key)
        {
            return Client.Get(key);
        }
        public object[] GetMultipleArray(string[] keys)
        {
            return Client.GetMultipleArray(keys);
        }
        public Dictionary<string, T> GetMultiple<T>(string[] keys, bool removeNull = false)
        {
            Dictionary<string, T> rtn = new Dictionary<string, T>();
            object[] objs = Client.GetMultipleArray(keys);
            if (objs == null) return rtn;
            for (int i = 0; i < keys.Length; i++)
            {
                if (removeNull && objs[i] == null) continue;
                rtn[keys[i]] = (T)objs[i];
            }
            return rtn;
        }
        public T Get<T>(string key)
        {
            return Client.Get<T>(key);
        }
        public bool KeyExistsAny(string key)
        {
            return Client.KeyExistsAny(key);
        }
        public bool KeyExistsEach(string key)
        {
            return Client.KeyExistsEach(key);
        }
        public void FlushAll()
        {
            Client.FlushAll();
            Init();
        }
    }
}
