using System.Collections.Generic;
using System.Linq;

namespace UsAcRe.Web.Client.Extensions {
	public static class FilterDescriptorExtensions {
		public static Web.Shared.Models.LogicalFilterOperator ToLogicalFilterOperator(this Radzen.LogicalFilterOperator sortOrder) {
			switch(sortOrder) {
				case Radzen.LogicalFilterOperator.And:
					return Web.Shared.Models.LogicalFilterOperator.And;
				default:
					return Web.Shared.Models.LogicalFilterOperator.Or;
			}
		}
		public static Web.Shared.Models.FilterOperator ToFilterOperator(this Radzen.FilterOperator sortOrder) {
			switch(sortOrder) {
				case Radzen.FilterOperator.Equals:
					return Web.Shared.Models.FilterOperator.Equals;
				case Radzen.FilterOperator.NotEquals:
					return Web.Shared.Models.FilterOperator.NotEquals;
				case Radzen.FilterOperator.LessThan:
					return Web.Shared.Models.FilterOperator.LessThan;
				case Radzen.FilterOperator.LessThanOrEquals:
					return Web.Shared.Models.FilterOperator.LessThanOrEquals;
				case Radzen.FilterOperator.GreaterThan:
					return Web.Shared.Models.FilterOperator.GreaterThan;
				case Radzen.FilterOperator.GreaterThanOrEquals:
					return Web.Shared.Models.FilterOperator.GreaterThanOrEquals;
				case Radzen.FilterOperator.Contains:
					return Web.Shared.Models.FilterOperator.Contains;
				case Radzen.FilterOperator.StartsWith:
					return Web.Shared.Models.FilterOperator.StartsWith;
				case Radzen.FilterOperator.EndsWith:
					return Web.Shared.Models.FilterOperator.EndsWith;
				default:
					return Web.Shared.Models.FilterOperator.EndsWith;
			}
		}

		public static Web.Shared.Models.FilterDescriptor ToFilterDescriptor(this Radzen.FilterDescriptor filterDescriptor) {
			return new Web.Shared.Models.FilterDescriptor() {
				Field = filterDescriptor.Property,
				FilterValue = filterDescriptor.FilterValue,
				FilterOperator = filterDescriptor.FilterOperator.ToFilterOperator(),
				SecondFilterValue = filterDescriptor.SecondFilterValue,
				SecondFilterOperator = filterDescriptor.SecondFilterOperator.ToFilterOperator(),
				LogicalFilterOperator = filterDescriptor.LogicalFilterOperator.ToLogicalFilterOperator()
			};
		}

		public static IEnumerable<Web.Shared.Models.FilterDescriptor> ToFilterDescriptors(this IEnumerable<Radzen.FilterDescriptor> filterDescriptors) {
			return filterDescriptors.Select(x => x.ToFilterDescriptor());
		}
	}
}
