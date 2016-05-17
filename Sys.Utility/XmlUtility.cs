using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace Sys.Utility
{
   public class XmlUtility
    {
        public static T GetObjConfigure<T>(string key)
        {
            return (T)ConfigurationManager.GetSection(key);
        }
        public static string GetValueConfigure(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
        /// <summary>
        /// 根据xml文档路径（不含根目录的路径）和xpath返回节点
        /// </summary>
        /// <param name="fileName">xml文档路径（不含根目录的路径）</param>
        /// <param name="xpath">NodePath</param>
        /// <returns></returns>
        public static XmlNode GetItem(string fileName, string xpath)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlNode xmlNod = null;

            XmlDoc.Load(StringUtility.AppPath + fileName);
            xmlNod = XmlDoc.SelectSingleNode(xpath);
            return xmlNod;
        }
        public static string Serialize(object obj)
        {
            return Serialize(obj, true);
        }
        public static string Serialize(object obj, bool removeHead)
        {
            if (obj == null) return "";
            StringBuilder s = new StringBuilder();
            StringWriter sw = new StringWriter(s);
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(String.Empty, String.Empty);
            xs.Serialize(sw, obj, xmlns);
            sw.Close();
            if (removeHead)s.Remove(0, s.ToString().IndexOf(">") + 1);
            return s.ToString().Trim();
        }
        public static T DeSerialize<T>(string str)
        {
            if (string.IsNullOrEmpty(str)) return default(T);
            StringBuilder s = new StringBuilder();
            StringReader sr = new StringReader(str);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            object o = xs.Deserialize(sr);
            sr.Close();
            return (T)o;
        }
    }
}
