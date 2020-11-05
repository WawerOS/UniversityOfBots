/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Gauss.Models {

	public enum FilterMode {
		Disabled,
		Whitelist,
		Blacklist,
	}

	public class FilterEntry {
		[Key]
		public ulong UserId { get; set; }
		public string Username { get; set; }
	}

	public class UserVoiceSettings {

		public ulong GuildId { get; set; }
		public ulong UserId { get; set; }
		public bool IsActive { get; set; }

		public FilterMode FilterMode { get; set; } = FilterMode.Disabled;
		public List<FilterEntry> TargetUsers { get; set; } = new List<FilterEntry>();
		public UserStatus TargetStatus { get; set; } = UserStatus.Idle;

		public bool IsInTimeout { get; set; }

		public bool CheckFilter(ulong userId) {
			switch(this.FilterMode){
				case FilterMode.Whitelist: {
					return this.TargetUsers.Any(y => y.UserId == userId);
				}
				case FilterMode.Blacklist: {
					return !this.TargetUsers.Any(y => y.UserId == userId);
				}
				default: {
					return true;
				}
			}
		}
	}

}