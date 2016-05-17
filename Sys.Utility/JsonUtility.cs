using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Sys.Utility
{
   public  class JsonUtility
    {
        public static T DeserializeByNewton<T>(string json)
        {
            if (json == null || json.Trim() == "") return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static string SerializerByNewton(object obj, string dateFormat = SysConstUtility.FullTime)
        {
            if (obj == null) return "";
            JsonSerializerSettings js = new JsonSerializerSettings();
            js.NullValueHandling = NullValueHandling.Ignore;
            IsoDateTimeConverter t = new IsoDateTimeConverter();
            t.DateTimeFormat = dateFormat;
            js.Converters.Add(t);
            return JsonConvert.SerializeObject(obj,Formatting.None,js);
        }
        /// <summary>
        /// 深度克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject">源对象</param>
        /// <returns>返回目标对象</returns>
        public static T Clone<T>(object sourceObject)
        {
            string strJSON = SerializerByNewton(sourceObject);
            T objResult = DeserializeByNewton<T>(strJSON);
            return objResult;
        }
    }
}
