/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gauss.Models {

	public class MessageDataContext : DbContext {
		public DbSet<MessageRestrictedUser> RestrictedUsers { get; set; }
		public DbSet<MessageUserConfig> UserSettings { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite("Data Source=messagedata.db");

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<VCNotificationConfig>()
				.HasKey(e => new { e.GuildId, e.UserId });
		}
		public MessageDataContext() {
			this.Database.EnsureCreated();
		}
	}

	public class MessageRestrictedUser {
		[Key]
		public ulong UserId { get; set; }
		public DateTime? RestrictionEnd { get; set; }
	}

	public class MessageUserConfig {
		[Key]
		public ulong UserId { get; set; }

		public bool BlockDMs { get; set; }
	}
}