using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class InfoParseWhoops : IInfoParse
    {
        public void Parse(Info info, XmlReader xmlReader)
        {
                // moves on to the info element (if there is one)
                var keepReadingWoops = xmlReader.ReadToFollowing("woops");
                while (keepReadingWoops)
                {
                    // the start of a whoops entry
                    if (!xmlReader.HasAttributes)
                    {
                        throw new InvalidOperationException("The Woops entry does not have any attributes");
                    }

                    var rating = 0;
                    var title = string.Empty;

                    while (xmlReader.MoveToNextAttribute())
                    {
                        if (xmlReader.Name == "rating")
                            rating = Convert.ToInt32(xmlReader.Value);
                        else if (xmlReader.Name == "woopstitle")
                            title = xmlReader.Value;
                        else
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to process the attribute '{0}'.", xmlReader.Name));
                    }

                    xmlReader.MoveToElement();
                    var keepReadingCurrentWoops = xmlReader.Read();

                    var whoop = new Whoop() { Title = title, Rating = rating};


                    var contentString = new StringBuilder();
                    using (var xmlWriter = XmlWriter.Create(contentString, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment }))
                    {
                        while (keepReadingCurrentWoops)
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                if (xmlReader.Name.Equals("image", StringComparison.OrdinalIgnoreCase))
                                {
                                    var imageReader = new ImageReader();
                                    imageReader.ReadImageElement(xmlReader.ReadSubtree(), whoop);
                                    keepReadingCurrentWoops = xmlReader.Read();
                                }
                                else
                                {
                                    xmlWriter.WriteNode(xmlReader, true);
                                }
                            }
                            else if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "woops")
                            {
                                xmlWriter.Flush();
                                whoop.Content = contentString.ToString();
                                info.AddWhoop(whoop);
                                keepReadingCurrentWoops = false;
                                keepReadingWoops = xmlReader.ReadToFollowing("woops");
                            }
                        }
                    }
                }
            }
    }
}