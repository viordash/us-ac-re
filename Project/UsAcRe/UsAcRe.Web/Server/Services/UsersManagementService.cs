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
	public interface IUsersManagementService {
		Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs);
		Task<UserModel> Get(System.Guid id);
		Task Edit(UserModel user);
	}

	public class UsersManagementService : IUsersManagementService {
		readonly ApplicationDbContext dbContext;
		readonly UserManager<ApplicationUser> userManager;

		public UsersManagementService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) {
			Guard.NotNull(dbContext, nameof(dbContext));
			Guard.NotNull(userManager, nameof(userManager));
			this.dbContext = dbContext;
			this.userManager = userManager;
		}

		public async Task<UserModel> Get(System.Guid id) {
			var loadDataArgs = new LoadDataArgs() {
				Filter = $"{nameof(ApplicationUser.Id)}=\"{id}\""
			};
			var user = (await List(loadDataArgs)).FirstOrDefault();
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return user;
		}

		public async Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs) {
			var query = from u in dbContext.Users
						join ur in dbContext.UserRoles on u.Id equals ur.UserId into gj
						from x in gj.DefaultIfEmpty()
						join r in dbContext.Roles on x.RoleId equals r.Id into gj1
						from x1 in gj1.DefaultIfEmpty()
						select new {
							u.Id,
							u.UserName,
							u.Email,
							Role = x1.Name,
						};

			var users = await query.PerformLoadPagedData(loadDataArgs, nameof(ApplicationUser.Email));
			var groupedUsers = users.GroupBy(
				u => u.Id,
				u => u.Role,
				(k, roles) => {
					var user = users.FirstOrDefault(x => x.Id == k);
					return new UserModel() {
						Id = user.Id,
						UserName = user.UserName,
						Email = user.Email,
						Roles = roles
					};
				});
			return groupedUsers;
		}

		public Task Edit(UserModel user) {
			throw new ObjectNotFoundException();
		}
	}
}
