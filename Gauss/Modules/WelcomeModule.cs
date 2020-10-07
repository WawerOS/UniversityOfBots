using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Gauss.Models;

namespace Gauss.Modules {
	public class WelcomeModule {
		private readonly GaussConfig _config;
		private DiscordMessage _lastMessage;
		private readonly Regex _triggerExpression = new Regex("(met gauss|meet gauss)", RegexOptions.IgnoreCase);

		public WelcomeModule(DiscordClient client, GaussConfig config) {
			client.MessageCreated += this.HandleNewMessage;
			this._config = config;
		}

		public Task HandleNewMessage(MessageCreateEventArgs e) {
			return Task.Run(async () => {
				if (e.Channel.IsPrivate) {
					return;
				}
				
				_config.WelcomeChannel.TryGetValue(e.Guild.Id, out ulong welcomeChannel);
				_config.WelcomeMessage.TryGetValue(e.Guild.Id, out string welcomeMessage);
				if (welcomeChannel == 0 || welcomeMessage == null) {
					return;
				}
				if (e.Channel.Id != welcomeChannel) {
					return;
				}

				if (_triggerExpression.IsMatch(e.Message.Content)) {
					if (this._lastMessage != null) {
						Console.WriteLine(this._lastMessage.CreationTimestamp);
						Console.WriteLine(DateTime.UtcNow.AddHours(-1));
						await this._lastMessage.ModifyAsync("[This previously contained the welcome message]");
					}
					this._lastMessage = await e.Channel.SendMessageAsync(welcomeMessage);
				}
			});
		}
	}
}
