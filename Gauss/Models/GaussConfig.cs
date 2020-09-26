/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gauss.Models {
	public class GaussConfig {
		[JsonProperty("discord_token")]
		public string DiscordToken { get; set; }

		[JsonProperty("command_prefix")]
		public string CommandPrefix { get; set; }

		[JsonProperty("assign_roles")]
		public ulong AutoAssignedRole { get; set; }

		[JsonProperty("voice_notification_categories")]
		public List<ulong> VoiceNotificationCategories {get; set;}

	}
}