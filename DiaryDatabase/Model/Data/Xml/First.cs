using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class First
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public bool ShouldSerializeValue()
        {
            return !string.IsNullOrEmpty(Value);
        }

        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
}