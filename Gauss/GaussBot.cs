/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Gauss.Models;

namespace Gauss {
    public class GaussBot{
        private readonly DiscordClient _client;

		public GaussBot(GaussConfig config){
            this._client = new DiscordClient(new DiscordConfiguration{
                Token = config.DiscordToken,
            });

            var commandConfig = new CommandsNextConfiguration {
				StringPrefixes = new List<string>() { config.CommandPrefix },
				EnableDms = true,
				EnableMentionPrefix = true,
			};

			this._client.UseCommandsNext(commandConfig);
        }

        public void Connect(){
            Task.Run(() => this._client.ConnectAsync());
        }

        public void Disconnect() {
            this._client.DisconnectAsync().GetAwaiter().GetResult();
            this._client.Dispose();
        }
    }
}