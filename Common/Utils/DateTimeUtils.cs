using System.Globalization;

namespace TestShopApp.Common.Utils
{
    public class DateTimeUtils
    {
        public static DateTimeOffset GetCurrentTimeFormatted()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Belarus Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return new DateTimeOffset(localDateTime, timeZone.GetUtcOffset(localDateTime));
        }

        public static DateTimeOffset? ParseDate(string date)
        {
            DateTimeOffset res;
            if (DateTimeOffset.TryParse(date,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
            {
                return res;
            }
            return null;
        }
    }
}
