using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IRolesManagementService {
		Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs);
	}

	public class RolesManagementService : IRolesManagementService {
		readonly ApplicationDbContext dbContext;

		public RolesManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public async Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs) {
			var items = await dbContext.Roles
				.AsQueryable()
				.PerformLoadPagedData(loadDataArgs, nameof(RoleModel.Name));
			return items
				.Select(x => new RoleModel() {
					Id = x.Id,
					Name = x.Name
				});
		}
	}
}
