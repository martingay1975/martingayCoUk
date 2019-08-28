using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    [DataContract]
    [XmlRoot("diary")]
    public class Diary
    {
        public Diary()
        {
            
        }

        public Diary(Diary source)
        {
            Entries = new List<Entry>(source.Entries.Count);
            foreach (var newEntry in source.Entries.Select(sourceEntry => new Entry(sourceEntry)))
            {

                this.Entries.Add(newEntry);
            }

            Locations = new List<Location>(source.Locations);
            People = new List<Person>(source.People);
        }

        [DataMember(Name = "entries")]
        [XmlElement("entry")]
        public List<Entry> Entries { get; set; }

        [XmlArray("persons")]
        [XmlArrayItem("person")]
        public List<Person> People { get; set; }

        [XmlArray("locations")]
        [XmlArrayItem("location")]
        public List<Location> Locations { get; set; }
    }
}