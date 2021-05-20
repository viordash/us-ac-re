using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IRolesManagementService {
		Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs);
	}

	public class RolesManagementService : IRolesManagementService {
		readonly IApplicationDbContext dbContext;

		public RolesManagementService(IApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public async Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs) {
			var query = dbContext.Roles.AsQueryable();

			if(!string.IsNullOrEmpty(loadDataArgs.Filter)) {
				query = query.Where(loadDataArgs.Filter);
			}

			string orderField;
			if(!string.IsNullOrEmpty(loadDataArgs.OrderBy)) {
				orderField = loadDataArgs.OrderBy;
			} else {
				orderField = $"{nameof(RoleModel.Name)} asc";
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

			var items = await orderedQuery
				.ToListAsync();

			return items
				.Select(x => new RoleModel() {
					Id = x.Id,
					Name = x.Name
				});
		}
	}
}
