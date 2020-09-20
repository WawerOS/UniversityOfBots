/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Gauss.Commands;
using Gauss.Models;
using Gauss.Modules;

namespace Gauss {
	public class GaussBot {
		private readonly DiscordClient _client;
		private readonly GaussConfig _config;
		private readonly List<object> _modules = new List<object>();

		public GaussBot(GaussConfig config) {
			this._config = config;
			this._client = new DiscordClient(new DiscordConfiguration {
				Token = config.DiscordToken,
			});

			var commandConfig = new CommandsNextConfiguration {
				StringPrefixes = new List<string>() { _config.CommandPrefix },
				EnableDms = true,
				EnableMentionPrefix = true,
			};
			this._client.UseCommandsNext(commandConfig);
			this._client.GetCommandsNext().RegisterCommands<SendMessageCommands>();
			
			
			this._modules.Add(new RoleAssign(this._client, _config));
		}

		public void Connect() {
			Task.Run(() => this._client.ConnectAsync());
		}

		public void Disconnect() {
			this._client.DisconnectAsync().GetAwaiter().GetResult();
			this._client.Dispose();
		}
	}
}