using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Identity;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IRolesManagementService {
		Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs);
	}

	public class RolesManagementService : IRolesManagementService {
		readonly ApplicationDbContext dbContext;
		readonly RoleManager<ApplicationIdentityRole> roleManager;

		public RolesManagementService(ApplicationDbContext dbContext, RoleManager<ApplicationIdentityRole> roleManager) {
			Guard.NotNull(dbContext, nameof(dbContext));
			Guard.NotNull(roleManager, nameof(roleManager));
			this.dbContext = dbContext;
			this.roleManager = roleManager;
		}

		public async Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs) {
			var items = await roleManager.Roles
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
