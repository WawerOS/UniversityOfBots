
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Gauss.Utilities {
	public static class ContextExtensions {
		public static DiscordGuild GetGuild(this CommandContext context) {
			if (context.Channel.IsPrivate) {
				return context.Client.Guilds.Values
					.ToList()
                    .Find(
						x => x.Members.Values.Any(m => m.Id == context.Message.Author.Id)
					);
			} 
			return context.Guild;
		}

		public static async Task<DiscordChannel> GetDMChannel(this CommandContext context){
			if (context.Channel.IsPrivate){
				return context.Channel;
			}
			var guild = context.GetGuild();
			return await guild.Members[context.User.Id]?.CreateDmChannelAsync();
		}
	}
}