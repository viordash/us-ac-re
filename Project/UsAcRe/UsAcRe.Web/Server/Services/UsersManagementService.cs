using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
			var users = await ListInternal(q => q
					.Where(x => x.Id == id)
					.ToListAsync()
					);
			var user = users.FirstOrDefault();
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return user;
		}

		public async Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs) {
			var users = await ListInternal((q) => q.PerformLoadPagedData(loadDataArgs, nameof(ApplicationUser.Email)));
			return users;
		}

		public Task Edit(UserModel user) {
			throw new ObjectNotFoundException();
		}


		class UserRolesInternal {
			public Guid Id { get; set; }
			public string UserName { get; set; }
			public string Email { get; set; }
			public string Role { get; set; }
		}

		async Task<IEnumerable<UserModel>> ListInternal(Func<IQueryable<UserRolesInternal>, Task<List<UserRolesInternal>>> funcRetrieveData) {
			var query = from u in dbContext.Users
						join ur in dbContext.UserRoles on u.Id equals ur.UserId into gj
						from x in gj.DefaultIfEmpty()
						join r in dbContext.Roles on x.RoleId equals r.Id into gj1
						from x1 in gj1.DefaultIfEmpty()
						select new UserRolesInternal {
							Id = u.Id,
							UserName = u.UserName,
							Email = u.Email,
							Role = x1.Name,
						};

			var users = await funcRetrieveData(query);
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
	}
}
