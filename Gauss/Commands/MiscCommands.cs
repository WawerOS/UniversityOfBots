/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.CommandAttributes;

namespace Gauss.Commands {
	[NotBot]
	[CheckDisabled]
	public class MiscCommands : BaseCommandModule {
		[Description("Get a link to detailed documentation")]
		[Command("docs")]
		[Aliases("documentation")]
		public async Task GetDocumentation(CommandContext context) {
			await context.RespondAsync("https://stringepsilon.github.io/UniversityOfBots/");
		}

		[Description("Get information about your privacy and this Bot.")]
		[Command("privacy")]
		public async Task GetPrivacyInfo(CommandContext context) {
			await context.RespondAsync("You can find up to date privacy information in the link below.\n" +
				"https://stringepsilon.github.io/UniversityOfBots/PRIVACY\n");
		}
	}
}