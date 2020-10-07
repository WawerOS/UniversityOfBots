/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.IO;
using Gauss.Utilities;

namespace Gauss.Models {
	public class GaussConfig {
		private static GaussConfig _instance;
		private static string _directory;

		[JsonProperty("discord_token")]
		public string DiscordToken { get; set; }

		[JsonProperty("command_prefix")]
		public string CommandPrefix { get; set; }

		[JsonProperty("assign_roles")]
		public ulong AutoAssignedRole { get; set; }

		[JsonProperty("voice_notification_categories")]
		public List<ulong> VoiceNotificationCategories { get; set; }

		[JsonProperty("status_text")]
		public string StatusText { get; set; }

		[JsonProperty("welcome_message")]
		public Dictionary<ulong, string> WelcomeMessage {get; set;} = new Dictionary<ulong, string>();
		[JsonProperty("welcome_channel")]
		public Dictionary<ulong, ulong> WelcomeChannel {get; set;} = new Dictionary<ulong, ulong>();

		public List<ulong> RedditEnabledChannels { get; set; }

		public GaussConfig() {

		}

		public static GaussConfig GetInstance() {
			if (_instance == null) {
				_instance = new GaussConfig();
			}
			return _instance;
		}

		public static GaussConfig GetInstance(string configDirectory) {
			_directory = configDirectory;
			if (_instance == null) {
				if (!Directory.Exists(configDirectory)) {
					throw new Exception($"Config directory '{configDirectory}' does not exist.");
				}
				if (!File.Exists(Path.Join(configDirectory, "config.json"))) {
					throw new Exception($"'config.json' was not found in '{configDirectory}'.");
				}

				_instance = JsonUtility.Deserialize<GaussConfig>(Path.Join(configDirectory, "config.json"));
			}
			return _instance;
		}

		public void Save() {
			JsonUtility.Serialize<GaussConfig>(Path.Join(_directory, "config.json"), this);
		}
	}
}