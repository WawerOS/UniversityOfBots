/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

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