using Sys.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobileWx.Model
{
    [XmlRoot("xml")]
    public class WxResponse : ModelWx,IXmlSerializable
    {
        public WxResponse():base()
        { 
        
        }
        public WxResponse(string fromUser, string toUser)
            : this()
        {
            FromUserName = fromUser;
            ToUserName = toUser;
        }
        public WxResponse(string fromUser, string toUser, DateTime d)
            : this(fromUser, toUser)
        {
            CreateTime = (d - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
        public List<WxArticle> Articles
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
            throw new NotImplementedException();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
            if (Articles != null && Articles.Count>0)
            {
                writer.WriteStartElement("ArticleCount");
                writer.WriteString(Articles.Count.ToString());
                writer.WriteEndElement();
                writer.WriteStartElement("Articles");
                foreach (WxArticle item in Articles)
                {
                    writer.WriteRaw(XmlUtility.Serialize(item));
                }
                writer.WriteEndElement();
            }
        }
    }
}
