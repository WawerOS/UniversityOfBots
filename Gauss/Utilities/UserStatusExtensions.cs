/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using DSharpPlus.Entities;

namespace Gauss.Utilities {
	public static class UserStatusExtensions {
		public static bool MatchesAvailability(this UserStatus A, UserStatus target) {
			if (A == UserStatus.Offline) {
				return target == UserStatus.Offline;
			}
			if (target == UserStatus.Offline) {
				return true;
			}
			return (int)A <= (int)target;
		}
	}
}