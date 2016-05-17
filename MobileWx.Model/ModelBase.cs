using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;
namespace MobileWx.Model
{
    public class ModelBase
    {
        #region 时间格式常量
        /// <summary>
        /// yyyyMMdd
        /// </summary>
        public const string shortDate = "yyyyMMdd";
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string date = "yyyy-MM-dd";
        /// <summary>
        /// {0:yyyy-MM-dd}
        /// </summary>
        public const string date_param = "{0:yyyy-MM-dd}";
        /// <summary>
        /// yyyy-MM-dd HH:mm
        /// </summary>
        public const string date_HH_mm = "yyyy-MM-dd HH:mm";
        /// <summary>
        /// {0:yyyy-MM-dd HH:mm}
        /// </summary>
        public const string date_HH_mm_param = "{0:yyyy-MM-dd HH:mm}";
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string date_full = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss.ffff
        /// </summary>
        public const string date_full_ffff = "yyyy-MM-dd HH:mm:ss.ffff";
        /// <summary>
        /// {0:yyyy-MM-dd HH:mm:ss}
        /// </summary>
        public const string date_full_param = "{0:yyyy-MM-dd HH:mm:ss}";
        #endregion
        [XmlIgnore]
        public long? id
        {
            get;
            set;
        }
        [JsonProperty("d")]
        [XmlIgnore]
        public virtual DateTime? createDate
        {
            get;
            set;
        }
        [JsonProperty("ud")]
        [XmlIgnore]
        public virtual DateTime? updateDate
        {
            get;
            set;
        }
    }
}
