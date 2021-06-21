using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Extensions {
	public static class LoadDataExtensions {
		public static Task<List<TEntity>> PerformLoadPagedData<TEntity>(this IQueryable<TEntity> query, DataPaging dataPaging, string defaultOrderField) {
			if(!string.IsNullOrEmpty(dataPaging.Filter)) {
				query = query.Where(dataPaging.Filter);
			}

			string orderField;
			if(!string.IsNullOrEmpty(dataPaging.OrderBy)) {
				orderField = dataPaging.OrderBy;
			} else {
				orderField = $"{defaultOrderField} asc";
			}

			var orderedQuery = query
				.OrderBy(orderField)
				.AsQueryable();
			if(dataPaging.Skip.HasValue) {
				orderedQuery = orderedQuery.Skip(dataPaging.Skip.Value);
			}
			if(dataPaging.Top.HasValue) {
				orderedQuery = orderedQuery.Take(dataPaging.Top.Value);
			}
			return orderedQuery.ToListAsync();
		}
	}
}
