/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using DSharpPlus.Entities;
using NodaTime;

namespace Gauss.Models {
	public class Reminder {
		public ulong ID { get; set; }
		public ZonedDateTime? DueDateUser { get; set; }
		public DateTime DueDateUTC { get; set; }
		public string Message { get; set; }
		public ulong UserId { get; set; }

		public Reminder() { }

		public Reminder(ZonedDateTime due, string message, ulong userId) {
			this.DueDateUser = due;
			this.DueDateUTC = due.ToDateTimeUtc();
			this.Message = message;
			this.UserId = userId;
		}

		public Reminder(DateTime due, string message, ulong userId) {
			this.DueDateUTC = due;
			this.Message = message;
			this.UserId = userId;
		}

		public DiscordEmbed CreateEmbed() {
			var messagePreview = this.Message.Length < 50
				? this.Message
				: this.Message.Substring(0, 47).Trim() + "...";

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder {
				Color = DiscordColor.Blurple,
				Footer = new DiscordEmbedBuilder.EmbedFooter() {
				 	Text = $"{this.ID} • {messagePreview}"
				},
				Timestamp = this.DueDateUTC,
			};
			return embedBuilder.Build();
		}

		public bool IsDue(){
			if (this.DueDateUser != null){
				return this.DueDateUser.Value.ToDateTimeUtc() <= DateTime.UtcNow;
			}
			return this.DueDateUTC <= DateTime.UtcNow;
		}
	}
}