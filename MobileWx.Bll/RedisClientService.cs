using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using ServiceStack.Redis.Support;

namespace MobileWx.Bll
{
    public class RedisClientService
    {
        private static RedisClientService _instance = new RedisClientService();
        public static RedisClientService Instance
        {
            get
            {
                return _instance;
            }
        }

        private RedisClientService()
        {
        }

        private PooledRedisClientManager _manager = null;
        public PooledRedisClientManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new PooledRedisClientManager(System.Configuration.ConfigurationManager.AppSettings["RedisServer"]);
                }
                return _manager;
            }
        }

        #region
        public T GetObj<T>(string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.Get<T>(key);
            }
        }

        public string Get(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] b = mg.HGet(hashid, key.ToUtf8Bytes());
                if (b == null)
                    return null;
                return Encoding.UTF8.GetString(b);
            }
        }

        public int Set(string hashid, string key, string val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HSet(hashid, key.ToUtf8Bytes(), val.ToUtf8Bytes());
            }
        }

        public bool SetObj<T>(string key, T val, TimeSpan? expire = null)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                if (expire == null)
                {
                    return mg.Set<T>(key, val);
                }
                else
                {
                    return mg.Set<T>(key, val, expire.Value);
                }
            }
        }

        public int Del(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.Del(hashid);
            }
        }
        #endregion

        #region Hash
        public int HSet<T>(string hashid, string key, T val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] value = new ObjectSerializer().Serialize(val);

                return mg.HSet(hashid, key.ToUtf8Bytes(), value);
            }
        }

        public bool JsonSet<T>(string hashid, string key, T val)
        {
            string json = "";

            using (var mg = (RedisClient)Manager.GetClient())
            {

                if (val != null)
                    json = val.ToJson();

                return mg.SetEntryInHash(hashid, key, json);
            }

        }

        /// <summary>
        /// 获取hash集合，值为json
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="hashid">hash</param>
        /// <returns>值列表</returns>
        public List<T> JsonGetAll<T>(string hashid)
        {
            List<T> result = new List<T>();

            using (var mg = (RedisClient)Manager.GetClient())
            {
                List<String> jsonvals = mg.GetHashValues(hashid);

                if (result != null)
                {
                    foreach (string jsonval in jsonvals)
                    {
                        T obj = jsonval.FromJson<T>();
                        if (obj != null)
                            result.Add(obj);
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// Json的方式保存值，保存一个列表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="hashid">hash</param>
        /// <param name="vals">值</param>
        /// <param name="keyHandler">key值选取处理</param>
        /// <returns>成功保存的行数</returns>
        public int JsonSetAll<T>(string hashid, IList<T> vals, Func<T, string> keyHandler)
        {
            //受影响的行数
            int rows = 0;
            if (vals != null && vals.Count > 0)
            {
                foreach (T item in vals)
                {
                    string key = keyHandler(item);

                    bool result = JsonSet<T>(hashid, key, item);

                    if (result)
                    {
                        rows++;
                    }
                }
            }

            return rows;
        }

        public T JsonGet<T>(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                string jsonval = mg.GetValueFromHash(hashid, key);

                if (!string.IsNullOrEmpty(jsonval))
                {
                    T obj = jsonval.FromJson<T>();
                    if (obj != null)
                        return obj;
                }
            }

            return default(T);

        }



        public List<T> HGetAll<T>(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[][] arr = mg.HGetAll(hashid);
                List<T> result = new List<T>();
                if (arr == null)
                    return result;
                for (int i = 0; i < arr.Count(); i += 2)
                {
                    if (arr[i] != null)
                    {
                        byte[] data = arr[i + 1];
                        ObjectSerializer or = new ObjectSerializer();
                        var item = or.Deserialize(data);

                        if (item != null && item is T)
                            result.Add((T)item);

                    }
                }
                return result;
            }
        }

        public Dictionary<string, T> HGetDict<T>(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[][] arr = mg.HGetAll(hashid);
                Dictionary<string, T> result = new Dictionary<string, T>();
                if (arr == null)
                    return result;
                for (int i = 0; i < arr.Count(); i += 2)
                {
                    if (arr[i] != null)
                    {
                        byte[] data = arr[i + 1];
                        ObjectSerializer or = new ObjectSerializer();
                        var item = or.Deserialize(data);

                        if (item != null && item is T)
                            result.Add(arr[i].FromUtf8Bytes(), (T)item);

                    }
                }
                return result;
            }
        }

        public T HGet<T>(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] arr = mg.HGet(hashid, key.ToUtf8Bytes());
                if (arr != null)
                {
                    ObjectSerializer ser = new ObjectSerializer();
                    var obj = ser.Deserialize(arr);
                    if (obj != null)
                        return (T)obj;
                }
                return default(T);
            }
        }


        public int HDel(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HDel(hashid, key.ToUtf8Bytes());
            }
        }




        public bool JsonDel(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.RemoveEntryFromHash(hashid, key);
            }
        }
        #endregion





        public byte[][] HVals(string hashId)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HVals(hashId);
            }
        }
    }
}
