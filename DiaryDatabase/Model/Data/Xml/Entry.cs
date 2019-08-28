using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class Entry
    {
		[XmlElement("version")]
		public int Version { get; set; }

        [XmlElement("date")]
        public DateEntry DateEntry { get; set; }

        [XmlElement("title", IsNullable = true)]
        public Title Title { get; set; }

        [XmlElement("first", IsNullable = true)]
        public First First { get; set; }

        [XmlElement("info")]
        public Info.Info Info { get; set; }

        [XmlArray("locations", IsNullable = true)]
        [XmlArrayItem("location")]
        public List<int> Locations { get; set; }

        [XmlArray("wesaws", IsNullable = true)]
        [XmlArrayItem("wesaw")]
        public List<int> People { get; set; }

        public bool ShouldSerializeFirst()
        {
            return First != null;
        }

        public bool ShouldSerializeTitle()
        {
            return Title != null;
        }

        public bool ShouldSerializeLocations()
        {
            return Locations != null && Locations.Count > 0;
        }

        public bool ShouldSerializePeople()
        {
            return People != null && People.Count > 0;
        }

        public Entry(Entry source)
        {
            if (source.Info != null)
            {
                this.Info = new Info.Info(source.Info);
            }

            this.DateEntry = source.DateEntry;

            if (source.First != null)
            {
                this.First = source.First;
            }

            if (source.Locations != null)
            {
                this.Locations = source.Locations;
            }

            if (source.People != null)
            {
                this.People = source.People;
            }

            if (source.Title != null)
            {
                this.Title = new Title(source.Title);
            }

	        this.Version = source.Version;
        }

        public Entry()
        {
            
        }
    }
}