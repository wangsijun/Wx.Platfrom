using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MobileWx.Model
{
    [XmlRoot("xml")]
    public class WxRequest : ModelWx, IXmlSerializable
    {
        public string MsgId
        {
            get;
            set;
        }
        public string PicUrl
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Url
        {
            get;
            set;
        }
        public string Event
        {
            get;
            set;
        }
        public string EventKey
        {
            get;
            set;
        }
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {

            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.Read();
                    continue;
                }
                switch (reader.Name)
                {
                    case "ToUserName":
                        ToUserName = reader.ReadElementString();
                        break;
                    case "FromUserName":
                        FromUserName = reader.ReadElementString();
                        break;
                    case "CreateTime":
                        CreateTime = reader.ReadElementString();
                        break;
                    case "MsgType":
                        MsgType = reader.ReadElementString();
                        break;
                    case "Content":
                        Content = reader.ReadElementString();
                        break;
                    case "Title":
                        Title = reader.ReadElementString();
                        break;
                    case "Description":
                        Description = reader.ReadElementString();
                        break;
                    case "Url":
                        Url = reader.ReadElementString();
                        break;
                    case "MsgId":
                        MsgId = reader.ReadElementString();
                        break;
                    case "PicUrl":
                        PicUrl = reader.ReadElementString();
                        break;
                    case "Event":
                        Event = reader.ReadElementString();
                        break;
                    case "EventKey":
                        EventKey = reader.ReadElementString();
                        break;  
                    default:
                        reader.Read();
                        break;
                }
            }
        }        
    }
}
