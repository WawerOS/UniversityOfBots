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
using Gauss.Database;
using System.Threading;
using System;

namespace Gauss.Commands {
	[Group("vote")]
	[Description("voting commands")]
	[CheckDisabled]
	public class VoteCommands : BaseCommandModule {
		private PollRepository _pollRepository;

		public VoteCommands(PollRepository pollRepository) {
			_pollRepository = pollRepository;
		}


		private DiscordEmbed GetEmbedForPoll(CommandContext context, Poll poll) {
			var optionsText = string.Join(
				"\n",
				poll.Options.Select((y) => $"`{y.Id}` - {y.Name}")
			);
			var proposer = context.Client.Guilds[poll.GuildId].Members[poll.ProposingUser];

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
				Title = $"Poll #{(poll.Id == 0 ? "pending" : poll.Id.ToString())}:",
				Description = poll.Description + "\n\n" + optionsText + $"\n\nTo vote, send `!g vote for {poll.Id} <optionNumber>`",
				Color = DiscordColor.Blurple,
				Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = $"Proposed by {proposer.Username} at " },
				Timestamp = poll.ProposedAt,
			};

			return embedBuilder.Build();
		}


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
			var polls = this._pollRepository.ListPolls(context.GetGuild().Id);

			foreach (var poll in polls) {
				await context.RespondAsync(embed: this.GetEmbedForPoll(context, poll));
				Thread.Sleep(200);
			}
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
				ProposingUser = context.User.Id,
				GuildId = context.GetGuild().Id,
				Description = description,
				EndDate = DateTime.UtcNow + TimeSpan.FromDays(7),
				Options = options.Select(
					(y, i) => new PollOption() { Id = i + 1, Name = y }
				).ToList(),
				ProposedAt = DateTime.UtcNow,
			};

			DiscordEmbed embed = GetEmbedForPoll(context, poll);
			// embedBuilder.Timestamp = System.DateTime.UtcNow;
			await context.RespondAsync("Preview of the poll: ", embed: embed);
			await context.CreateConfirmation(
				"Propose the new poll?",
				() => {
					var id = _pollRepository.ProposePoll(poll);
					context.RespondAsync($"Poll added with ID `{id}`");
				}
			);
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
