/*!
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Gauss.Models;

namespace Gauss.Modules {
	[ModuleInactive]
	public class RoleAssign : BaseModule {
		private readonly DiscordClient _client;
		private readonly GaussConfig _config;

		public RoleAssign(DiscordClient client, GaussConfig config) {
			_client = client;
			_config = config;

			this._client.GuildMemberAdded += this.OnGuildMemberAdded;
		}

		private Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs e) {
			// Don't block event handler, by running this in the background:
			return Task.Run(async () => {
				// Might need to redo this, for when multiple assigned roles are needed.
				if (e.Guild.Roles.ContainsKey(this._config.AutoAssignedRole)) {
					var role = e.Guild.Roles[this._config.AutoAssignedRole];
					await e.Member.GrantRoleAsync(role);
				} else if (this._config.AutoAssignedRole != 0) {
					throw new Exception(
						$"The role with id '{this._config.AutoAssignedRole}' could not be found and assigned to '{e.Member.DisplayName}'."
					);
				}
			});
		}
	}
}