using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

	public static class FilterOperatorSpecifics {
		static string Equals(string field, object value) =>
		value switch {
			int or double => string.Format("({0}={1})", field, value),
			_ => string.Format("({0}=\"{1}\")", field, value),
		};
		static string NotEquals(string field, object value) =>
			value switch {
				int or double => string.Format("({0}!={1})", field, value),
				_ => string.Format("({0}!=\"{1}\")", field, value),
			};
		static string LessThan(string field, object value) =>
			value switch {
				int or double => string.Format("({0}<{1})", field, value),
				_ => string.Format("({0}<\"{1}\")", field, value),
			};
		static string LessThanOrEquals(string field, object value) =>
			value switch {
				int or double => string.Format("({0}<={1})", field, value),
				_ => string.Format("({0}<=\"{1}\")", field, value),
			};
		static string GreaterThan(string field, object value) =>
			value switch {
				int or double => string.Format("({0}>{1})", field, value),
				_ => string.Format("({0}>\"{1}\")", field, value),
			};
		static string GreaterThanOrEquals(string field, object value) =>
			value switch {
				int or double => string.Format("({0}>={1})", field, value),
				_ => string.Format("({0}>=\"{1}\")", field, value),
			};
		static string Contains(string field, object value) =>
			value switch {
				_ => $"({field} == null ? \"\" : {field}).ToLower().Contains(\"{value}\".ToLower())",
			};
		static string StartsWith(string field, object value) =>
			value switch {
				_ => $"({field} == null ? \"\" : {field}).ToLower().StartsWith(\"{value}\".ToLower())",
			};
		static string EndsWith(string field, object value) =>
			value switch {
				_ => $"({field} == null ? \"\" : {field}).ToLower().EndsWith(\"{value}\".ToLower())",
			};

		public static Dictionary<FilterOperator, Func<string, object, string>> Expressions = new Dictionary<FilterOperator, Func<string, object, string>>() {
			{FilterOperator.Equals, (field, value) => Equals(field, value)},
			{FilterOperator.NotEquals, (field, value) => NotEquals(field, value)},
			{FilterOperator.LessThan, (field, value) => LessThan(field, value)},
			{FilterOperator.LessThanOrEquals, (field, value) => LessThanOrEquals(field, value)},
			{FilterOperator.GreaterThan, (field, value) => GreaterThan(field, value)},
			{FilterOperator.GreaterThanOrEquals, (field, value) => GreaterThanOrEquals(field, value)},
			{FilterOperator.Contains, (field, value) => Contains(field, value)},
			{FilterOperator.StartsWith, (field, value) => StartsWith(field, value)},
			{FilterOperator.EndsWith, (field, value) => EndsWith(field, value)},

		};
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
