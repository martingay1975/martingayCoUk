using System;
using System.Xml.Serialization;

namespace DocXLib.Model.Data.Xml
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

        public string GetShortDate()
        {
            return $"{GetValue(this.Day)} {GetValue(this.Month)} {GetValue(this.Year)}";
        }
        public string GetLongDate()
        {
            return $"{GetValue(this.Day)} {GetMonthName(this.Month)} {GetValue(this.Year)}";
        }

        public static string GetValue(int? value)
        {
            return value.HasValue ? value.ToString() : string.Empty;
        }

        public static string GetMonthName(int? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }

            switch (value.Value)
            {
                case 1: return "January";
                case 2: return "February";
                case 3: return "March";
                case 4: return "April";
                case 5: return "May";
                case 6: return "June";
                case 7: return "July";
                case 8: return "August";
                case 9: return "September";
                case 10: return "October";
                case 11: return "November";
                case 12: return "December";
                default: throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

    }
}