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
			client.MessageDeleted += this.HandleMessageDeletion;
			this._config = config;
		}

		private Task HandleMessageDeletion(DiscordClient client,MessageDeleteEventArgs e) {
			if (e.Message == this._lastMessage) {
				this._lastMessage = null;
			}
			return Task.CompletedTask;
		}

		public Task HandleNewMessage(DiscordClient client, MessageCreateEventArgs e) {
			if (e.Channel.IsPrivate || e.Author.IsBot) {
				return Task.CompletedTask;
			}
			return Task.Run(async () => {
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
						await this._lastMessage.ModifyAsync("[This previously contained the welcome message]");
					}
					this._lastMessage = await e.Channel.SendMessageAsync(welcomeMessage);
				}
			});
		}
	}
}
