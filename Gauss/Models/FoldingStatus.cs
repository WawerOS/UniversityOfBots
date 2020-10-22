using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gauss.Models {
	public class FoldingStatus {
		[JsonProperty("wus")]
		public int WorkUnits { get; set; }
		[JsonProperty("rank")]
		public double Rank { get; set; }
		[JsonProperty("total_teams")]
		public double TeamsTotal { get; set; }
		[JsonProperty("credit")]
		public int Credit { get; set; }
		[JsonProperty("team")]
		public int Team { get; set; }
		[JsonProperty("last")]
		public DateTime Last { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("donors")]
		public List<FoldingDonor> Donors { get; set; }

		public int MonthlyRank { get; set; }

		[JsonProperty("retrieval_time")]
		public DateTime RetrievalTime { get; set; }
	}

	public class FoldingDonor {
		public int wus { get; set; }
		public string name { get; set; }
		public int credit { get; set; }
		public int id { get; set; }
	}
}