using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

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

		static new bool Equals(object field, object value) =>
			value switch {
				string val => val.Equals(field),
				JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field == val.GetDouble(),
				JsonElement val => field != null && field as string == val.GetString(),
				_ => field == value,
			};
		static bool NotEquals(object field, object value) =>
			value switch {
				string val => !val.Equals(field),
				JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field != val.GetDouble(),
				JsonElement val => field != null && field as string != val.GetString(),
				_ => field != value,
			};

		static bool LessThan(object field, object value) =>
			   value switch {
				   int val => field != null && (int)field < val,
				   double val => field != null && (double)field < val,
				   JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field < val.GetDouble(),
				   _ => false,
			   };
		static bool LessThanOrEquals(object field, object value) =>
			   value switch {
				   int val => field != null && (int)field <= val,
				   double val => field != null && (double)field <= val,
				   JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field <= val.GetDouble(),
				   _ => false,
			   };

		static bool GreaterThan(object field, object value) =>
			   value switch {
				   int val => field != null && (int)field > val,
				   double val => field != null && (double)field > val,
				   JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field > val.GetDouble(),
				   _ => false,
			   };
		static bool GreaterThanOrEquals(object field, object value) =>
			   value switch {
				   int val => field != null && (int)field >= val,
				   double val => field != null && (double)field >= val,
				   JsonElement val when val.ValueKind == JsonValueKind.Number => field != null && (double)field >= val.GetDouble(),
				   _ => false,
			   };
		static bool Contains(object field, object value) =>
			field switch {
				string fld when value is string val => fld.Contains(val, StringComparison.OrdinalIgnoreCase),
				string fld when value is JsonElement val => fld.Contains(val.GetString(), StringComparison.OrdinalIgnoreCase),
				_ => false,
			};
		static bool StartsWith(object field, object value) =>
			field switch {
				string fld when value is string val => fld.StartsWith(val, StringComparison.OrdinalIgnoreCase),
				string fld when value is JsonElement val => fld.StartsWith(val.GetString(), StringComparison.OrdinalIgnoreCase),
				_ => false,
			};
		static bool EndsWith(object field, object value) =>
			field switch {
				string fld when value is string val => fld.EndsWith(val, StringComparison.OrdinalIgnoreCase),
				string fld when value is JsonElement val => fld.EndsWith(val.GetString(), StringComparison.OrdinalIgnoreCase),
				_ => false,
			};

		public static Dictionary<FilterOperator, Func<object, object, bool>> Predicates = new Dictionary<FilterOperator, Func<object, object, bool>>() {
			{FilterOperator.Equals, (field, value) => Equals(field, value)},
			{FilterOperator.NotEquals, (field, value) => NotEquals(field, value)},
			{FilterOperator.LessThan, (field, value) => field == null ? false : LessThan(field, value)},
			{FilterOperator.LessThanOrEquals, (field, value) => field == null ? false : LessThanOrEquals(field, value)},
			{FilterOperator.GreaterThan, (field, value) => field == null ? false : GreaterThan(field, value)},
			{FilterOperator.GreaterThanOrEquals, (field, value) => field == null ? false : GreaterThanOrEquals(field, value)},
			{FilterOperator.Contains, (field, value) => field == null ? false : Contains(field, value)},
			{FilterOperator.StartsWith, (field, value) => field == null ? false : StartsWith(field, value)},
			{FilterOperator.EndsWith, (field, value) => field == null ? false : EndsWith(field, value)},
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
