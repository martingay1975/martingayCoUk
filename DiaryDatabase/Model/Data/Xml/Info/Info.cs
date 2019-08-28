using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class Info : IXmlSerializable, IImageConsumer
    {
        public Info()
        {
            
        }

        public Info(Info source)
        {
            this.Content = source.Content;
            this.OriginalContent = source.OriginalContent;
            this.Whoops = source.Whoops;
            this.Images = source.Images;
        }

        private static readonly List<IInfoParse> InfoParsers = new List<IInfoParse>() { new InfoParseForDiaryContentAndImages(), new InfoParseAll(), new InfoParseWhoops()};

        public string Content { get; set; }

        public string OriginalContent { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public List<DiaryImage> Images { get; set; }
        public void AddImage(string path, string description)
        {
            if (Images == null)
                Images = new List<DiaryImage>();

            Images.Add(new DiaryImage(path, description));
        }

        public List<Whoop> Whoops { get; private set; }
        public void AddWhoop(Whoop whoop)
        {
            if (Whoops == null)
            {
                Whoops = new List<Whoop>();
            }

            Whoops.Add(whoop);
        }

        public void ReadXml(XmlReader reader)
        {
            var input = reader.ReadOuterXml();

            foreach (var infoParser in InfoParsers)
            {
                using (var xmlReader = XmlReader.Create(new StringReader(input)))
                {
                    infoParser.Parse(this, xmlReader);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(Environment.NewLine + "\t\t");
            writer.WriteRaw(OriginalContent);
            writer.WriteRaw(Environment.NewLine + "\t");
        }
    }
}