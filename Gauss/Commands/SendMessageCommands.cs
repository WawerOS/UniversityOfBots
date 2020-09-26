/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Models;
using Gauss.Utilities;

namespace Gauss.Commands {
	public class SendMessageCommands : BaseCommandModule {
		private readonly MessageDataContext _dbContext;

		public SendMessageCommands() {
			this._dbContext = new MessageDataContext();
		}

		[Description("Send a message anonymously through the bot to a channel")]
		[Command("send")]
		public async Task SendMessage(
			CommandContext context,
			[Description("Name of the channel you want to post in.")]
			string channelName,
			[Description("Your message")]
			[RemainingText] string message
		) {
			var guild = context.GetGuild();
			var member = guild.Members[context.User.Id];
			var channel = guild.Channels.Values.SingleOrDefault(y => y.Name == channelName);
			if (channel != null) {
				if (channel.PermissionsFor(member).HasFlag(DSharpPlus.Permissions.SendMessages)) {
					await channel.SendMessageAsync(message);
				} else {
					await channel.SendMessageAsync($"Can't send your message to {channel.Name}.");
				}
			} else {
				await context.RespondAsync($"Channel '{channelName}' could not be found.");
			}
		}

		[Command("send_dm")]
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

			var guild = context.GetGuild();
			var sendingMember = guild.Members[context.User.Id];
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

			MessageUserConfig receiverSettings;
			lock (_dbContext) {
				receiverSettings = (from settings in _dbContext.UserSettings
									where settings.UserId == receivingMember.Id
									select settings).SingleOrDefault();
			}
			if (receiverSettings.BlockDMs) {
				await context.RespondAsync($"Can't send your message to {receiver}.");
			} else {
				var channel = await receivingMember.CreateDmChannelAsync();
				await channel.SendMessageAsync($"Anonymous says: {message}");
			}
		}


		[Description("Disable / enable receiving DMs via the send command. Only affects you.")]
		[Command("send_disableDMs")]
		[Aliases("send_blockDMs")]
		public async Task BlockDMs(
			CommandContext context,
			[Description("Block DMs (true) or allow them (false)")]
			bool block = true
		) {
			MessageUserConfig userConfig;
			lock (_dbContext) {
				userConfig =
					(from config in this._dbContext.UserSettings
					 where config.UserId == context.User.Id
					 select config).FirstOrDefault();
			}
			if (userConfig == null) {
				userConfig = new MessageUserConfig() {
					UserId = context.User.Id,
					BlockDMs = block
				};
				lock (_dbContext) {
					_dbContext.UserSettings.Add(userConfig);
					_dbContext.SaveChanges();
				}
			} else {
				userConfig.BlockDMs = block;
				lock (_dbContext) {
					_dbContext.UserSettings.Update(userConfig);
					_dbContext.SaveChanges();
				}
			}
			if (userConfig.BlockDMs) {
				await context.RespondAsync($"You will no longer receive DMs via the `send_dm` command.");
			} else {
				await context.RespondAsync($"You can now receive DMs via the `send_dm` command again.");
			}
		}
	}
}