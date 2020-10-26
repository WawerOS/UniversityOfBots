

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;
using Gauss.Models.Elections;
using Gauss.Utilities;
using Newtonsoft.Json;
using System.Security.Cryptography;
using DSharpPlus;

/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/
namespace Gauss.Database {
	public class ElectionRepository {
		private readonly Dictionary<ulong, List<Election>> _elections;
		private readonly string _configDirectory;

		public ElectionRepository(string configDirectory) {
			this._configDirectory = configDirectory;
			try {
				this._elections = JsonUtility.Deserialize<Dictionary<ulong, List<Election>>>(
					Path.Join(this._configDirectory, "elections.json")
				);
			} catch (Exception) {
				this._elections = new Dictionary<ulong, List<Election>>();
			}
		}

		public List<Election> GetActiveElections() {
			var results = new List<Election>();
			lock (this._elections) {
				foreach (var electionsList in this._elections.Values) {
					foreach (var election in electionsList.Where(y => y.Status == ElectionStatus.Active)) {
						results.Add(election);
					}
				}
			}

			return results;
		}


		public ulong AddElection(ulong guildId, Election election) {
			ulong nextId = 0;
			lock (this._elections) {
				if (!this._elections.ContainsKey(guildId)) {
					this._elections.Add(guildId, new List<Election>());
				}
				nextId = this._elections[guildId].Count > 0 ? this._elections[guildId].Max(y => y.ID) + 1 : 1;
				election.ID = nextId;
				election.GuildId = guildId;
				election.Status = ElectionStatus.Draft;
				this._elections[guildId].Add(election);
			}
			this.SaveChanges();
			return nextId;
		}

		public Election GetElection(ulong guildId, ulong id) {
			Election result = null;
			lock (this._elections) {
				if (!this._elections.ContainsKey(guildId)) {
					return null;
				}
				result = this._elections[guildId].Find(y => y.ID == id);
			}
			return result;
		}

		public bool CommitElection(ulong guildId, ulong electionId) {
			var election = this.GetElection(guildId, electionId);
			if (election == null || election.Status != ElectionStatus.Draft) {
				return false;
			}
			lock (_elections) {
				election.Status = ElectionStatus.Active;
			}
			this.SaveChanges();
			return true;
		}

		public void RemoveElection(ulong guildId, ulong electionId) {
			var election = this.GetElection(guildId, electionId);
			if (election == null || election.Status != ElectionStatus.Draft) {
				return;
			}
			lock (_elections) {
				this._elections[guildId].Remove(election);
			}
			this.SaveChanges();
		}

		internal object AddCandidate(ulong guildId, ulong electionId, DiscordMember candidate) {
			var election = this.GetElection(guildId, electionId);
			if (election == null) {
				return false;
			}
			lock (this._elections) {
				if (!election.Candidates.Any(y => y.Username == candidate.Username)) {
					election.Candidates.Add(
						new Candidate() {
							UserId = candidate.Id,
							Username = candidate.Username,
							Votes = 0
						}
					);
				}
			}
			this.SaveChanges();
			return true;
		}

		public VoteStatus CanVote(ulong guildId, ulong electionId, ulong userId) {
			var election = this.GetElection(guildId, electionId);
			if (election == null) {
				return VoteStatus.ElectionNotFound;
			}
			if (election.Voters.Contains(userId)) {
				return VoteStatus.AlreadyVoted;
			}
			if (election.Start >= DateTime.UtcNow || election.Status == ElectionStatus.Draft) {
				return VoteStatus.ElectionNotStarted;
			}
			if (election.End <= DateTime.UtcNow) {
				return VoteStatus.ElectionOver;
			}

			return VoteStatus.CanVote;
		}

		internal object RemoveCandidate(ulong guildId, ulong electionId, DiscordMember candidate) {
			var election = this.GetElection(guildId, electionId);
			if (election == null) {
				return false;
			}
			lock (this._elections) {
				var candidateEntry = election.Candidates.Find(y => y.Username == candidate.Username);
				if (candidate != null) {
					election.Candidates.Remove(candidateEntry);
				}
			}
			this.SaveChanges();
			return true;
		}

		public void SaveChanges() {
			lock (this._elections) {
				JsonUtility.Serialize(Path.Join(this._configDirectory, "elections.json"), this._elections);
			}
		}

		public (string, string) SaveVotes(ulong guildId, ulong electionId, ulong voterId, List<Candidate> candidates, DiscordClient client) {
			var election = this.GetElection(guildId, electionId);
			var hashBefore = election.GetHash();
			var hashAfter = hashBefore;
			if (!election.Voters.Contains(voterId)) {
				lock (_elections) {
					election.Voters.Add(voterId);
					foreach (Candidate candidate in candidates) {
						election.Candidates.Find(y => y.UserId == candidate.UserId).Votes++;
					}
					hashAfter = election.GetHash();
				}
				this.SaveChanges();
				_ = election.Message.UpdateMessage(client, election.GetEmbed());
			}
			return (hashBefore, hashAfter);
		}
	}
}