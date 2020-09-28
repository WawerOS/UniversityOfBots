/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gauss.Database;
using Gauss.Models;
using Gauss.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Gauss.Modules {
	public class VoiceChannelChangeEvent {
		public DiscordChannel Channel { get; set; }
		public DiscordUser User { get; set; }
		public bool HasJoined { get; set; } // true: joined. false: left.
	}

	public class VCModule {
		private readonly GaussConfig _config;
		private readonly UserSettingsContext _settings;
		

		public VCModule(DiscordClient client, GaussConfig config, ServiceProvider commandServices) {
			this._config = config;
			this._settings = commandServices.GetService<UserSettingsContext>();
			client.VoiceStateUpdated += this.HandleVoiceStateEvent;
		}

		private async Task NotifyUsers(DiscordGuild guild, DiscordUser joinedUser, DiscordChannel channel) {
			var configs = from config in _settings.UserVoiceSettings
						  where config.GuildId == guild.Id
							  && config.IsInTimeout == false
							  && config.IsActive == true
						  select config;

			foreach (var config in configs) {
				if (channel.Users.Any(user => user.Id == config.UserId)) {
					continue;
				}
				if (config.UserId == joinedUser.Id) {
					config.IsInTimeout = true;
					continue;
				}

				if (config.FilterMode != FilterMode.Disabled && config.TargetUsers.Count > 0) {
					var joinedUserOnList = config.TargetUsers.Any(y => y.UserId == joinedUser.Id);
					if (config.FilterMode == FilterMode.Whitelist && !joinedUserOnList) {
						continue;
					}
					if (joinedUserOnList && config.FilterMode == FilterMode.Blacklist) {
						continue;
					}
				}

				try {
					var member = guild.Members[config.UserId];
					var matchingStatus = member?.Presence != null && member.Presence.Status.MatchesAvailability(config.TargetStatus);
					if (!matchingStatus) {
						continue;
					}
					var dmChannel = await member.CreateDmChannelAsync();
					await dmChannel.SendMessageAsync($"{joinedUser.Username} just joined the {channel.Name} voice channel!");
					config.IsInTimeout = true;
				} catch (Exception ex) {
					throw new Exception($"Exception while trying to notify {config.UserId} about voice chat. {ex}");
				}
			}
			_settings.SaveChanges();
		}


		private Task HandleVoiceStateEvent(VoiceStateUpdateEventArgs e) {
			/* 	3 possible events:
				A: User joins a voice channel.
					-> Dispatch one JoinedVoiceChannel event.
				B: User switches from one channel to another.
					-> Dispatch one JoinedVoiceChannel event.
					-> Dispatch one LeftVoiceChannel event.
				C: User disconnects from voice.
					-> Dispatch one LeftVoiceChannel event.
			*/

			/*
				Design questions:
				- When to notify about a new voice chat? Options:
					A: Notify about *any* channel going from 0 users to 1 user, regardless of other voice chats.
						Example: Xorander joins General Voice 01 -> Notify IonSprite.
								Ripple also joins General Voice 01: No additional notification.
								Shortly after, Fritz joins General Voice 02 -> Notifiy IonSprite again.
								And only when a channel is empty again can a new notification be send.

					B: Notify per channel category.
						Exmaple:  Xorander joins General Voice 01 -> Notify IonSprite.
								Ripple joins General Voice 02 -> No additional notification.
								And only when all voice channels in #General are empty again can a new notification be send.

					C: Notify for every joined user when the would-be-notified user isn't in a voice chat themselves?
						- Could end up spammy, especially when people join quickly.
							- Could also lead to very strict whitelisting
						... maybe that could work with some clever cooldown.
			*/

			return Task.Run(() => {
				if (e.Before?.Channel == null && e.After?.Channel != null) {
					if (!e.Channel.ParentId.HasValue || this._config.VoiceNotificationCategories.Contains(e.Channel.ParentId.Value)) {
						return;
					}
					Task.Run(async () => {
						await Task.Delay(TimeSpan.FromSeconds(10));
						if (e.Channel.Users.Count() > 0) {
							await NotifyUsers(e.Guild, e.User, e.After.Channel);
						}
					});
				} else {
					if (e.Before?.Channel?.Users?.Count() == 0) {
						Task.Run(async () => {
							await Task.Delay(TimeSpan.FromSeconds(15));
							if (e.Before?.Channel?.Users?.Count() > 0) {
								return;
							}

							foreach (var config in _settings.UserVoiceSettings.Where(x => x.GuildId == e.Guild.Id)) {
								config.IsInTimeout = false;
							}
							_settings.SaveChanges();
						});
					}
				}
			});
		}
	}
}