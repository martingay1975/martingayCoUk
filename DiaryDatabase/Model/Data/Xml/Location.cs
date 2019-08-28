using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class Location
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute("lat")]
        public double Lattitude { get; set; }

        [XmlAttribute("lng")]
        public double Longitude { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}