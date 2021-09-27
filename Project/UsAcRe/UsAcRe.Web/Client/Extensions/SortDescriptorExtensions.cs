using System.Collections.Generic;
using System.Linq;

namespace UsAcRe.Web.Client.Extensions {
	public static class SortDescriptorExtensions {
		public static Web.Shared.Models.SortOrder ToSortOrder(this Radzen.SortOrder? sortOrder) {
			switch(sortOrder) {
				case Radzen.SortOrder.Descending:
					return Web.Shared.Models.SortOrder.Descending;
				default:
					return Web.Shared.Models.SortOrder.Ascending;
			}
		}

		public static Web.Shared.Models.SortDescriptor ToSortDescriptor(this Radzen.SortDescriptor sortDescriptor) {
			return new Web.Shared.Models.SortDescriptor() {
				Field = sortDescriptor.Property,
				SortOrder = sortDescriptor.SortOrder.ToSortOrder()
			};
		}

		public static IEnumerable<Web.Shared.Models.SortDescriptor> ToSortDescriptors(this IEnumerable<Radzen.SortDescriptor> sortDescriptors) {
			return sortDescriptors.Select(x => x.ToSortDescriptor());
		}
	}
}
