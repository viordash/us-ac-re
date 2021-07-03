using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Radzen;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Extensions {
	public static class FilteringExtensions {
		static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> query, Shared.Models.FilterDescriptor filter) {
			var expr = FilterOperatorSpecifics.Expressions[filter.FilterOperator](filter.Field, filter.FilterValue);
			if(filter.SecondFilterValue == null) {
				query = query.Where(expr);
			} else {
				var secondExpr = FilterOperatorSpecifics.Expressions[filter.SecondFilterOperator](filter.Field, filter.SecondFilterValue);
				switch(filter.LogicalFilterOperator) {
					case Shared.Models.LogicalFilterOperator.And:
						query = query.Where(string.Format("({0} and {1})", expr, secondExpr));
						break;
					case Shared.Models.LogicalFilterOperator.Or:
						query = query.Where(string.Format("({0} or {1})", expr, secondExpr));
						break;
				}
			}
			return query;
		}
		public static IQueryable<TEntity> ApplyFilters<TEntity>(this IQueryable<TEntity> query, IEnumerable<Shared.Models.FilterDescriptor> filters) {
			if(filters == null) {
				return query;
			}
			foreach(var filter in filters) {
				query = query.ApplyFilter(filter);
			}
			return query;
		}

		static object GetFilterField<TEntity>(TEntity entity, Shared.Models.FilterDescriptor filter) =>
			entity switch {
				UserModel userModel when filter.Field == nameof(UserModel.Id) => userModel.Id,
				UserModel userModel when filter.Field == nameof(UserModel.UserName) => userModel.UserName,
				UserModel userModel when filter.Field == nameof(UserModel.Email) => userModel.Email,
				UserModel userModel when filter.Field == nameof(UserModel.Roles) => userModel.Roles,
				UserModel userModel when filter.Field == UserModel.RolesNamesField => UserRolesView.Concat(userModel),
				_ => null,
			};

		static IEnumerable<TEntity> ApplyFilter<TEntity>(this IEnumerable<TEntity> query, Shared.Models.FilterDescriptor filter) {
			var predicate = FilterOperatorSpecifics.Predicates[filter.FilterOperator];
			if(filter.SecondFilterValue == null) {
				query = query.Where(x => predicate(GetFilterField(x, filter), filter.FilterValue)).ToList();
			} else {
				var secondPredicate = FilterOperatorSpecifics.Predicates[filter.SecondFilterOperator];
				switch(filter.LogicalFilterOperator) {
					case Shared.Models.LogicalFilterOperator.And:
						query = query.Where(x => predicate(GetFilterField(x, filter), filter.FilterValue) && secondPredicate(GetFilterField(x, filter), filter.SecondFilterValue));
						break;
					case Shared.Models.LogicalFilterOperator.Or:
						query = query.Where(x => predicate(GetFilterField(x, filter), filter.FilterValue) || secondPredicate(GetFilterField(x, filter), filter.SecondFilterValue));
						break;
				}
			}
			return query;
		}

		public static IEnumerable<TEntity> ApplyFilter<TEntity>(this IEnumerable<TEntity> users, IEnumerable<Shared.Models.FilterDescriptor> filters) {
			if(filters == null || !filters.Any()) {
				return users;
			}

			foreach(var filter in filters) {
				users = users.ApplyFilter(filter);
			}
			return users;
		}

	}
}
