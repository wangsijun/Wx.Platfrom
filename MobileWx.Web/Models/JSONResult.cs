using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileWx.Web.Models
{
    public class JSONResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "text/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {

                string rtn = SerializerByNewton(Data);
                Data = null;
                response.Write(rtn);
            }
        }

        public string dateformat { get; set; }

        /// <summary>
        /// 属性名称小写字母转换
        /// </summary>
        public bool IsLowerPropertyName { get; set; }

        public string[] ignoreproperties { get; set; }

        /// <summary>
        /// 重命名属性名map
        /// </summary>
        public Dictionary<string, string> NameMapping { get; set; }

        public string SerializerByNewton(object obj)
        {
            if (obj == null)
                return "";
            JsonSerializerSettings js = new JsonSerializerSettings();
            js.NullValueHandling = NullValueHandling.Ignore;
            js.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //忽略属性
            js.ContractResolver = new CustomResolver(ignoreproperties,
                IsLowerPropertyName, NameMapping);
            IsoDateTimeConverter t = new IsoDateTimeConverter();
            if (string.IsNullOrEmpty(dateformat))
            {
                dateformat = "yyyy-MM-dd HH:mm:ss";
            }
            t.DateTimeFormat = dateformat;
            js.Converters.Add(t);
            return JsonConvert.SerializeObject(obj, Formatting.None, js);
        }

        /// <summary>
        /// 所有属性名小写
        /// </summary>
        /// <returns></returns>
        public JSONResult Lower()
        {
            this.IsLowerPropertyName = true;
            return this;
        }

        /// <summary>
        /// 忽略属性名
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public JSONResult Ignore(params string[] names)
        {
            this.ignoreproperties = names;
            return this;
        }

        /// <summary>
        /// 所有的时间格式化
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public JSONResult SetDateFormat(string format)
        {
            this.dateformat = format;
            return this;
        }

        /// <summary>
        /// 重命名属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public JSONResult RenameProperty(string name, string replace)
        {
            if (NameMapping == null)
            {
                NameMapping = new Dictionary<string, string>();
            }

            if (!NameMapping.Keys.Contains(name))
            {
                NameMapping.Add(name, replace);
            }

            return this;
        }
    }

    public class CustomResolver : DefaultContractResolver
    {
        IEnumerable<string> lstExclude { get; set; }

        bool IsLowerPropertyName { get; set; }

        Dictionary<string, string> NameMapping { get; set; }

        public CustomResolver(IEnumerable<string> excludedProperties, bool islowername,
            Dictionary<string, string> mapping)
        {
            lstExclude = excludedProperties;
            this.IsLowerPropertyName = islowername;
            this.NameMapping = mapping;
        }

        protected override IList<JsonProperty> CreateProperties(Type type,
            MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            if (properties != null)
            {
                return SetAll(type, properties);
            }

            return new List<JsonProperty>();
        }



        private List<JsonProperty> SetAll(Type type, IList<JsonProperty> properties)
        {
            List<JsonProperty> result = new List<JsonProperty>();
            foreach (JsonProperty prop in properties)
            {
                if (!IsIgnorProperty(type, prop))
                {
                    LowerName(prop);
                    Rename(prop);
                    result.Add(prop);
                }
            }

            return result;

        }

        private bool IsIgnorProperty(Type type, JsonProperty item)
        {
            bool result = false;
            if (lstExclude != null && lstExclude.Count() > 0)
            {
                result = lstExclude.Contains<string>(item.PropertyName);

            }

            return result;
        }

        private void LowerName(JsonProperty item)
        {
            if (IsLowerPropertyName)
            {
                item.PropertyName = item.PropertyName.ToLower();
            }
        }

        private void Rename(JsonProperty item)
        {
            if (NameMapping != null
                && NameMapping.Keys.Contains(item.PropertyName))
            {
                item.PropertyName = NameMapping[item.PropertyName];
            }
        }
    }
}