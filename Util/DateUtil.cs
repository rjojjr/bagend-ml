using System;
using System.Reflection;

namespace bagend_ml.Util
{
	public class DateUtil
	{
		public DateUtil()
		{
		}

        public static DateTime GetDateTimeFromString(string date)
        {
            var parts = date.Split("-");
            var year = Int32.Parse(parts[0]);
            var month = Int32.Parse(parts[1]);
            var day = Int32.Parse(parts[2]);

            return new DateTime(year, month, day);
        }

        public static int GetNumberOfDaysBetween(string start, string stop)
        {
            return Math.Abs(int.Parse((GetDateTimeFromString(stop) - GetDateTimeFromString(start)).TotalDays.ToString()));
        }

        public static string GetToday()
        {
            return GetDateString(DateTime.UtcNow);
        }

        public static string GetDateString(DateTime dateTime)
        {
            return dateTime.Year + "-" + padNumber(dateTime.Month) + "-" + padNumber(dateTime.Day);
        }

        public static IList<string> GetDatesBetween(string start, string end)
        {
            var dates = new List<string>();
            var date = start;
            while(CompareDateStrings(date, end) < 1)
            {
                dates.Add(date);
                var next = GetDateTimeFromString(date).AddDays(1);
                date = GetDateString(next);
            }

            return dates;
        }

        private static string padNumber(int num)
        {
            return num < 10 ? "0" + num : "" + num;
        }

        public static int CompareDateStrings(string first, string second)
        {
            var firstDt = GetDateTimeFromString(first);
            var secondDt = GetDateTimeFromString(second);

            if (GetMillisFromDateTime(firstDt) < GetMillisFromDateTime(secondDt))
            {
                return -1;
            }
            else if (GetMillisFromDateTime(firstDt) > GetMillisFromDateTime(secondDt))
            {
                return 1;
            }
            return 0;
        }

        public static long GetMillisFromDateTime(DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
        }
    }
}

