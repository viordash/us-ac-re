using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Extensions {
	public static class LoadDataExtensions {
		static IQueryable<TEntity> ApplySorts<TEntity>(this IQueryable<TEntity> query, IEnumerable<Shared.Models.SortDescriptor> sorts) {
			if(sorts == null) {
				return query;
			}
			IOrderedQueryable<TEntity> orderedQuery = null;
			foreach(var sort in sorts) {
				switch(sort.SortOrder) {
					case Shared.Models.SortOrder.Descending:
						if(orderedQuery != null) {
							orderedQuery = orderedQuery.ThenBy($"{sort.Field} desc");
						} else {
							orderedQuery = query.OrderBy($"{sort.Field} desc");
						}
						break;

					default:
						if(orderedQuery != null) {
							orderedQuery = orderedQuery.ThenBy($"{sort.Field} asc");
						} else {
							orderedQuery = query.OrderBy($"{sort.Field} asc");
						}
						break;
				}
			}
			return orderedQuery ?? query;
		}

		public static Task<List<TEntity>> PerformLoadPagedData<TEntity>(this IQueryable<TEntity> query, DataPaging dataPaging) {
			query = query
				.ApplyFilters(dataPaging.Filters)
				.ApplySorts(dataPaging.Sorts);

			if(dataPaging.Skip.HasValue) {
				query = query.Skip(dataPaging.Skip.Value);
			}
			if(dataPaging.Top.HasValue) {
				query = query.Take(dataPaging.Top.Value);
			}
			return query.ToListAsync();
		}
	}
}
