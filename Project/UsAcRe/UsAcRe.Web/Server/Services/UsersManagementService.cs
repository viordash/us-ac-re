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
using UsAcRe.Web.Server.Exceptions;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IUsersManagementService {
		Task<PagedDataResult<UserModel>> List(DataPaging dataPaging);
		Task<UserModel> Get(System.Guid id);
		Task Edit(UserModel user);
		Task Create(UserModel user);
	}

	public class UsersManagementService : IUsersManagementService {
		#region inner classes
		class DataResult<TSource> {
			public IEnumerable<TSource> Data { get; set; }
			public int Total { get; set; }
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
			var usersResult = await ListInternal(q => q
					.Where(x => x.Id == id)
					.ToListAsync()
					);
			var user = usersResult.Data.FirstOrDefault();
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return user;
		}



		public async Task<PagedDataResult<UserModel>> List(DataPaging dataPaging) {
			bool rolesSortAsc = false;
			bool rolesSortDesc = false;
			if(dataPaging.Sorts != null) {
				var rolesSort = dataPaging.Sorts.Where(x => x.Field == nameof(UserModel.Roles));
				rolesSortAsc = rolesSort.Any(x => x.SortOrder == Shared.Models.SortOrder.Ascending);
				rolesSortDesc = rolesSort.Any(x => x.SortOrder == Shared.Models.SortOrder.Descending);
				if(rolesSortAsc || rolesSortDesc) {
					dataPaging.Sorts = dataPaging.Sorts.Where(x => x.Field != nameof(UserModel.Roles));
				}
			}
			var rolesFiltering = dataPaging.Filters?.Where(x => x.Field == nameof(UserModel.Roles)).ToList();
			if(rolesFiltering != null && rolesFiltering.Any()) {
				dataPaging.Filters = dataPaging.Filters.Where(x => x.Field != nameof(UserModel.Roles));
			}
			var usersResult = await ListInternal((q) => {
				var pagedQuery = q.PerformLoadPagedData(dataPaging);
				return pagedQuery;
			});

			usersResult.Data = usersResult.Data.ApplyFilter(rolesFiltering);

			if(rolesSortAsc) {
				usersResult.Data = usersResult.Data.OrderBy(x => UserRolesView.Concat(x));
			} else if(rolesSortDesc) {
				usersResult.Data = usersResult.Data.OrderByDescending(x => UserRolesView.Concat(x));
			}
			return new PagedDataResult<UserModel>() {
				Data = usersResult.Data.ToList(),
				Total = usersResult.Total,
				Skip = dataPaging.Skip,
				Take = dataPaging.Take
			};
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
					editRolesResult = await userManager.AddToRolesAsync(appUser, user.Roles.Select(x => x.Name));
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
				var editRolesResult = await userManager.AddToRolesAsync(appUser, user.Roles.Select(x => x.Name));
				if(!editRolesResult.Succeeded) {
					throw new IdentityErrorException(editRolesResult);
				}
			}
			dbContext.CommitChanges();
		}

		async Task<DataResult<UserModel>> ListInternal(Func<IQueryable<ApplicationIdentityUser>, Task<List<ApplicationIdentityUser>>> funcRetrieveData) {
			var total = await dbContext.Users.CountAsync();
			var users = await funcRetrieveData(dbContext.Users);

			var userRoles = await dbContext.UserRoles
				.Where(x => users.Select(x => x.Id).Contains(x.UserId))
				.ToListAsync();

			var roles = roleManager.Roles
				.Where(r => userRoles.Any(u => u.RoleId == r.Id))
				.ToList();

			var groupedUsers = users
				.Select(u => {
					var ur = userRoles.Where(x => x.UserId == u.Id);
					return new UserModel() {
						Id = u.Id,
						UserName = u.UserName,
						Email = u.Email,
						ConcurrencyStamp = u.ConcurrencyStamp,
						Roles = roles.Where(x => ur.Any(r => r.RoleId == x.Id)).Select(x => new RoleModel() {
							Id = x.Id,
							Name = x.Name
						})
					};

				});

			return new DataResult<UserModel>() {
				Data = groupedUsers,
				Total = total
			};
		}
	}
}
