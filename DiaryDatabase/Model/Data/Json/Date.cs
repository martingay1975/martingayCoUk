using System.Runtime.Serialization;

namespace DiaryDatabase.Model.Data.Json
{
    [DataContract]
    public class Date
    {
        [DataMember(Name="d", EmitDefaultValue = false)]
        public int? Day { get; set; }

        [DataMember(Name = "m", EmitDefaultValue = false)]
        public int? Month { get; set; }

        [DataMember(Name = "y")]
        public int Year { get; set; }

		//[DataMember(Name = "dayofweek", EmitDefaultValue = false)]
		//public string DayOfWeek { get; set; }
    }
}