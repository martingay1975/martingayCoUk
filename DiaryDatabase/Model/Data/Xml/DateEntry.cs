using System;
using System.Xml.Serialization;

namespace DiaryDatabase.Model.Data.Xml
{
    public class DateEntry
    {
        [XmlElement("day", IsNullable = true)]
        public int? Day { get; set; }

        [XmlElement("month", IsNullable = true)]
        public int? Month { get; set; }

        [XmlElement("year")]
        public int Year { get; set; }

        [XmlElement("dayofweek", IsNullable = true)]
        public string DayOfWeek { get; set; }

        public bool ShouldSerializeDayOfWeek()
        {
            return !string.IsNullOrEmpty(DayOfWeek);
        }

        public bool ShouldSerializeDay()
        {
            return Day != null && Day > 0;
        }

        public bool ShouldSerializeMonth()
        {
            return Month != null && Month > 0;
        }

        public DateTime Value
        {
            get { return new DateTime(Year, Month ?? 1, Day ?? 1); }
        }

    }
}