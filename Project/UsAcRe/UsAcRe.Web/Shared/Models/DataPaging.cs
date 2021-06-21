namespace UsAcRe.Web.Shared.Models {
	public class DataPaging {
		public int? Skip { get; set; }
		public int? Top { get; set; }
		public string OrderBy { get; set; }
		public string Filter { get; set; }
	}
}
