using NodaTime;
using System.Linq;

namespace Gauss.Utilities {
	static class TimezoneHelper {
		public static DateTimeZone Find(string query) {
			var timezoneID = DateTimeZoneProviders.Tzdb.Ids.FirstOrDefault(y => y.ToLower() == query.ToLower());
			if (timezoneID != null) {
				return DateTimeZoneProviders.Tzdb[timezoneID];
			}

			timezoneID = DateTimeZoneProviders.Bcl.Ids.FirstOrDefault(y => y.ToLower() == query.ToLower());
			if (timezoneID != null) {
				return DateTimeZoneProviders.Bcl[timezoneID];
			}
			return null;
		}
	}
}