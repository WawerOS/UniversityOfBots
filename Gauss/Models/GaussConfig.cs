using Newtonsoft.Json;

namespace Gauss.Models {
    public class GaussConfig{
        [JsonProperty("discord_token")]
		public string DiscordToken { get; set; }

		[JsonProperty("command_prefix")]
		public string CommandPrefix { get; set; }

        [JsonProperty("assign_roles")]
		public ulong[] AutoAssignedRoles { get; set; }
        
    }
}