
/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using FuzzySharp;

namespace Gauss.Utilities {
	public static class ContextExtensions {
		static private readonly DiscordEmoji _confirmEmoji = DiscordEmoji.FromUnicode("✅");
		static private readonly DiscordEmoji _abortEmoji = DiscordEmoji.FromUnicode("❌");

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

		public static void GuessCommand(this CommandContext context, string query) {
			// TODO: Aliases should probably be searched as well.
			var bestGuess = context.CommandsNext.RegisteredCommands
				.OrderBy(y => Fuzz.Ratio(y.Value.QualifiedName, query))
				.Last()
				.Value;

			if (Fuzz.Ratio(bestGuess.QualifiedName, query) < 60) {
				bestGuess = null;
			}

			if (bestGuess != null) {
				context.RespondAsync(
					$"Could not find command `{query}`. Did you mean `{bestGuess.QualifiedName}`?"
				);
			} else {
				context.RespondAsync($"Could not find command `{query}`.");
			}
		}

		public static async Task<DiscordChannel> GetDMChannel(this CommandContext context) {
			if (context.Channel.IsPrivate) {
				return context.Channel;
			}
			var guild = context.GetGuild();
			return await guild.Members[context.User.Id]?.CreateDmChannelAsync();
		}

		public static async Task CreateConfirmation(this CommandContext context, string message, Action onConfirm = null, Action onAbort = null) {
			var embed = new DiscordEmbedBuilder()
				.WithTitle("Confirm:")
				.WithDescription(message)
				.WithColor(DiscordColor.Red)
				.WithFooter($"{_confirmEmoji} to confirm, {_abortEmoji} to abort.")
				.Build();

			DiscordMessage botMessage = await context.RespondAsync(null, false, embed);

			await botMessage.CreateReactionAsync(_confirmEmoji);
			await Task.Delay(250);
			await botMessage.CreateReactionAsync(_abortEmoji);

			var interactivity = context.Client.GetInteractivity();
			var reaction = await interactivity.WaitForReactionAsync(
				(MessageReactionAddEventArgs x) => {
					if (x.User.IsBot) {
						return false;
					}
					return x.Message == botMessage &&
				 		(x.Emoji == _confirmEmoji || x.Emoji == _abortEmoji);
				},
				TimeSpan.FromMinutes(1)
			);

			if (!reaction.TimedOut) {
				if (reaction.Result.Emoji == _confirmEmoji) {
					onConfirm?.Invoke();
					await botMessage.DeleteOwnReactionAsync(_abortEmoji);
				} else {
					onAbort?.Invoke();
					await botMessage.DeleteOwnReactionAsync(_confirmEmoji);
				}
			} else {
				await botMessage.DeleteOwnReactionAsync(_abortEmoji);
				await Task.Delay(250);
				await botMessage.DeleteOwnReactionAsync(_confirmEmoji);
				await Task.Delay(250);
				await botMessage.CreateReactionAsync(DiscordEmoji.FromUnicode("⌛"));
			}
		}
	}
}