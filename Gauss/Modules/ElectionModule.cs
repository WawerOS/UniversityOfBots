using System;
using System.Threading.Tasks;
using DSharpPlus;
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
			foreach (var election in activeElections) {
				if (election.Message == null) {
					if (election.Start <= DateTime.UtcNow) {
						var voteChannel = this._client
							.Guilds[election.GuildId]
							.Channels[this._config.GuildConfigs[election.GuildId].VoteChannel];

						var message = await voteChannel.SendMessageAsync(embed: election.GetEmbed());
						election.Message = new Models.Elections.MessageReference() {
							GuildId = election.GuildId,
							ChannelId = message.ChannelId,
							MessageId = message.Id,
						};
					}
				} else {
					if (election.End <= DateTime.UtcNow) {
						await election.Message.UpdateMessage(this._client, election.GetEmbed());
						var voteChannel = this._client
							.Guilds[election.GuildId]
							.Channels[this._config.GuildConfigs[election.GuildId].VoteChannel];
						election.Status = Models.Elections.ElectionStatus.Decided;
						await voteChannel.SendMessageAsync(
							$"Election #{election.ID} for {election.Title} has concluded. Results:\n{election.GetResults()}"
						);
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