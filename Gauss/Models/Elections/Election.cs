/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace Gauss.Models.Elections {
	public class Election {


		public ElectionStatus Status { get; set; }
		public ulong GuildId { get; set; }
		public ulong ID { get; set; }

		/// <summary>
		/// Title of the office to elect for.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Description of the election / office.
		/// </summary>
		public string Description { get; set; }

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public List<Candidate> Candidates { get; set; }

		public List<ulong> Voters { get; set; } = new List<ulong>();

		public MessageReference Message { get; set; }

		public string GetHash() {
			var json = JsonConvert.SerializeObject(this);
			var hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(json));
			var stringBuilder = new StringBuilder();
			for (int i = 0; i < hash.Length; i++) {
				stringBuilder.Append(hash[i].ToString("x2"));
			}
			// Return the hexadecimal string.
			return stringBuilder.ToString();
		}

		internal DiscordEmbed GetEmbed() {
			var optionsText = string.Join(
				"\n",
				this.Candidates.Select((y) => $" - {y.Username}")
			);

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
				Title = $"Election for {this.Title}",
				Description = this.Description + "\n" +
					$"**Start:** {this.Start:yyyy-MM-dd HH:mm} UTC\n" +
					$"**End:** {this.End:yyyy-MM-dd HH:mm} UTC\n" +
					$"**Candidates:**\n{optionsText}\n\n" +
					$"Vote via `!g election vote {this.ID} <username> [<username> ...]`",

				Color = DiscordColor.Blurple,
				Timestamp = this.End,
			};

			if (this.Message != null) {
				embedBuilder.Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = this.GetHash() };
			}
			return embedBuilder.Build();
		}

		public string GetResults() {
			var stringBuilder = new StringBuilder();
			var sortedCandidates = this.Candidates.OrderByDescending(y => y.Votes);
			int place = 1;
			foreach (var candidate in sortedCandidates) {
				stringBuilder
					.Append(place)
					.Append(". ")
					.Append(candidate.Username)
					.Append(": ")
					.Append(candidate.Votes)
					.Append("\n");

				place++;
			}
			stringBuilder
				.Append(this.Voters.Count)
				.Append(" have cast their vote.");

			return stringBuilder.ToString();
		}
	}
}