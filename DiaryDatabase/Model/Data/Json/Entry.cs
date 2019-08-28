using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DiaryDatabase.Model.Data.Json
{
    [DataContract]
    public class Entry
    {
		[DataMember(Name = "version", IsRequired = true)]
		public int Version { get; set; }
		
		[DataMember(Name = "date", IsRequired = true)]
        public Date Date { get; set; }

        [DataMember(Name = "title", EmitDefaultValue = false)]
        public Title Title { get; set; }

        [DataMember(Name = "first", EmitDefaultValue = false)]
        public First First { get; set; }

        [DataMember(Name = "info", EmitDefaultValue = false)]
        public Info Info { get; set; }

        [DataMember(Name = "images", EmitDefaultValue = false)]
        public int Images { get; set; }

	    [DataMember(Name = "datev")]
	    public int DateValue
	    {
		    get
		    {
				//20140308 - 8th March 2014.
			    var dateTimeString = string.Format("{0}{1:00}{2:00}", Date.Year, Date.Month ?? 0, Date.Day ?? 0);
			    return Convert.ToInt32(dateTimeString);
		    }
		    set
		    {
				var dateTimeString = string.Format("{0}{1:00}{2:00}", Date.Year, Date.Month ?? 0, Date.Day ?? 0);
				value = Convert.ToInt32(dateTimeString); 
		    }
	    }
    }
}