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
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IRolesManagementService {
		Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs);
		Task<RoleModel> Get(string id);
		Task Edit(RoleModel user);
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

		public async Task<RoleModel> Get(string id) {
			var role = await roleManager.FindByIdAsync(id);
			if(role == null) {
				throw new ObjectNotFoundException();
			}
			return MapRole(role);
		}

		public async Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs) {
			var items = await roleManager.Roles
				.AsQueryable()
				.PerformLoadPagedData(loadDataArgs, nameof(RoleModel.Name));
			return items
				.Select(MapRole);
		}

		public Task Edit(RoleModel user) {
			throw new ObjectNotFoundException();
		}

		RoleModel MapRole(ApplicationIdentityRole role) {
			return new RoleModel() {
				Id = role.Id,
				Name = role.Name
			};
		}
	}
}
