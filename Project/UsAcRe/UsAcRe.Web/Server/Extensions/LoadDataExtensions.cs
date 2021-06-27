using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Extensions {
	public static class LoadDataExtensions {
		public static Task<List<TEntity>> PerformLoadPagedData<TEntity>(this IQueryable<TEntity> query, DataPaging dataPaging) {
			if(dataPaging.Filters.Any()) {
				foreach(var filter in dataPaging.Filters) {
					//query = query.Where(dataPaging.Filter);
				}
			}


			var orderedQuery = query as IOrderedQueryable<TEntity>;
			bool useThenBy = false;
			foreach(var sort in dataPaging.Sorts) {
				switch(sort.SortOrder) {
					case Shared.Models.SortOrder.Descending:
						if(useThenBy) {
							orderedQuery = orderedQuery.ThenBy($"{sort.Field} desc");
						} else {
							orderedQuery = orderedQuery.OrderBy($"{sort.Field} desc");
						}
						break;

					default:
						if(useThenBy) {
							orderedQuery = orderedQuery.ThenBy($"{sort.Field} asc");
						} else {
							orderedQuery = orderedQuery.OrderBy($"{sort.Field} asc");
						}
						break;
				}
				useThenBy = true;
			}

			var pagedQuery = orderedQuery as IQueryable<TEntity>;
			if(dataPaging.Skip.HasValue) {
				pagedQuery = pagedQuery.Skip(dataPaging.Skip.Value);
			}
			if(dataPaging.Top.HasValue) {
				pagedQuery = pagedQuery.Take(dataPaging.Top.Value);
			}
			return pagedQuery.ToListAsync();
		}
	}
}
