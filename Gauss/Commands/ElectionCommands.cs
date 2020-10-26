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
using Gauss.CommandAttributes;
using Gauss.Models.Elections;
using System;
using Gauss.Database;

namespace Gauss.Commands {
	[Group("election")]
	[Description("voting commands")]
	[RequireGuild]
	[CheckDisabled]
	public class ElectionCommands : BaseCommandModule {
		private readonly ElectionRepository _pollRepository;

		public ElectionCommands(ElectionRepository pollRepository) {
			_pollRepository = pollRepository;
		}




		[Command("draft")]
		[Aliases("create")]
		[Description("Draft a new election. The details can be edited with other commands until finalized.")]
		[RequireAdmin]
		public async Task CreateElection(
			CommandContext context,
			[Description("Title that is being elected.")]
		 	string title,
			[Description("Planned start of the election (UTC).")]
			DateTime start,
			[Description("Planned end of the election (UTC).")]
			DateTime end,
			[Description("List of candidates. Either by username or displayname")]
			params string[] candidateNames
		) {
			var guild = context.GetGuild();
			List<Candidate> candidates = new List<Candidate>();
			List<string> missingCandidates = new List<string>();
			foreach (var item in candidateNames) {
				var member = guild.FindMember(item);
				if (member == null) {
					missingCandidates.Add(item);
				}
				candidates.Add(new Candidate() { UserId = member.Id, Username = member.Username, Votes = 0 });
			}
			var election = new Election() {
				Candidates = candidates,
				Title = title,
				Start = start.ToUniversalTime(),
				End = end.ToUniversalTime(),
			};
			_pollRepository.AddElection(guild.Id, election);

			await context.RespondAsync(embed: election.GetEmbed());

			await context.CreateConfirmation(
				"Schedule the above election poll?",
				() => {
					this._pollRepository.CommitElection(context.GetGuild().Id, election.ID);
					context.RespondAsync("Election committed and scheduled.");
				},
				() => {
					this._pollRepository.RemoveElection(context.GetGuild().Id, election.ID);
					context.RespondAsync("Draft withdrawn.");
				}
			);
		}



		[Command("vote")]
		[Description("Vote for one or more candidates. The voting system is approval voting.")]
		public async Task VoteFor(CommandContext context, ulong electionId, params string[] candidateNames) {
			var guild = context.GetGuild();
			var member = guild.Members[context.User.Id];
			var voterStatus = _pollRepository.CanVote(guild.Id, electionId, member.Id);
			switch (voterStatus) {
				case VoteStatus.ElectionNotFound: {
						await context.RespondAsync($"Could not find the election with ID `{electionId}`.");
						return;
					}
				case VoteStatus.ElectionNotStarted: {
						await context.RespondAsync($"Election #{electionId} has not started yet.");
						return;
					}
				case VoteStatus.ElectionOver: {
						await context.RespondAsync($"Election #{electionId} is already over and decided.");
						return;
					}
				case VoteStatus.AlreadyVoted: {
						await context.RespondAsync($"You already voted in this election. Unfortunately, you can not change your vote.");
						return;
					}
			}

			var election = _pollRepository.GetElection(guild.Id, electionId);
			List<Candidate> candidates = new List<Candidate>();
			foreach (var item in candidateNames) {
				var foundCandidate = election.Candidates.Find(y => y.Username.ToLower() == item.ToLower());
				if (foundCandidate == null) {
					await context.RespondAsync($"Could not find `{item}` on the list of candidates.");
					return;
				}
				candidates.Add(foundCandidate);
			}

			var confirmationMessage = $"Do you approve of the following candidate(s) for {election.Title} and want to give them your vote?:\n\n"
				+ $"{string.Join("\n", candidates.Select(y => " - " + y.Username))}\n\n"
				+ $"**Your vote can not be changed after confirmation. Make sure you selected all your approved candidates.**";


			await context.CreateConfirmation(
				confirmationMessage,
				async () => {
					var (hashBefore, hashafter) = _pollRepository.SaveVotes(guild.Id, election.ID, context.User.Id, candidates, context.Client);
					await guild.Channels[637646862798553108].SendMessageAsync(
						$"A user cast a their vote for election #{electionId}.\nHash before: {hashBefore}\nHash after: {hashafter}"
					);
					await context.RespondAsync("Your vote has been cast.");
				}
			);
		}
	}
}
