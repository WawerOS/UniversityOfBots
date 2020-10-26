/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Gauss.Models.Elections {
	public class MessageReference {
		public ulong GuildId { get; set; }
		public ulong ChannelId { get; set; }
		public ulong MessageId { get; set; }

		public async Task UpdateMessage(DiscordClient client, DiscordEmbed newEmbed) {
			try {
				var message = await client.Guilds[this.GuildId].Channels[this.ChannelId].GetMessageAsync(this.MessageId);
				await message.ModifyAsync(embed: newEmbed);
			} catch (Exception ex) {
				client.Logger.LogError(ex, "Could not modify message.");
			}
		}
	}
}