/*!
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Database;
using Gauss.Models;
using Gauss.Utilities;

namespace Gauss.CommandAttributes {
	public class RequireAdminAttribute : CheckBaseAttribute {
		private GuildSettingsContext _context = null;

		public override Task<bool> ExecuteCheckAsync(CommandContext context, bool help) {
			if (help) {
				return Task.FromResult(true);
			}

			var guild = context.GetGuild();
			var member = guild.Members[context.User.Id];
			var isBotOwner = context.Client.CurrentApplication.Owners.Contains(context.User);
			if ((member != null && member.IsOwner) || isBotOwner) {
				return Task.FromResult(true);
			}

			if (_context == null) {
				_context = (GuildSettingsContext)context.Services.GetService(typeof(GuildSettingsContext));
			}
			return Task.FromResult(
				_context.IsAdminRole(guild.Id, member?.Roles)
			);
		}
	}

}