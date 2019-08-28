using System;
using System.Xml;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class ImageReader
    {
        public void ReadImageElement(XmlReader xmlReader, IImageConsumer imageConsumer)
        {
            string src = string.Empty;
            string description = string.Empty;
            xmlReader.Read();

            while (xmlReader.Read() && xmlReader.NodeType != XmlNodeType.EndElement && xmlReader.Name != "image")
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "src")
                        {
                            xmlReader.Read();
                            if (xmlReader.NodeType != XmlNodeType.Text)
                            {
                                throw new InvalidOperationException(string.Format("The source for the image must be populated."));
                            }

                            src = xmlReader.Value;
                            xmlReader.Read();
                        }

                        if (xmlReader.Name == "caption")
                        {
                            xmlReader.Read();

                            //if (xmlReader.NodeType != XmlNodeType.Text)
                            //{
                            //    throw new InvalidOperationException(string.Format("The caption for the image must be populated."));
                            //}

                            description = xmlReader.Value;
                            xmlReader.Read();
                        }

                        break;
                }
            }

            imageConsumer.AddImage(src, description);
        }
    }
}