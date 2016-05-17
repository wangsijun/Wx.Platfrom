using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobileWx.Model
{
    [XmlRoot("item")]
    public class WxArticle : ModelBase, IXmlSerializable
    {
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
         public string PicUrl
         {
             get;
             set;
         }
         public string Url
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

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                writer.WriteStartElement("Title");
                writer.WriteCData(Title);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(Description))
            {
                writer.WriteStartElement("Description");
                writer.WriteCData(Description);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(PicUrl))
            {
                writer.WriteStartElement("PicUrl");
                writer.WriteCData(PicUrl);
                writer.WriteEndElement();
            }
            if (!string.IsNullOrEmpty(Url))
            {
                writer.WriteStartElement("Url");
                writer.WriteCData(Url);
                writer.WriteEndElement();
            } 
        }
    }
}
