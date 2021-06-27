namespace UsAcRe.Web.Shared.Models {
	public enum LogicalFilterOperator {
		And,
		Or
	}

	public enum FilterOperator {
		Equals,
		NotEquals,
		LessThan,
		LessThanOrEquals,
		GreaterThan,
		GreaterThanOrEquals,
		Contains,
		StartsWith,
		EndsWith
	}

	public class FilterDescriptor {
		public string Field { get; set; }
		public object FilterValue { get; set; }
		public FilterOperator FilterOperator { get; set; }
		public object SecondFilterValue { get; set; }
		public FilterOperator SecondFilterOperator { get; set; }
		public LogicalFilterOperator LogicalFilterOperator { get; set; }
	}
}
