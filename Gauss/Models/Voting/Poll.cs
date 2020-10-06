/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System;

namespace Gauss.Models.Voting {

	public class Poll {
		public ulong Id { get; set; }
		public ulong GuldId { get; set; }
		public DateTime EndDate { get; set; }
		public string Description { get; set; }
		public List<PollOption> Options { get; set; }
		public List<ulong> ParticipantIds { get; set; }
	}
}