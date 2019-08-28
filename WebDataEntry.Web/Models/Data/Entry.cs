using System.Collections.Generic;

namespace WebDataEntry.Web.Models
{
    public class Entry
    {
        //[Required(ErrorMessage="A date is required")]
        public DateEntry DateEntry { get; set; }

        public Title Title { get; set; }

        public First First { get; set; }

        public Info Info { get; set; }

        public List<DiaryImage> Images { get; set; }

        public List<int> People { get; set; }

        public List<int> Locations { get; set; }

		public int Version { get; set; }
    }
}