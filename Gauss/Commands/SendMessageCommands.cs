/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Gauss.CommandAttributes;
using Gauss.Database;
using Gauss.Models;
using Gauss.Utilities;

namespace Gauss.Commands {
	[Group("send")]
	[NotBot]
	[CheckDisabled]
	public class SendMessageCommands : BaseCommandModule {
		private readonly UserSettingsContext _dbContext;

		public SendMessageCommands(UserSettingsContext dbContext) {
			this._dbContext = dbContext;
		}

		[Description("Send a message anonymously through the bot to a channel")]
		[GroupCommand]
		[Command("channel")]
		public async Task SendMessage(
			CommandContext context,
			[Description("Name of the channel you want to post in.")]
			string channelName,
			[Description("Your message")]
			[RemainingText] string message
		) {
			if (string.IsNullOrWhiteSpace(message)) {
				await context.RespondAsync("You must specify a message.");
				return;
			}
			var guild = context.GetGuild();
			var member = guild.Members[context.User.Id];
			var channel = guild.FindChannel(channelName);

			if (channel != null) {
				if (channel.PermissionsFor(member).HasFlag(DSharpPlus.Permissions.SendMessages)) {
					await channel.SendMessageAsync(message);
					await context.Message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":white_check_mark:"));
				} else {
					await channel.SendMessageAsync($"Can't send your message to {channel.Name}.");
				}
			} else {
				await context.RespondAsync($"Channel '{channelName}' could not be found.");
			}
		}

		[Command("dm")]
		[Description("Send an anonymous message to another guild member via DM.")]
		public async Task SendDM(
			CommandContext context,
			[Description("Name or @mention of the user you want to message.")]
			string receiver,
			[Description("Your message")]
			[RemainingText] string message
		) {
			if (context.User.IsBot) {
				return;
			}

			if (string.IsNullOrWhiteSpace(message)) {
				await context.RespondAsync("You must specify a message.");
				return;
			}

			var guild = context.GetGuild();
			var receivingMember = guild.FindMember(receiver);

			if (receivingMember == null) {
				await context.RespondAsync($"Could not find '{receiver}'.");
				return;
			} else if (receivingMember.IsBot) {
				await context.RespondAsync($"You can't message bots with this command.");
				return;
			} else if (receivingMember.Id == context.User.Id) {
				return;
			}

			UserMessageSettings receiverSettings = this._dbContext.GetMessageSettings(guild.Id, receivingMember.Id);
			if (receiverSettings != null && receiverSettings.BlockDMs) {
				await context.RespondAsync($"Can't send your message to {receiver}.");
			} else {
				var channel = await receivingMember.CreateDmChannelAsync();
				await channel.SendMessageAsync($"Anonymous says: {message}");
			}
		}


		[Description("Disable / enable receiving DMs via the send command. Only affects you.")]
		[Command("disableDMs")]
		[Aliases("blockDMs")]
		public async Task BlockDMs(
			CommandContext context,
			[Description("Block DMs (true) or allow them (false)")]
			bool block = true
		) {
			var guild = context.GetGuild();

			UserMessageSettings userConfig = _dbContext.GetMessageSettings(guild.Id, context.User.Id);
			if (userConfig == null) {
				userConfig = new UserMessageSettings() {
					GuildId = guild.Id,
					UserId = context.User.Id,
					BlockDMs = block
				};
				_dbContext.SetMessageSettings(userConfig);
			} else {
				userConfig.BlockDMs = block;
				_dbContext.SetMessageSettings(userConfig);
			}
			if (userConfig.BlockDMs) {
				await context.RespondAsync($"You will no longer receive DMs via the `send_dm` command.");
			} else {
				await context.RespondAsync($"You can now receive DMs via the `send_dm` command again.");
			}
		}
	}
}