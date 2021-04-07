using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using GuardNet;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IRolesManagementService {
		IEnumerable<RoleModel> List(LoadDataArgs loadDataArgs);
	}

	public class RolesManagementService : IRolesManagementService {
		readonly ApplicationDbContext dbContext;

		public RolesManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public IEnumerable<RoleModel> List(LoadDataArgs loadDataArgs) {
			var query = dbContext.Roles.AsQueryable();

			if(!string.IsNullOrEmpty(loadDataArgs.Filter)) {
				query = query.Where(loadDataArgs.Filter);
			}

			if(!string.IsNullOrEmpty(loadDataArgs.OrderBy)) {
				query = query.OrderBy(loadDataArgs.OrderBy);
			}

			return query
				.Skip(loadDataArgs.Skip.Value)
				.Take(loadDataArgs.Top.Value)
				.Select(x => new RoleModel() {
					Id = x.Id,
					Name = x.Name
				});
		}
	}
}
