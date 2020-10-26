using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gauss.Commands;
using Gauss.Models;
using Gauss.Database;
using Gauss.Scheduling;

namespace Gauss.Modules {
	class ElectionModule : BaseModule {
		private readonly DiscordClient _client;
		private readonly GaussConfig _config;
		private readonly Scheduler _scheduler;
		private readonly ElectionRepository _repository;
		private DateTime _nextCheck = DateTime.MinValue;

		public ElectionModule(
			DiscordClient client,
			Scheduler scheduler,
			GaussConfig config,
			ElectionRepository repository
		) {
			this._client = client;
			this._config = config;
			this._scheduler = scheduler;
			this._repository = repository;

			this._scheduler.AddTask(new TaskThunk(this.ShouldUpdateElections, this.UpdateElections));
		}

		private async Task UpdateElections() {
			var activeElections = _repository.GetActiveElections();
			if (activeElections.Count == 0) {
				return;
			}
			Console.WriteLine("UpdateElections");
			foreach (var election in activeElections) {
				if (election.Message == null) {
					Console.WriteLine("Found election w/o message.");
					if (election.Start <= DateTime.UtcNow) {
						var voteChannel = this._client
							.Guilds[election.GuildId]
							.Channels[this._config.GuildConfigs[election.GuildId].VoteChannel];

						Console.WriteLine("Sending message.");
						var message = await voteChannel.SendMessageAsync(embed: election.GetEmbed());
						Console.WriteLine("Sent message.");
						election.Message = new Models.Elections.MessageReference() {
							GuildId = election.GuildId,
							ChannelId = message.ChannelId,
							MessageId = message.Id,
						};
					}
				} else {
					if (election.End <= DateTime.UtcNow) {
						await election.Message.UpdateMessage(this._client, election.GetEmbed());
						election.Status = Models.Elections.ElectionStatus.Decided;
					}
				}
			}
			this._repository.SaveChanges();
			return;
		}

		private bool ShouldUpdateElections() {
			if (this._nextCheck < DateTime.UtcNow) {
				this._nextCheck = DateTime.UtcNow + TimeSpan.FromMinutes(5);
				return true;
			}
			return false;
		}
	}
}