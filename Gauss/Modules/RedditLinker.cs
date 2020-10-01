using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Gauss.Modules {

	class RedditLinker {
		public RedditLinker(DiscordClient client) {
			client.MessageCreated += this.HandleNewMessage;
		}

		public async Task HandleNewMessage(MessageCreateEventArgs e) {
			if (e.Author.IsBot) {
				return;
			}
			var redditRegex = new Regex(@"^([^>]* | *)(\/?r\/\w{2,})(?=[^>]*$)", RegexOptions.Multiline);
			var redditMatches = redditRegex.Matches(e.Message.Content);
			List<string> validSubs = new List<string>();
            List<string> nsfwSubs = new List<string>();

			foreach (Match match in redditMatches) {
				if (match.Groups.Count >= 3) {
					string subreddit = match.Groups[2].Value.Trim();
					subreddit = subreddit.StartsWith("/") ? subreddit : "/" + subreddit;
                    string content = "";
					try {
						var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", "Gauss discord bot");
						var result = await client.GetAsync($"https://api.reddit.com/{subreddit}/about");
                        content = await result.Content.ReadAsStringAsync();
						var json = JsonDocument.Parse(content);
						if (!json.RootElement.GetProperty("data").GetProperty("over18").GetBoolean()) {
                            validSubs.Add(subreddit);
						}else{
                            nsfwSubs.Add(subreddit);
                        }
					} catch (Exception) {
						// TODO
					}
				}
			}

            string answer = "";

			if (validSubs.Count > 0) {
                answer = validSubs.Count == 1
                    ? $"Here is a clickable link: <https://www.reddit.com{validSubs[0]}>"
                    : "Here are clickable links:\n" + string.Join("\n", validSubs.Select(y => $"<https://www.reddit.com{y}>"));
			}
            if (nsfwSubs.Count > 0) {
                answer += nsfwSubs.Count ==  1
                    ? $"Warning: {nsfwSubs[0]} is NSFW!"
                    : $"Warning! These subs are NSFW:" + string.Join(", ", validSubs);
            }
            if (nsfwSubs.Count == 0 && validSubs.Count == 0){
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(e.Client, ":person_shrugging:"));
            }else{
                await e.Message.RespondAsync(answer);
            }
		}
	}
}