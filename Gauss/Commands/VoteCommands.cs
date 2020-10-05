/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Utilities;
using DSharpPlus.Entities;

namespace Gauss.Commands {
	[Group("vote")]

	[Description("voting commands")]
	public class VoteCommands : BaseCommandModule {

		/* Stuff to implement / design:
			- Database for the polls
			- Database for user's token pools
			- Periodically check if votes have run out of time ->
				- close the poll
				- update the original message to indicate that it's decided
				- post the result
			- Run a scheduler to reset the token pool
				- Probably write a general schedular that tasks can be added too.
				- "run these sets of methods every second" or something.

		*/

		[Command("post")]
		public async Task PostVote(CommandContext context, string description, params string[] options) {
			/*
				TODO: Make this specifically always post in the dedicated vote channel.
				TODO: Ask for confirmation before actually posting the vote.
			 	TODO: Should a vote be retractable by the person who posted it?
					If yes: For how long? 10 minutes
					Either way: Should other people (custodians) have that power?
			*/

			var optionsText = string.Join(
				"\n",
				options.Select((x, i) => $"`{i + 1} - {x}`")
			);
			ulong voteId = 1;

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
				Title = description,
				Description = optionsText,
				Color = DiscordColor.Blurple,
				Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = $"To vote, send `!g vote for {voteId} <optionNumber>`" }
			};

			await context.RespondAsync(embed: embedBuilder.Build());
		}

		[Command("for")]
		public async Task VoteFor(CommandContext context, ulong voteId, ulong optionNumber, uint votes = 1) {
			uint voteCost = votes == 1
				? 0
				: ((votes * votes / 2) + (votes / 2) - 1);
			var confirmationMessage = $"Do you want to vote for {optionNumber} with {votes} vote(s) for {voteCost} tokens?";

			await context.CreateConfirmation(
				confirmationMessage,
				async () => {
					await context.RespondAsync("Your vote has been cast");
				}
			);
		}
	}
}
