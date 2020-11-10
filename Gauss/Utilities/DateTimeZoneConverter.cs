/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Linq;
using Newtonsoft.Json;
using NodaTime;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Gauss.Utilities {
	public class DateTimeZoneConverter : JsonConverter<DateTimeZone> {
		public override void WriteJson(JsonWriter writer, [AllowNull] DateTimeZone value, JsonSerializer serializer) {
			throw new NotImplementedException();
		}

		public override DateTimeZone ReadJson(JsonReader reader, Type objectType, [AllowNull] DateTimeZone existingValue, bool hasExistingValue, JsonSerializer serializer) {
			var jobject = JObject.Load(reader);
			if (jobject?.HasValues == true) {
				var timezoneId = jobject.Properties().FirstOrDefault(y => y.Name == "Id")?.Value;

				if (!string.IsNullOrEmpty(timezoneId.ToString())) {
					var result = TimezoneHelper.Find(timezoneId.ToString());
					if (result != null) {

						return result;
					}
				}
			}
			throw new Exception("Can not deserialize DateTimeZone due to invalid values.");
		}
	}
}