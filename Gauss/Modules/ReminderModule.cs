using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Gauss.Database;
using Gauss.Scheduling;

namespace Gauss.Modules {
	/// <summary>
	/// Module to keep track of all active elections to post and update their respective messages.
	/// Also closes the election polls on scheduled end date/time.
	/// </summary>
	class ReminderModule : BaseModule {
		private readonly DiscordClient _client;
		private readonly Scheduler _scheduler;
		private readonly ReminderRepository _repository;
		private DateTime _nextCheck = DateTime.MinValue;

		public ReminderModule(
			DiscordClient client,
			Scheduler scheduler,
			ReminderRepository repository
		) {
			this._client = client;
			this._scheduler = scheduler;
			this._repository = repository;

			// Setup the UpdateElections function to run in the general scheduler:
			this._scheduler.AddTask(
				new TaskThunk(
					this.ShouldUpdateElections,
					this.UpdateElections
				)
			);
		}

		/// <summary>
		/// Election update logic.
		/// </summary>
		private async Task UpdateElections() {
			var reminders = _repository.GetReminders();
			if (reminders.Count == 0) {
				return;
			}

			foreach (var reminder in reminders.Where(reminder => reminder.IsDue())) {
				var guild = this._client.Guilds.Values.FirstOrDefault(y => y.Members.ContainsKey(reminder.UserId));

				if (guild != null) {
					var member = guild.Members[reminder.UserId];
					try {
						await member.SendMessageAsync($"You requested a reminder for: {reminder.Message}");
					} catch (Exception) {
						// Nothing to do.
					}
				}
				this._repository.RemoveReminder(reminder);
			}
			return;
		}

		/// <summary>
		/// Determines if the main task should run.
		/// </summary>
		/// <returns>
		/// True if the task should run, otherwise false.
		/// </returns>
		private bool ShouldUpdateElections() {
			// Run the main logic every 10 seconds:
			if (this._nextCheck < DateTime.UtcNow) {
				this._nextCheck = DateTime.UtcNow + TimeSpan.FromSeconds(10);
				return true;
			}
			return false;
		}
	}
}