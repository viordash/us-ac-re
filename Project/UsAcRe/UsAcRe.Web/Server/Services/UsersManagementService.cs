using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using GuardNet;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Models;
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IUsersManagementService {
		IEnumerable<UserModel> List(LoadDataArgs loadDataArgs);
		UserModel Get(string id);
	}

	public class UsersManagementService : IUsersManagementService {
		readonly ApplicationDbContext dbContext;

		public UsersManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public UserModel Get(string id) {
			var user = dbContext.Users.Find(id);
			if(user == null) {
				throw new ObjectNotFoundException();
			}
			return MapUser(user);
		}

		public IEnumerable<UserModel> List(LoadDataArgs loadDataArgs) {
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

			return query
				.OrderBy(orderField)
				.Skip(loadDataArgs.Skip.Value)
				.Take(loadDataArgs.Top.Value)
				.Select(MapUser);
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
