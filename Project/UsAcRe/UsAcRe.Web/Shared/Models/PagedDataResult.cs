using System.Collections.Generic;

namespace UsAcRe.Web.Shared.Models {
	public class PagedDataResult<TSource> {
		public IList<TSource> Data { get; set; }
		public int Total { get; set; }
		public int? Skip { get; set; }
		public int? Take { get; set; }
	}
}
