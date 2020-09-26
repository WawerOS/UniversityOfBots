/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.CommandAttributes;
using Gauss.Models;
using Gauss.Utilities;

namespace Gauss.Commands {
	[Group("send")]
	[NotBot]
	public class SendMessageCommands : BaseCommandModule {
		protected static readonly MessageDataContext _dbContext;

		static SendMessageCommands() {
			_dbContext = new MessageDataContext();
		}

		[RequireAdmin]
		[Group("admin")]
		public class MessageAdmin : BaseCommandModule {
			[Command("restrict")]
			[Aliases("ban")]
			[Description("TODO")]
			public async Task BanUser(CommandContext context, string user) {
				var guild = context.GetGuild();
				var userId = guild.FindMember(user).Id;
				MessageRestrictedUser existingRestriction;
				lock (_dbContext) {
					existingRestriction = _dbContext.RestrictedUsers.FirstOrDefault(y => y.UserId == userId);
				}
				if (existingRestriction == null) {
					lock (_dbContext) {
						_dbContext.Add(new MessageRestrictedUser() {
							UserId = userId,
							RestrictionEnd = null,
						});
						_dbContext.SaveChanges();
					}
				}

				await context.RespondAsync($"'{user}' is now indefinetly restricted from using the send command.");
			}

			[Command("unrestrict")]
			[Aliases("unban")]
			[Description("TODO")]
			public async Task UnbanUser(CommandContext context, string user) {
				var guild = context.GetGuild();
				var userId = guild.FindMember(user).Id;
				MessageRestrictedUser existingRestriction;
				lock (_dbContext) {
					existingRestriction = _dbContext.RestrictedUsers.FirstOrDefault(y => y.UserId == userId);
					if (existingRestriction != null) {
						_dbContext.RestrictedUsers.Remove(existingRestriction);
						_dbContext.SaveChanges();
					}
				}
				await context.RespondAsync($"'{user}' can now use send command again.");
			}
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
			bool notAllowed;
			lock (_dbContext) {
				notAllowed = _dbContext.RestrictedUsers.Any(y => y.UserId == context.User.Id);
			}
			if (notAllowed) {
				await context.RespondAsync("You are not allowed to use this command.");
				return;
			}
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
			bool notAllowed;
			lock (_dbContext) {
				notAllowed = _dbContext.RestrictedUsers.Any(y => y.UserId == context.User.Id);
			}
			if (notAllowed) {
				await context.RespondAsync("You are not allowed to use this command.");
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
			MessageUserConfig userConfig;
			lock (_dbContext) {
				userConfig =
					(from config in _dbContext.UserSettings
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