using System.Xml;

namespace DocXLib.Model.Data.Xml.Info
{
    public interface IInfoParse
    {
        void Parse(Info info, XmlReader xmlReader);
    }
}