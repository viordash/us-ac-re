using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
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

		public UsersManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public async Task<UserModel> Get(System.Guid id) {
			var user = await dbContext.Users.FindAsync(id);
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return MapUser(user);
		}

		public async Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs) {
			var items = await dbContext.Users
				.AsQueryable()
				.PerformLoadPagedData(loadDataArgs, nameof(UserModel.Email));
			return items
				.Select(MapUser);
		}

		public Task Edit(UserModel user) {
			throw new ObjectNotFoundException();
		}

		UserModel MapUser(ApplicationUser user) {
			return new UserModel() {
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				RoleNames = user.Id.ToString()
			};
		}

	}
}
