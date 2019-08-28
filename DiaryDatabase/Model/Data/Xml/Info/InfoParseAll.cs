using System.Xml;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public class InfoParseAll : IInfoParse
    {
        public void Parse(Info info, XmlReader xmlReader)
        {
            xmlReader.Read();
            info.OriginalContent = xmlReader.ReadInnerXml();
        }
    }
}