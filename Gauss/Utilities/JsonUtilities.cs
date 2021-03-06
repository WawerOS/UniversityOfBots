/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Gauss.Utilities {

	static class JsonUtility {
		public static T Deserialize<T>(string path) {
			try {
				var json = File.ReadAllText(path, new UTF8Encoding(false));
				return JsonConvert.DeserializeObject<T>(json, new DateTimeZoneConverter());
			} catch (Exception ex) {
				throw new Exception($"Exception encountered while trying to load and deserialize {path}: {ex}");
			}
		}

		public static void Serialize<T>(string path, T instance) {
			try {
				var json = JsonConvert.SerializeObject(instance, Formatting.Indented);
				File.WriteAllText(path, json, new UTF8Encoding(false));
			} catch (Exception ex) {
				throw new Exception($"Error while serializing file '{path}': {ex}");
			}
		}
	}
}