/*!
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Gauss.Models {
	public class UserRestriction {
		public ulong GuildId { get; set; }
		public ulong UserId { get; set; }
		public ICollection<CommandRestriction> RestrictedCommands { get; set; }

		public CommandRestriction FindCommandRestriction(string query){
			if (this.RestrictedCommands == null || this.RestrictedCommands.Count == 0) {
				return null;
			}

			// TODO: This is a bit ham-fisted and needs optimization.
			CommandRestriction result = null;
			var queryFragments = query.Split(" ");
			foreach (var entry in this.RestrictedCommands)
			{
				if (entry.CommandName.ToLower() == query.ToLower()) {
					return entry;
				}
				var entryFragments = entry.CommandName.Split(" ");
				if (entryFragments.Length > queryFragments.Length){
					break;
				}
				for (int i = 0; i < entryFragments.Length; i++){
					if (entryFragments[i].ToLower() == queryFragments[i].ToLower()){
						result = entry;		
					}else{
						result = null;
						break;
					}
				}
			}

			return result;
		}
	}

	[Owned]
	public class CommandRestriction {
		public string CommandName { get; set; }
		public System.DateTime? Expiration { get; set; }
	}
}