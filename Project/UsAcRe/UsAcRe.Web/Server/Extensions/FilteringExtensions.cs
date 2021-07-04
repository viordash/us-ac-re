using System;
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

		static IEnumerable<TEntity> ApplyFilter<TEntity>(this IEnumerable<TEntity> query, Shared.Models.FilterDescriptor filter,
				Func<TEntity, string, object> fieldSelector) {
			var predicate = FilterOperatorSpecifics.Predicates[filter.FilterOperator];
			if(filter.SecondFilterValue == null) {
				query = query.Where(x => predicate(fieldSelector(x, filter.Field), filter.FilterValue)).ToList();
			} else {
				var secondPredicate = FilterOperatorSpecifics.Predicates[filter.SecondFilterOperator];
				switch(filter.LogicalFilterOperator) {
					case Shared.Models.LogicalFilterOperator.And:
						query = query.Where(x => predicate(fieldSelector(x, filter.Field), filter.FilterValue) && secondPredicate(fieldSelector(x, filter.Field), filter.SecondFilterValue));
						break;
					case Shared.Models.LogicalFilterOperator.Or:
						query = query.Where(x => predicate(fieldSelector(x, filter.Field), filter.FilterValue) || secondPredicate(fieldSelector(x, filter.Field), filter.SecondFilterValue));
						break;
				}
			}
			return query;
		}

		public static IEnumerable<TEntity> ApplyFilter<TEntity>(this IEnumerable<TEntity> users, IEnumerable<Shared.Models.FilterDescriptor> filters,
				Func<TEntity, string, object> fieldSelector = null) {
			if(filters == null || !filters.Any()) {
				return users;
			}

			if(fieldSelector == null) {
				fieldSelector = DefaultFilterFieldSelector;
			}

			foreach(var filter in filters) {
				users = users.ApplyFilter(filter, fieldSelector);
			}
			return users;
		}

		static object MapField<TEntity>(TEntity entity, string fieldName) {
			var propertyInfo = typeof(TEntity).GetProperty(fieldName);
			if(propertyInfo != null) {
				return propertyInfo.GetValue(entity);
			}
			var fieldInfo = typeof(TEntity).GetField(fieldName);
			if(fieldInfo != null) {
				return fieldInfo.GetValue(entity);
			}
			return false;
		}

		static object DefaultFilterFieldSelector<TEntity>(TEntity entity, string fieldName) =>
			entity switch {
				UserModel userModel => UserModel.MapField(userModel, fieldName),
				_ => MapField(entity, fieldName),
			};

	}
}
