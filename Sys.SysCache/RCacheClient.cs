using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sys.Utility;
using System.Collections;
namespace Sys.SysCache
{
    public class RCacheClient
    {
       
        #region 属性
        public string servers
        {
            get;
            set;
        }
        public int hashCount
        {
            get;
            set;
        }
        public string hashIndex(object idx)
        {
            return Convert.ToInt64(idx) % hashCount+"";
        }
        /// <summary>
        /// 产品
        /// </summary>
        public Dictionary<int, string> products
        {
            get;
            set;
        } 
        #endregion
        public RCacheClient()
        {
            //servers = "localhost";//默认端口：6379
            servers = "172.28.3.82";
            hashCount = 1; 

        }
  
        private PooledRedisClientManager _Manager;
        protected PooledRedisClientManager Manager
        {
            get{if(_Manager==null)Init();
            return _Manager;}
            set{_Manager=value;}
            
        }
        protected void Init()
        {
            if (string.IsNullOrEmpty(servers)) return;
            _Manager=new PooledRedisClientManager(servers.Split(','), servers.Split(','));
        }
        #region hash 取值
        public string Get(string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] b = mg.Get(key);
                if (b == null) return null;

                return Encoding.UTF8.GetString(b);
            }
        }
        public T GetObj<T>(string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return ToObject<T>(mg.Get<byte[]>(key));
            }
        }
        public string Get(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] b = mg.HGet(hashid, key.ToUtf8Bytes());
                if (b == null) return null;
                return Encoding.UTF8.GetString(b);
            }
        }
        public bool HashContains(string hashid,string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HashContainsEntry(hashid, key);
            }
        }
        public T GetObj<T>(string hashid,string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return ToObject<T>(mg.HGet(hashid, key.ToUtf8Bytes()));
            }
        }
        public List<string> SearchKeys(string pattern)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.SearchKeys(pattern);
            }
        }
        public IDictionary<string,T> SearchVals<T>(string pattern)
        {
            List<string> keys = SearchKeys(pattern);
            if (keys == null || keys.Count == 0) return new Dictionary<string, T>();
            return GetAllObj<T>(keys);
        }
        public List<string> GetAllKeys()
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.GetAllKeys();
            }
        }
        public List<string> GetAllVals(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                List<string> rtn = new List<string>();
                byte[][] arr = mg.HVals(hashid);
                if (arr == null) return rtn;
                foreach (byte[] bytes in arr)
                {
                    if (bytes == null) continue;
                    rtn.Add(Encoding.UTF8.GetString(bytes));
                }
                return rtn;
            }
        }
        public List<string> GetAllKeys(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                List<string> rtn = new List<string>();
                byte[][] arr = mg.HKeys(hashid);
                if (arr == null) return rtn;
                foreach (byte[] bytes in arr)
                {
                    if (bytes == null) continue;
                    rtn.Add(Encoding.UTF8.GetString(bytes));
                }
                return rtn;
            }
        }
        public List<T> GetAllObj<T>(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return ToObjects<T>(mg.HVals(hashid));
            }
        }
        public IDictionary<string, T> GetAllObj<T>(IList<string> keys)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.GetAll<T>(keys);
            }
        }
        public Dictionary<string, string> GetAll(string hashid, IList<string> keys)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                Dictionary<string, string> rtn = new Dictionary<string, string>();
                if (keys == null || keys.Count == 0) return rtn;
                byte[][] arr = mg.HMGet(hashid, keys.ToArray().ToUtf8Bytes());
                if (arr == null) return rtn;
                for (int i = 0; i < keys.Count(); ++i)
                {
                    if (arr[i] == null) continue;
                    rtn[keys[i]] = Encoding.UTF8.GetString(arr[i]);
                }
                return rtn;
            }
        }
        public Dictionary<string, string> GetAll(string hashid,Dictionary<string, string> result=null)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {               
                byte[][] arr = mg.HGetAll(hashid);
                Dictionary<string, string> rtn = result??new Dictionary<string, string>();
                if (arr == null) return rtn;
                for (int i = 0; i < arr.Count(); ++i)
                {
                    if (i % 2 == 0)
                    { 
                        rtn[Encoding.UTF8.GetString(arr[i])]=(arr[i+1]==null)?null:Encoding.UTF8.GetString(arr[i+1]);
                    }
                }
                return rtn;
            }
        }
        public Dictionary<string,T> GetAllObj<T>(string hashid,IList<string> keys)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                Dictionary<string, T> rtn = new Dictionary<string, T>();
                if (keys == null || keys.Count == 0) return rtn;
                byte[][] arr = mg.HMGet(hashid, keys.ToArray().ToUtf8Bytes());
                if (arr == null) return rtn;
                for (int i = 0; i < keys.Count(); ++i)
                {
                    if (arr[i] == null) continue;
                    rtn[keys[i]] = ToObject<T>(arr[i]);
                }
                return rtn;
            }
        }
        #endregion

        #region hash 设值
        public bool Set(string key, string val, TimeSpan? ts = null)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                if (ts == null)
                {
                    return mg.Set<byte[]>(key, val.ToUtf8Bytes());
                }
                else
                {
                    return mg.Set<byte[]>(key, val.ToUtf8Bytes(), ts.Value);
                }
            }
        }
        public bool Set(string key, string val, long sec)
        {
            return Set(key, val, new TimeSpan(10 * 1000 * 1000 * sec));
        }

        public bool SetObj<T>(string key, T val, long sec)
        {
            return SetObj<T>(key, val, new TimeSpan(10*1000*1000 * sec));
        }
        public bool SetObj<T>(string key, T val,TimeSpan? ts=null)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                if (ts == null)
                {
                    return mg.Set<T>(key, val);
                }
                else
                {
                    return mg.Set<T>(key, val, ts.Value);
                }
            }
        }
        public void SetAllObj<T>(IDictionary<string,T> vals)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.SetAll<T>(vals);
            }
        }
        public int Set(string hashid,string key, string val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HSet(hashid, key.ToUtf8Bytes(), val.ToUtf8Bytes());
            }
        }
        public int SetObj<T>(string hashid, string key, T val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HSet(hashid, key.ToUtf8Bytes(), ToUtf8Byte(val));
            }
        }
        public int Del(string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.Del(key);
            }
        }
        public int Del(string[] key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.Del(key);
            }
        }
        public void DelList(string listid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.RemoveAllFromList(listid);
            }
        }
        public int HDel(string hashid, string key)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.HDel(hashid, key.ToUtf8Bytes());
            }
        }
        public void SetAll(string hashid, Dictionary<string, string> map)
        {
            if (map == null || map.Keys.Count == 0) return;
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.HMSet(hashid, map.Keys.ToArray().ToUtf8Bytes(), map.Values.ToArray().ToUtf8Bytes());
            }
        }
        public void SetAllObj<T>(string hashid,Dictionary<string, T> map)
        {
            if (map == null || map.Keys.Count == 0) return;
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.HMSet(hashid, map.Keys.ToArray().ToUtf8Bytes(), ToUtf8Bytes(map.Values.ToArray()));
            }
        }
        public int GetHashCount(string hashid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.GetHashCount(hashid);
            }
        }
        #endregion

        #region


        public int LPush(string listid,string val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.LPush(listid, val.ToUtf8Bytes());
            }
        }
        public int LLen(string listid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.LLen(listid);
            }
        }
        public string LPop(string listid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                byte[] b = mg.LPop(listid);
                if (b == null) return null;
                return Encoding.UTF8.GetString(b);
            }
        }
        public List<string> GetList(string listid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.GetAllItemsFromList(listid);
            }
        }
        public int SAdd(string setid, string val)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                return mg.SAdd(setid, val.ToUtf8Bytes());
            }
        }
        public void SMembers(string setid)
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.SMembers(setid);
            }
        }
        public void FlushAll()
        {
            using (var mg = (RedisClient)Manager.GetClient())
            {
                mg.FlushAll();
            }
        }
        public static byte[][] ToUtf8Bytes(IList arr)
        {
            BinaryFormatter bit = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                byte[][] rtn = new byte[arr.Count][];
                for (int i = 0; i < arr.Count; ++i)
                {
                    stream.Position = 0;
                    bit.Serialize(stream, arr[i]);
                    rtn[i] = stream.ToArray();
                    stream.Flush();
                }
                return rtn;
            }
           
        }
        public static byte[] ToUtf8Byte(object obj)
        {
            BinaryFormatter bit = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Position = 0;
                bit.Serialize(stream, obj);
                return stream.ToArray();
            }

        }
        public static T ToObject<T>(byte[] arr)
        {
            if (arr==null)return default(T);
            using (MemoryStream stream = new MemoryStream(arr))
            {
                stream.Position = 0;
                BinaryFormatter bit = new BinaryFormatter();
                return (T)bit.Deserialize(stream);
            }
        }
        public static List<T> ToObjects<T>(byte[][] arr)
        {
            List<T> rtn=  new List<T> ();
            if (arr == null) return rtn;
            foreach (byte[] bytes in arr)
            {
                if (bytes == null) continue;                
                rtn.Add(ToObject<T>(bytes));
            }
            return rtn;
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
        #endregion
    }
}
