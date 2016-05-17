using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Sys.Utility
{
    public class FileUtility
    {
        public static void Downfile(string filepath, string fileName)
        {
            Stream iStream = null;
            byte[] buffer = new Byte[10000];
            int length;
            long dataToRead;

            try
            {
                iStream = new FileStream(filepath, System.IO.FileMode.Open,
                            FileAccess.Read, FileShare.Read);
                dataToRead = iStream.Length;
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + Uri.EscapeUriString(fileName));

                while (dataToRead > 0)
                {
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
            }

        }

        public static string ReadText(string path)
        {
            string str = "";
            if (File.Exists(path))
            {
                TextReader strR = File.OpenText(path);
                str = strR.ReadToEnd();
                strR.Close();
            }
            return str;
        }
        public static string WriteText(string path, string str)
        {
            UTF8Encoding utf8 = new UTF8Encoding(false);
            return WriteText(path, str, utf8);
        }
        public static string PreAppendText(string path, string str)
        {
            string s = ReadText(path);
            s = str + s;
            UTF8Encoding utf8 = new UTF8Encoding(false);
            return WriteText(path, s, utf8);
        }
        public static string AppendText(string path, string str)
        {
            UTF8Encoding utf8 = new UTF8Encoding(false);
            return WriteText(path, str, utf8, true);
        }
        public static string WriteJson(string path, object obj)
        {
            UTF8Encoding utf8 = new UTF8Encoding(false);
            return WriteText(path, JsonUtility.SerializerByNewton(obj), utf8);
        }
        public static T ReadJson<T>(string path)
        {
            return JsonUtility.DeserializeByNewton<T>(ReadText(path));
        }
        public static string WriteText(string path, string str, Encoding encode, bool append = false)
        {
            try
            {
                if (!Directory.Exists(path.Substring(0, path.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("\\")));
                }
                StreamWriter strW = new StreamWriter(path, append, encode);
                strW.Write(str);
                strW.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        public static string FormatPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            if (path.IndexOf(":") != 1)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + path;
            }
            return path;
        }
    }
}
