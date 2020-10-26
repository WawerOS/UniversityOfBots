/**
This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

namespace Gauss.Models.Elections {
	/// <summary>
	/// Possible states a user can have in regards to eligiblity to vote in a given election.
	/// </summary>
	public enum VoteStatus {
		CanVote,
		AlreadyVoted,
		NotAllowed, // Currently unused.
		ElectionNotFound,
		ElectionNotStarted,
		ElectionOver,
	}
}