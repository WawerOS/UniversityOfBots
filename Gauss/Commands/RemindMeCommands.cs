/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.CommandAttributes;
using Gauss.Database;
using Gauss.Models;
using Gauss.Utilities;
using NodaTime;

namespace Gauss.Commands {
	[NotBot]
	[CheckDisabled]
	public class RemindMeCommands : BaseCommandModule {
		private readonly Dictionary<ulong, DateTimeZone> _userTimezones = new Dictionary<ulong, DateTimeZone>();
		private readonly ReminderRepository _repository;
	
		public RemindMeCommands(ReminderRepository repository){
			this._repository = repository;
		}


		private DateTimeZone GetUserTimezone(ulong userId){
			DateTimeZone result;
			lock (_userTimezones) {
				this._userTimezones.TryGetValue(userId, out result);
			}
			if (result == null) {
				result = DateTimeZone.Utc;
			}
			return result;
		}

		[Command("settimezone")]
		public async Task SetUserTimezone(CommandContext context, string timezoneName) {
			var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneName);
			if (timezone != null) {
				var now = Instant.FromDateTimeUtc(DateTime.UtcNow);
				lock (_userTimezones) {
					if (this._userTimezones.ContainsKey(context.User.Id)) {
						this._userTimezones[context.User.Id] = timezone;
					} else {
						this._userTimezones.Add(context.User.Id, timezone);
					}
				}
				
				await context.RespondAsync($"Saved {timezone.Id}: {timezone.GetUtcOffset(now).ToTimeSpan()}");
			}
			return;
		}

		[Command("remindme")]
		public async Task SetReminder(CommandContext context, DateTime datetime, [RemainingText] string message = "") {
			var zonedDateTime = datetime.InTimeZone(this.GetUserTimezone(context.User.Id));
			Reminder reminder = new Reminder(zonedDateTime, message, context.User.Id);
			this._repository.AddReminder(reminder);
			await context.RespondAsync(embed: reminder.CreateEmbed());
			return;
		}

		[Command("remindme")]
		public async Task SetReminder(CommandContext context, DateTime date, DateTime time, [RemainingText] string message = "") {
			var datetime = date.Date + time.TimeOfDay;
			var zonedDateTime = datetime.InTimeZone(this.GetUserTimezone(context.User.Id));
			Reminder reminder = new Reminder(zonedDateTime, message, context.User.Id);
			this._repository.AddReminder(reminder);
			await context.RespondAsync(embed: reminder.CreateEmbed());
			return;
		}

		[Command("remindme")]
		public async Task SetReminder(CommandContext context, string day, DateTime time, [RemainingText] string message = "") {
			ZonedDateTime zonedDateTime;
			switch(day){
				case "today": {
					zonedDateTime = time.InTimeZone(this.GetUserTimezone(context.User.Id));
					break;
				}
				case "tomorrow":{
					zonedDateTime = time.InTimeZone(this.GetUserTimezone(context.User.Id));
					zonedDateTime = zonedDateTime.PlusHours(24);
					break;
				}	
				default: {
					await context.RespondAsync($"'{day}' not supported. Only 'today' or 'tomorrow'");
					return;
				}
			}
			Reminder reminder = new Reminder(zonedDateTime, message, context.User.Id);
			this._repository.AddReminder(reminder);
			await context.RespondAsync(embed: reminder.CreateEmbed());
			return;
		}

		
		[Command("remindme")]
		public async Task SetReminder(CommandContext context, int time, string unit, [RemainingText] string message = "") {
			if (time < 0){
				await context.RespondAsync("Reminder can't be set for the past.");
				return;
			}
			DateTime dueAt = DateTime.UtcNow;

			switch (unit.ToLower()) {
				case "minute":
				case "minutes": {
						dueAt = dueAt.AddMinutes(time);
						break;
					}
				case "hour":
				case "hours": {
						dueAt = dueAt.AddHours(time);
						break;
					}
				case "day":
				case "days": {
						dueAt = dueAt.AddDays(time);
						break;
					}
				case "week":
				case "weeks": {
						dueAt = dueAt.AddDays(time * 7);
						break;
					}
				case "month":
				case "months": {
						dueAt = dueAt.AddMonths(time);
						break;
					}
				case "year":
				case "years": {
						dueAt = dueAt.AddYears(time);
						break;
					}
				default: {
						await context.RespondAsync("Unknown unit of time. Supported: minute, hour, day, week, month, year");
						return;
					}
			}
			if (dueAt > DateTime.UtcNow.AddYears(5) ){
				await context.RespondAsync("Reminders set more than 5 years in the future are not supported");
				return;
			}

			Reminder reminder = new Reminder(dueAt, message, context.User.Id);
			this._repository.AddReminder(reminder);
			await context.RespondAsync(embed: reminder.CreateEmbed());
		}
	}
}