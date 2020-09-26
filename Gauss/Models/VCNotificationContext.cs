/**
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
**/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gauss.Models {

	public enum FilterMode {
		Disabled,
		Whitelist,
		Blacklist,
	}

    public class FilterEntry{
        [Key]
        public ulong UserId {get;set;}
        public string Username {get;set;}
    }

    public class VCNotificationConfig {
        
        public ulong GuildId {get;set;}      
         public ulong UserId {get;set;}
        public bool IsActive {get;set;}

        public FilterMode FilterMode { get; set; } = FilterMode.Disabled;
		public List<FilterEntry> TargetUsers { get; set; } = new List<FilterEntry>();
		public UserStatus TargetStatus { get; set; } = UserStatus.Idle;

        public bool IsInTimeout {get;set;}
    }

    public class VCNotificationContext : DbContext {
        public  DbSet<VCNotificationConfig> Configs {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=botdata.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VCNotificationConfig>()
                .HasKey(e => new { e.GuildId, e.UserId});
        }
        public VCNotificationContext()
        {
            this.Database.EnsureCreated();
        }
    }

}