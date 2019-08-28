using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class Title
    {
        public Title()
        {
            
        }

        public Title(Title source)
        {
            this.Value = source.Value;
            this.Name = source.Name;
        }

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