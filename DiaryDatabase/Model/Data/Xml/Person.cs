using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class Person
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}