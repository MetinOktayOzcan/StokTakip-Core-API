using StokTakip_Core_API.Interfaces;

namespace StokTakip_Core_API.Services
{
    public class TurkeyDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now
        {
            get
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                }
                catch (TimeZoneNotFoundException)
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                }
            }
        }
    }
}