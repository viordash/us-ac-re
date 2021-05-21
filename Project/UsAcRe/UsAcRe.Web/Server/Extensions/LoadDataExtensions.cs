using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace UsAcRe.Web.Server.Extensions {
	public static class LoadDataExtensions {
		public static Task<List<TEntity>> PerformLoadPagedData<TEntity>(this IQueryable<TEntity> query, LoadDataArgs loadDataArgs, string defaultOrderField) {
			if(!string.IsNullOrEmpty(loadDataArgs.Filter)) {
				query = query.Where(loadDataArgs.Filter);
			}

			string orderField;
			if(!string.IsNullOrEmpty(loadDataArgs.OrderBy)) {
				orderField = loadDataArgs.OrderBy;
			} else {
				orderField = $"{defaultOrderField} asc";
			}

			var orderedQuery = query
				.OrderBy(orderField)
				.AsQueryable();
			if(loadDataArgs.Skip.HasValue) {
				orderedQuery = orderedQuery.Skip(loadDataArgs.Skip.Value);
			}
			if(loadDataArgs.Top.HasValue) {
				orderedQuery = orderedQuery.Take(loadDataArgs.Top.Value);
			}
			return orderedQuery.ToListAsync();
		}
	}
}
