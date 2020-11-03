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
			CultureInfo.CurrentCulture = new CultureInfo("C", false);
			string configDirectory = Path.Join(GetFolderPath(SpecialFolder.UserProfile), "GaussBot");

			if (args.Length > 0 && args[0] == "--configDir") {
				configDirectory = args[1];
			}
			GaussConfig config = GaussConfig.ReadConfig(configDirectory);

			_botInstance = new GaussBot(config);
			_botInstance.Connect();
			await Task.Delay(-1);
		}

		private static void HandleExit(object sender, EventArgs e) {
			_botInstance.Disconnect();
		}
	}
}
