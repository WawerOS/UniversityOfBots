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
using Gauss.Utilities;

namespace Gauss.Commands {
	[NotBot]
	[CheckDisabled]
	public class MiscCommands : BaseCommandModule {
		private readonly CalendarAccessor _calendar;

		public MiscCommands(CalendarAccessor calendar) {
			this._calendar = calendar;
		}

		[Command("upcoming")]
		[Aliases("nextevent")]
		[Description("Get the next scheduled server event from the calendar.")]
		public async Task GetEvents(CommandContext context) {
			var nextEvent = await this._calendar.GetNextEvent(context.GetGuild().Id);
			if (nextEvent == null) {
				await context.RespondAsync("No upcoming event found");
				return;
			} else {
				await context.RespondAsync(
					embed: new DiscordEmbedBuilder()
						.WithTitle(nextEvent.Summary)
						.WithDescription(nextEvent.Description)
						.WithTimestamp(nextEvent.Start.DateTime)
						.Build()
				);
			}
		}

		[Description("Get a link to detailed documentation")]
		[Command("docs")]
		[Aliases("documentation")]
		public async Task GetDocumentation(CommandContext context) {
			await context.RespondAsync("<https://stringepsilon.github.io/UniversityOfBots/>");
		}

		[Description("Get information about your privacy and this Bot.")]
		[Command("privacy")]
		public async Task GetPrivacyInfo(CommandContext context) {
			await context.RespondAsync("You can find up to date privacy information in the link below.\n" +
				"<https://stringepsilon.github.io/UniversityOfBots/PRIVACY>\n");
		}
	}
}