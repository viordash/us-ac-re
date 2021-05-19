using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Models;
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IUsersManagementService {
		Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs);
		Task<UserModel> Get(string id);
		Task Edit(UserModel user);
	}

	public class UsersManagementService : IUsersManagementService {
		readonly ApplicationDbContext dbContext;

		public UsersManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public async Task<UserModel> Get(string id) {
			var user = await dbContext.Users.FindAsync(id);
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return MapUser(user);
		}

		public async Task<IEnumerable<UserModel>> List(LoadDataArgs loadDataArgs) {
			var query = dbContext.Users.AsQueryable();

			if(!string.IsNullOrEmpty(loadDataArgs.Filter)) {
				query = query.Where(loadDataArgs.Filter);
			}

			string orderField;
			if(!string.IsNullOrEmpty(loadDataArgs.OrderBy)) {
				orderField = loadDataArgs.OrderBy;
			} else {
				orderField = $"{nameof(UserModel.Email)} asc";
			}

			var qUsers = await query
				.OrderBy(orderField)
				.Skip(loadDataArgs.Skip.Value)
				.Take(loadDataArgs.Top.Value)
				.ToListAsync();

			return qUsers
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
				Role = user.Id
			};
		}

	}
}
