/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Threading.Tasks;

namespace Gauss.Scheduling {
	public class CyclicTask : IScheduledTask {
		private DateTime _nextExecution;
		private readonly TimeSpan _cycleLength;
		private readonly Func<Task> _action;

		public bool IsComplete => false;

		public CyclicTask(TimeSpan cycleLength, DateTime firstExecution, Func<Task> action) {
			this._nextExecution = firstExecution;
			this._cycleLength = cycleLength;
			this._action = action;
		}

		public Task Run() {
			return Task.Run(() => this._action());
		}

		public bool ShouldRun() {
			var result = DateTime.UtcNow > this._nextExecution;
			if (result) {
				this._nextExecution += this._cycleLength;
			}
			return result;
		}
	}

}