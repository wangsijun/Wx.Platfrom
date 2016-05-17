using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Utility
{
    public class SysConstUtility
    {
        #region 缓存键值

        #endregion
        #region 错误原因分类
        public const string ErrorReason_Other = "Other";
        public const string ErrorReason_Error = "Error";
        public const string ErrorReason_NoRecord = "NoRecord";//记录已不存在
        public const string ErrorReason_Reference = "Reference";//记录已被引用
        #endregion
        #region MaskType
        public const string MaskType_Number = "Number";
        public const string MaskType_Word = "Word";
        public const string MaskType_Date = "Date";
        public const string MaskType_Regex = "Regex";
        public const string MaskType_Int = "Int";
        public const string MaskType_Email = "Email";
        #endregion

        #region 日期格式
        /// <summary>
        /// MM月dd日 HH:mm
        /// </summary>
        public const string Month_Day_Hour_Minute = "MM月dd日 HH:mm";
        /// <summary>
        /// {0:MM月dd日 HH:mm}
        /// </summary>
        public const string Month_Day_Hour_MinuteParam = "{0:MM月dd日 HH:mm}";
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string FullTime = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// {0:yyyy-MM-dd HH:mm:ss}
        /// </summary>
        public const string FullTimeParam = "{0:yyyy-MM-dd HH:mm:ss}";
        #endregion
        #region 小数格式
        /// <summary>
        /// 两位小数
        /// </summary>
        public const string Dec_2 = "#.##";
        #endregion
    }
}
