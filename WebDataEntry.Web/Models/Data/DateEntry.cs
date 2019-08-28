namespace WebDataEntry.Web.Models
{
    public class DateEntry
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int Year { get; set; }
        public string DayOfWeek { get; set; }

        public override bool Equals(object obj)
        {
            var target = obj as DateEntry;
            if (target == null)
                return false;

            if (target.Year > 0 && target.Year != Year)
                return false;

            if (target.Month.HasValue && target.Month > 0 && target.Month != Month)
                return false;

            if (target.Day.HasValue && target.Day > 0 && target.Day != Day)
                return false;

            return true;
        }

        public static bool operator == (DateEntry a, DateEntry b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Equals(b);
        }

        public static bool operator != (DateEntry a, DateEntry b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Day ?? 0) ^ (Month ?? 0) ^ Year;
        }
    }
}