using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class InfoParseForDiaryContentAndImages : IInfoParse
    {
        public void Parse(Info info, XmlReader xmlReader)
        {
            var ignoreElements = new List<string>() { "info", "woops" };

            // create a means of writing the results via an XmlWriter to the contentString.
            var contentString = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(contentString, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
            {
                // moves on to the info element (if there is one)
                var keepReading = xmlReader.Read();

                // keep looping around the content.
                while (keepReading)
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.Name.Equals("image", StringComparison.OrdinalIgnoreCase))
                        {
                            var imageReader = new ImageReader();
                            imageReader.ReadImageElement(xmlReader.ReadSubtree(), info);
                            keepReading = xmlReader.Read();
                        }
                        else if (ignoreElements.Contains(xmlReader.Name))
                        {
                            // ignore this element. Do not process or write out.
                            keepReading = xmlReader.Read();
                        }
                        else
                        {
                            xmlWriter.WriteNode(xmlReader, true);
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement && ignoreElements.Contains(xmlReader.Name))
                    {
                        keepReading = xmlReader.Read();
                    }
                    else
                    {
                        string msgLocation = string.Empty;
                        if (xmlReader.NodeType == XmlNodeType.Text)
                            msgLocation = xmlReader.Value;

                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The Xml is not in the correct format to be passed. See '{0}'", msgLocation));
                    }
                }
            }

            info.Content = contentString.ToString();
        }
    }
}