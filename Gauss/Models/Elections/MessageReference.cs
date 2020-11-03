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
	/// <summary>
	/// Lean object to represent a message, only containing the bare minimum information to retrieve and update the message.
	/// </summary>
	public class MessageReference {
		/// <summary>
		/// ID of the guild where the message is posted.
		/// </summary>
		public ulong GuildId { get; set; }

		/// <summary>
		/// ID of the channel the message is posted in.
		/// </summary>
		public ulong ChannelId { get; set; }

		/// <summary>
		/// ID of the message itself.
		/// </summary>
		public ulong MessageId { get; set; }

		/// <summary>
		/// Update the referenced message with a new embed.
		/// </summary>
		/// <param name="client">
		/// Client to utilize for the update.
		/// </param>
		/// <param name="newEmbed">
		/// New embed to set.
		/// </param>
		public async Task UpdateMessage(DiscordClient client, DiscordEmbed newEmbed) {
			// This might be overly defensive and verbose logging, but I plan to use this for more features, and having it robost makes that easier.
			if (!client.Guilds.ContainsKey(this.GuildId)){
				client.Logger.LogError(LogEvent.UpdateMessage, "Could not modify message - guild not found.");
				return;
			}
			var guild = client.Guilds[this.GuildId];
			if (!guild.Channels.ContainsKey(this.ChannelId)){
				client.Logger.LogError(LogEvent.UpdateMessage, "Could not modify message - channel not found.");
				return;
			}
			DiscordMessage message;
			try {
				message = await guild.Channels[this.ChannelId].GetMessageAsync(this.MessageId);
			} catch (Exception ex) {
				client.Logger.LogError(LogEvent.UpdateMessage, ex, "Could not modify message - error retrieving message.");
				return;
			}
			try {
				await message.ModifyAsync(embed: newEmbed);
			} catch (Exception ex) {
				client.Logger.LogError(LogEvent.UpdateMessage, ex, "Could not modify message - error while editing message.");
				return;
			}
		}
	}
}