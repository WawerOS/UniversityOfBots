/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gauss.Utilities;
using Gauss.Models;

namespace Gauss.Database {
	public class ReminderRepository {
		private readonly List<Reminder> _reminders;
		private readonly string _configDirectory;

		public ReminderRepository(string configDirectory) {		
			this._configDirectory = configDirectory;
			try {
				this._reminders = JsonUtility.Deserialize<List<Reminder>>(
					Path.Join(this._configDirectory, "reminders.json")
				);
			} catch (Exception) {
				this._reminders = new List<Reminder>();
			}
		}

		public void AddReminder(Reminder newReminder){
			lock(_reminders){
				var newId = this._reminders.Count > 0
					? this._reminders.Select(y => y.ID).Max() + 1
					: 1;
				newReminder.ID = newId;
				this._reminders.Add(newReminder);
			}
			this.SaveChanges();
		}

		public bool RemoveReminder(ulong userId, ulong reminderId){
			lock(_reminders){
				var reminder = this._reminders.Find(y => y.UserId == userId && y.ID == reminderId);
				if (reminder != null){
					this._reminders.Remove(reminder);
					return true;
				}
			}
			return false;
		}

		public List<Reminder> GetReminders(){
			return this._reminders;
		}


		public void SaveChanges() {
			lock (this._reminders) {
				JsonUtility.Serialize(
					Path.Join(this._configDirectory, "reminders.json"), 
					this._reminders
				);
			}
		}
	}
}