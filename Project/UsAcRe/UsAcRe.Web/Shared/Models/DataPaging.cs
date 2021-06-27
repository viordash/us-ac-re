using System.Collections.Generic;

namespace UsAcRe.Web.Shared.Models {
	public class DataPaging {
		public int? Skip { get; set; }
		public int? Top { get; set; }
		public string Filter { get; set; }
		public IEnumerable<FilterDescriptor> Filters { get; set; }
		public IEnumerable<SortDescriptor> Sorts { get; set; }
	}
}
