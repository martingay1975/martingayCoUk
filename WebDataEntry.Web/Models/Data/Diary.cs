using System.Collections.Generic;
using WebDataEntry.Web.Models.Data;

namespace WebDataEntry.Web.Models
{
    public class Diary
    {
        public List<Entry> Entries { get; set; }
        public List<Location> Locations { get; set; }
        public List<Person> People { get; set; }

        public Diary()
        {
            
        }

        public Diary(Diary source)
        {
            Entries = new List<Entry>(source.Entries);
            Locations = new List<Location>(source.Locations);
            People = new List<Person>(source.People);
        }
    }


}