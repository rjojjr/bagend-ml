using System;
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
    }
}

