/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Gauss.Utilities {
	public static class DiscordGuildExtensions {
		public static DiscordMember FindMember(this DiscordGuild guild, string query) {
			foreach (var member in guild.Members.Values) {
				if (member.Username.ToLower() == query.ToLower()) {
					return member;
				}
				if (member.DisplayName.ToLower() == query.ToLower()) {
					return member;
				}
				if (member.Mention == query) {
					return member;
				}
			}
			return null;
		}
	}
}