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
		Task<RoleModel> Get(System.Guid id);
		Task Add(RoleModel role);
		Task Edit(System.Guid id, RoleModel role);
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

		public async Task<RoleModel> Get(System.Guid id) {
			var role = await roleManager.FindByIdAsync(id.ToString());
			if(role == null) {
				throw new ObjectNotFoundException();
			}
			return MapRole(role);
		}

		public async Task<IEnumerable<RoleModel>> List(LoadDataArgs loadDataArgs) {
			var items = await roleManager.Roles
				.PerformLoadPagedData(loadDataArgs, nameof(RoleModel.Name));
			return items
				.Select(MapRole);
		}

		public async Task Add(RoleModel role) {
			await roleManager.CreateAsync(MapRole(role));
		}

		public async Task Edit(System.Guid id, RoleModel role) {
			var existsRole = await roleManager.FindByIdAsync(id.ToString());
			if(existsRole == null) {
				throw new ObjectNotFoundException();
			}
			role.Id = id;
			await roleManager.UpdateAsync(MapRole(role));
		}

		RoleModel MapRole(ApplicationIdentityRole role) {
			return new RoleModel() {
				Id = role.Id,
				Name = role.Name
			};
		}

		ApplicationIdentityRole MapRole(RoleModel role) {
			return new ApplicationIdentityRole() {
				Id = role.Id,
				Name = role.Name
			};
		}
	}
}
