using System.Collections.Generic;

namespace WebDataEntry.Web.Models
{
    public class EntryComparer : IComparer<Entry>
    {
        private bool TryCompare<T>(T x, T y, out int value)
        {
            if (ReferenceEquals(x, y))
            {
                value = 0;
                return true;
            }

            if (x == null && y == null)
            {
                value = 0;
                return true;
            }

            if (x == null)
            {
                value = -1;
                return true;
            }

            if (y == null)
            {
                value = 1;
                return true;
            }

            value = -2;
            return false;
        }

        public int Compare(Entry x, Entry y)
        {
            int ret;

            if (TryCompare(x, y, out ret))
                return ret;

            return Compare(x.DateEntry, y.DateEntry);
        }

        private int Compare(DateEntry x, DateEntry y)
        {
            int ret;
            if (TryCompare(x, y, out ret))
                return ret;

            if (x.Year != y.Year)
                return x.Year < y.Year ? -1 : 1;

            if (x.Month != y.Month)
                return x.Month < y.Month ? -1 : 1;

            if (x.Day != y.Day)
                return x.Day < y.Day ? -1 : 1;

            return 0;
        }
    }
}