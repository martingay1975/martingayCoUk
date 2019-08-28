using System.Xml;

namespace DiaryDatabase.Model.Data.Xml.Info
{
    public interface IInfoParse
    {
        void Parse(Info info, XmlReader xmlReader);
    }
}