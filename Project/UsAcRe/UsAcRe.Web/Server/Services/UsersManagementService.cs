using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Exceptions;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IUsersManagementService {
		Task<IList<UserModel>> List(LoadDataArgs loadDataArgs);
		Task<UserModel> Get(System.Guid id);
		Task Edit(UserModel user);
		Task Create(UserModel user);
	}

	public class UsersManagementService : IUsersManagementService {
		#region inner classes
		class UserRolesInternal : ConcurrencyModel {
			public Guid Id { get; set; }
			public string UserName { get; set; }
			public string Email { get; set; }
			public Guid? RoleId { get; set; }
		}
		#endregion

		readonly ApplicationDbContext dbContext;
		readonly UserManager<ApplicationIdentityUser> userManager;
		readonly RoleManager<ApplicationIdentityRole> roleManager;

		public UsersManagementService(ApplicationDbContext dbContext, UserManager<ApplicationIdentityUser> userManager,
			RoleManager<ApplicationIdentityRole> roleManager) {
			Guard.NotNull(dbContext, nameof(dbContext));
			Guard.NotNull(userManager, nameof(userManager));
			Guard.NotNull(roleManager, nameof(roleManager));
			this.dbContext = dbContext;
			this.userManager = userManager;
			this.roleManager = roleManager;
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

		public async Task<IList<UserModel>> List(LoadDataArgs loadDataArgs) {
			var users = await ListInternal((q) => q.PerformLoadPagedData(loadDataArgs, nameof(ApplicationIdentityUser.Email)));
			return users.ToList();
		}

		public async Task Edit(UserModel user) {
			var appUser = await userManager.FindByIdAsync(user.Id.ToString());
			if(appUser == null) {
				throw new ObjectNotFoundException();
			}

			appUser.UserName = user.UserName;
			appUser.Email = user.Email;
			appUser.SetConcurrencyStamp(dbContext, user.ConcurrencyStamp);

			var updateResult = await userManager.UpdateAsync(appUser);
			if(!updateResult.Succeeded) {
				throw new IdentityErrorException(updateResult);
			}

			var userRoles = await userManager.GetRolesAsync(appUser);
			if(userRoles != null) {
				var editRolesResult = await userManager.RemoveFromRolesAsync(appUser, userRoles);
				if(!editRolesResult.Succeeded) {
					throw new IdentityErrorException(editRolesResult);
				}

				if(user.Roles != null && user.Roles.Any()) {
					editRolesResult = await userManager.AddToRolesAsync(appUser, user.Roles);
					if(!editRolesResult.Succeeded) {
						throw new IdentityErrorException(editRolesResult);
					}
				}
			}
			dbContext.CommitChanges();
		}

		public async Task Create(UserModel user) {
			var appUser = new ApplicationIdentityUser {
				UserName = user.UserName,
				Email = user.Email,
				ConcurrencyStamp = user.ConcurrencyStamp
			};

			var createResult = await userManager.CreateAsync(appUser);
			if(!createResult.Succeeded) {
				throw new IdentityErrorException(createResult);
			}

			if(user.Roles != null && user.Roles.Any()) {
				var editRolesResult = await userManager.AddToRolesAsync(appUser, user.Roles);
				if(!editRolesResult.Succeeded) {
					throw new IdentityErrorException(editRolesResult);
				}
			}
		}

		async Task<IEnumerable<UserModel>> ListInternal(Func<IQueryable<UserRolesInternal>, Task<List<UserRolesInternal>>> funcRetrieveData) {
			var query = from u in dbContext.Users
						join ur in dbContext.UserRoles on u.Id equals ur.UserId into gj
						from x in gj.DefaultIfEmpty()
						select new UserRolesInternal {
							Id = u.Id,
							UserName = u.UserName,
							Email = u.Email,
							ConcurrencyStamp = u.ConcurrencyStamp,
							RoleId = x.RoleId,
						};

			var users = await funcRetrieveData(query);
			var filteredRoles = roleManager.Roles
				.Where(r => users.Any(u => u.RoleId == r.Id))
				.ToList();

			var groupedUsers = users
				.GroupBy(
				u => u.Id,
				u => u.RoleId,
				(k, roles) => {
					var user = users.First(x => x.Id == k);
					return new UserModel() {
						Id = user.Id,
						UserName = user.UserName,
						Email = user.Email,
						ConcurrencyStamp = user.ConcurrencyStamp,
						Roles = filteredRoles.Where(x => roles.Any(r => r == x.Id)).Select(x => x.Name)
					};
				});
			return groupedUsers;
		}
	}
}
