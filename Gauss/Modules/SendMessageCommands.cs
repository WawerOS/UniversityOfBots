using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;


namespace Gauss.Commands {
    public class SendMessageCommands: BaseCommandModule {
        [Command("send")]
        public async Task SendMessage(CommandContext context){
            await context.RespondAsync("This feature is not yet implemented.");
        }
    }
}