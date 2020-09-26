/*!
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Models;
using Gauss.Utilities;

namespace Gauss.CommandAttributes {
	public class RequireAdmin : CheckBaseAttribute {
		private readonly GaussConfig _config;

		public RequireAdmin() {
			this._config = GaussConfig.GetInstance();
		}

		public override Task<bool> ExecuteCheckAsync(CommandContext context, bool help) {
			if (help) {
				return Task.FromResult(true);
			}
			if (this._config.AdminRoles == null || this._config.AdminRoles.Count == 0) {
				return Task.FromResult(false);
			}
			var guild = context.GetGuild();
			return Task.FromResult(
				guild.Members[context.User.Id].Roles.Any(
					y => this._config.AdminRoles.Contains(y.Id)
			));
		}
	}

}