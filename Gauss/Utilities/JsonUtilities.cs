using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Gauss.Utilities {
	static class JsonUtility {
		public static T Deserialize<T>(string path) {
			try {
				var json = File.ReadAllText(path, new UTF8Encoding(false));
				return JsonConvert.DeserializeObject<T>(json);
			} catch (Exception ex) {
				throw new Exception($"Exception encountered while trying to load and deserialize {path}: {ex}");
			}
		}
	}
}