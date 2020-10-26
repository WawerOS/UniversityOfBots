/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Threading.Tasks;

namespace Gauss.Scheduling {
	/// <summary>
	/// A shell of a task to customize fully by the instanciating site. 
	/// ShouldRun() and Run() are just delegates and this task never completes.
	/// </summary>
	public class TaskThunk : IScheduledTask {
		private readonly Func<bool> _shouldRun;
		private readonly Func<Task> _action;

		public bool IsComplete => false;

		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="shouldRun">
		/// Delegate to determine whether or not the main logic should run.
		/// </param>
		/// <param name="action">
		/// Delegate containing the main logic of the task.
		/// </param>
		public TaskThunk(Func<bool> shouldRun, Func<Task> action) {
			this._shouldRun = shouldRun;
			this._action = action;
		}

		public Task Run() {
			return Task.Run(() => this._action());
		}

		public bool ShouldRun() {
			return this._shouldRun();
		}
	}

}