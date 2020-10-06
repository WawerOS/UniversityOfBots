/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Utilities;
using DSharpPlus.Entities;
using Gauss.Models.Voting;
using Gauss.CommandAttributes;

namespace Gauss.Commands {
	[Group("vote")]
	[Description("voting commands")]
	[CheckDisabled]
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
		[Command("help")]
		public async Task GetHelp(CommandContext context) {
			await context.RespondAsync(
				"To vote, you have to use the `vote for` command. " +
				"You can either do that in a public channel (be mindful of bot-spam) or by DMing this bot."
			);
		}

		[Command("list")]
		[Description("List polls proposed for the next voting cycle.")]
		public async Task ListPolls(CommandContext context, PollState status = PollState.Proposal) {
			await context.RespondAsync("This command is not yet implemented.");
		}

		[Command("link")]
		[Description("Post the link to a given poll.")]
		public async Task ListPolls(CommandContext context, uint pollId) {
			await context.RespondAsync("This command is not yet implemented.");
		}

		[Command("retract")]
		[Description("Retract a proposed poll.")]
		public async Task RetractPoll(CommandContext context, uint pollId) {
			await context.RespondAsync("This command is not yet implemented.");
		}

		[Command("propose")]
		[Description("Propose a new poll to vote for in the next cycle.")]
		public async Task PostVote(CommandContext context, string description, params string[] options) {
			/*
				TODO: Make this specifically always post in the dedicated vote channel.
				TODO: Ask for confirmation before actually posting the vote.
			 	TODO: Should a vote be retractable by the person who posted it?
					If yes: For how long? 10 minutes
					Either way: Should other people (custodians) have that power?
			*/
			var poll = new Poll() {
				Id = 1,
				GuldId = context.GetGuild().Id,
				Description = description,
				EndDate = System.DateTime.UtcNow + System.TimeSpan.FromDays(7),
				Options = options.Select(
					(y, i) => new PollOption() { Id = i + 1, Name = y }
				).ToList()
			};


			var optionsText = string.Join(
				"\n",
				options.Select((x, i) => $"`{i + 1}` - {x}")
			);
			ulong voteId = 1;

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
				Title = $"Poll #{voteId}:",
				Description = description + "\n\n" + optionsText + $"\n\nTo vote, send `!g vote for {voteId} <optionNumber>`",
				Color = DiscordColor.Blurple,
				Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "More information via '!g vote help'" }
			};
			embedBuilder.Timestamp = System.DateTime.UtcNow;
			await context.RespondAsync(embed: embedBuilder.Build());
		}

		[Command("for")]
		[Description("Give your vote for a poll.")]
		[GroupCommand]
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
