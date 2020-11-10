/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Gauss.Models;
using static System.Environment;

namespace Gauss {
	public class Program {
		private static GaussBot _botInstance;
		public async static Task Main(string[] args) {
			var taskCompletion = new TaskCompletionSource<int>();
			// Set culture to default "C" culture to ensure the bot behaves the same in terms of parsing and formatting, regardless of where it runs:
			CultureInfo.CurrentCulture = new CultureInfo("C", false);

			// Listen for SIGKILL / SIGTERM / SIGHUP to handle a graceful shutdown:
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => {
				_botInstance.Disconnect();
				taskCompletion.TrySetResult(1);
			};
			Console.CancelKeyPress += (sender, e) => {
				_botInstance.Disconnect();
				taskCompletion.TrySetResult(1);
			};

			string configDirectory = Path.Join(GetFolderPath(SpecialFolder.UserProfile), "GaussBot");

			if (args.Length > 0 && args[0] == "--configDir") {
				configDirectory = args[1];
			}
			GaussConfig config = GaussConfig.ReadConfig(configDirectory);

			// Initiate the bot itself:
			_botInstance = new GaussBot(config);
			_botInstance.Connect();

			// Wait for the program to be terminated via signal:
			await taskCompletion.Task;
		}
	}
}
