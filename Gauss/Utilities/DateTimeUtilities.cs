/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using NodaTime;

namespace Gauss.Utilities {
	public static class DateTimeExtensions {
		/// <summary>
		/// Assumes the display value of a UTC DateTime and shifts it into a speficied timezone.
		/// I.E. making 12:00:00 UTC into 12:00:00 CEST.
		/// </summary>
		public static ZonedDateTime InTimeZone(this DateTime datetime, DateTimeZone timeZone) {
			return new LocalDateTime(
				datetime.Year,
				datetime.Month,
				datetime.Day,
				datetime.Hour,
				datetime.Minute,
				datetime.Second
			).InZoneLeniently(timeZone);
		}
	}
}