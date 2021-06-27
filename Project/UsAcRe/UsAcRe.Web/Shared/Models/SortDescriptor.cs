namespace UsAcRe.Web.Shared.Models {
	public enum SortOrder {
		Ascending,
		Descending
	}

	public class SortDescriptor {
		public string Field { get; set; }
		public SortOrder SortOrder { get; set; }
	}
}
