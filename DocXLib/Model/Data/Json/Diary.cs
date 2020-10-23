using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DocXLib.Model.Data.Json
{
    [DataContract]
    public class Diary
    {
        [DataMember(Name = "entries")]
        public List<Entry> Entries { get; set; }

		[DataMember(Name = "version")]
		public int Hash { get; set; }
    }
}