using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
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

		//static bool Compare<T>(FilterOperator filterOperator, T field, T value) where T : IComparable<T> =>
		//	filterOperator switch {
		//		FilterOperator.Equals => value.CompareTo(field) == 0,
		//		FilterOperator.NotEquals => value.CompareTo(field) != 0,
		//		FilterOperator.LessThan => value.CompareTo(field) < 0,
		//		FilterOperator.LessThanOrEquals => value.CompareTo(field) <= 0,
		//		FilterOperator.GreaterThan => value.CompareTo(field) > 0,
		//		FilterOperator.GreaterThanOrEquals => value.CompareTo(field) >= 0,
		//		_ => throw new ArgumentException(),
		//	};

		static IComparable ConvertToComparable(object field, IComparable val) =>
			val switch {
				Byte => Convert.ToByte(field),
				Int16 => Convert.ToInt16(field),
				Int32 => Convert.ToInt32(field),
				Int64 => Convert.ToInt64(field),
				SByte => Convert.ToSByte(field),
				UInt16 => Convert.ToUInt16(field),
				UInt32 => Convert.ToUInt32(field),
				UInt64 => Convert.ToUInt64(field),
				Decimal => Convert.ToDecimal(field),
				Double => Convert.ToDouble(field),
				Single => Convert.ToSingle(field),
				_ => Convert.ToString(field),
			};

		static IComparable ConvertToComparable<T>(object field) =>
			ConvertToComparable(field, default(T) as IComparable);

		enum CompareResult {
			NoComparable,
			Less,
			Equal,
			Greater,
			NotEqual,
		}

		static CompareResult GetCompareResult(int value) =>
			value switch {
				-1 => CompareResult.Less,
				0 => CompareResult.Equal,
				1 => CompareResult.Greater,
				_ => CompareResult.NoComparable
			};


		static CompareResult CompareTo(object field, object value) =>
			(field, value) switch {
				(null, null) => CompareResult.Equal,
				(null, not null) => CompareResult.Greater,
				(_, null) => CompareResult.NoComparable,

				(String fieldVal, String stringVal) => GetCompareResult(fieldVal.CompareTo(stringVal)),
				(String stringVal, Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) when Double.TryParse(stringVal, out Double doubleVal) => GetCompareResult(doubleVal.CompareTo(value)),
				(Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte, String stringVal) when Double.TryParse(stringVal, out Double doubleVal) => GetCompareResult(doubleVal.CompareTo(field)),

				(Int32 fieldVal, Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Single fieldVal, Single or Int32 or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Double fieldVal, Double or Int32 or Single or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Int64 fieldVal, Int64 or Int32 or Single or Double or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Decimal fieldVal, Decimal or Int32 or Single or Double or Int64 or UInt32 or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(UInt32 fieldVal, UInt32 or Int32 or Single or Double or Int64 or Decimal or UInt16 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(UInt16 fieldVal, UInt16 or Int32 or Single or Double or Int64 or Decimal or UInt32 or Byte or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Byte fieldVal, Byte or Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Int16 or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(Int16 fieldVal, Int16 or Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or SByte) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),
				(SByte fieldVal, SByte or Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16) => GetCompareResult(fieldVal.CompareTo(ConvertToComparable(value, fieldVal))),

				(JsonElement { ValueKind: JsonValueKind.Number } jsonElement, Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte) =>
							GetCompareResult(jsonElement.GetDouble().CompareTo(ConvertToComparable<Double>(value))),
				(JsonElement { ValueKind: JsonValueKind.Number } jsonElement, String stringVal) when Double.TryParse(stringVal, out Double doubleVal) => GetCompareResult(jsonElement.GetDouble().CompareTo(doubleVal)),
				(JsonElement { ValueKind: JsonValueKind.String } jsonElement, _) => GetCompareResult(jsonElement.GetString().CompareTo(value)),
				(JsonElement jsonElement, _) => GetCompareResult(jsonElement.GetRawText().CompareTo(value)),

				(Int32 or Single or Double or Int64 or Decimal or UInt32 or UInt16 or Byte or Int16 or SByte, JsonElement { ValueKind: JsonValueKind.Number } jsonElement) =>
							GetCompareResult(jsonElement.GetDouble().CompareTo(ConvertToComparable<Double>(field))),
				(String stringVal, JsonElement { ValueKind: JsonValueKind.Number } jsonElement) when Double.TryParse(stringVal, out Double doubleVal) => GetCompareResult(jsonElement.GetDouble().CompareTo(doubleVal)),
				(_, JsonElement { ValueKind: JsonValueKind.String } jsonElement) => GetCompareResult(jsonElement.GetString().CompareTo(field)),
				(_, JsonElement jsonElement) => GetCompareResult(jsonElement.GetRawText().CompareTo(field)),

				(IComparable fieldVal, IComparable val) when fieldVal.GetType().IsAssignableFrom(val.GetType()) => GetCompareResult(fieldVal.CompareTo(val)),
				(_, _) when field.GetType().IsAssignableFrom(value.GetType()) => field == value ? CompareResult.Equal : CompareResult.NotEqual,
				(_, _) => CompareResult.NoComparable

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
			{FilterOperator.Equals, (field, value) => CompareTo(field, value) == CompareResult.Equal},
			{FilterOperator.NotEquals, (field, value) => CompareTo(field, value) != CompareResult.Equal},
			//{FilterOperator.LessThan, (field, value) => Compare(FilterOperator.LessThan, field, value)},
			//{FilterOperator.LessThanOrEquals, (field, value) => Compare(FilterOperator.LessThanOrEquals, field, value)},
			//{FilterOperator.GreaterThan, (field, value) => Compare(FilterOperator.GreaterThan, field, value)},
			//{FilterOperator.GreaterThanOrEquals, (field, value) => Compare(FilterOperator.GreaterThanOrEquals, field, value)},
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
