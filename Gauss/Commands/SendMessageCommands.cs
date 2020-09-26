/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.Utilities;

namespace Gauss.Commands {
	public class SendMessageCommands : BaseCommandModule {
		[Description("Send a message anonymously through the bot to a channel")]
		[Command("send")]
		public async Task SendMessage(
			CommandContext context,
			[Description("Name of the channel you want to post in.")]
			string channelName,
			[Description("Your message")]
			[RemainingText] string message
		) {
			var guild = context.GetGuild();
            var member = guild.Members[context.User.Id];
			var channel = guild.Channels.Values.SingleOrDefault(y => y.Name == channelName);
			if (channel != null) {
                if (channel.PermissionsFor(member).HasFlag(DSharpPlus.Permissions.SendMessages)){
                    await channel.SendMessageAsync(message);
                }else{
                    await channel.SendMessageAsync($"Can't send your message to {channel.Name}.");
                }
			} else {
				await context.RespondAsync($"Channel '{channelName}' could not be found.");
			}
		}
	}
}