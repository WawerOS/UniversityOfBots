/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Gauss.CommandAttributes;
using Gauss.Models;
using Gauss.Modules;

namespace Gauss.Commands {
	[NeedsGuild]
	[NotBot]
	[CheckDisabled]
	public class FoldingCommands : BaseCommandModule {
		[Description("Get current Folding@Hom team statistics.")]
		[Command("folding")]
		[Aliases("F@H")]
		public async Task GetFoldingStats(CommandContext context) {
			await context.TriggerTypingAsync();
			FoldingStatus stats = await FoldingModule.GetFoldingStats("265832");
			if (stats != null) {
				await context.RespondAsync(
					$"```University of Bayes F@H Statistics {stats.Last:yyyy-MM-dd HH:mm:ss}\n" +
					$"Current rank: {stats.Rank:N0} overall\n" +
					$"Monthly rank: {stats.MonthlyRank:N0}\n" +
					$"Team credit: {stats.Credit:N0}\n" +
					$"WUs folded: {stats.WorkUnits:N0}\n" +
					$"Team members: {stats.Donors.Count:N0}\n" +
					$"Average credit per WU: {stats.Credit / stats.WorkUnits:N0}\n" +
					"```<https://stats.foldingathome.org/team/265832>"
				);
			} else {
				await context.RespondAsync("Error trying to fetch the F@H stats.");
			}
		}
	}
}
