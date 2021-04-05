using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using GuardNet;
using Microsoft.EntityFrameworkCore;
using Radzen;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Services {
	public interface IUserAccountManagementService {
		IEnumerable<UserAccountModel> List(LoadDataArgs loadDataArgs);
	}

	public class UserAccountManagementService : IUserAccountManagementService {
		readonly ApplicationDbContext dbContext;

		public UserAccountManagementService(ApplicationDbContext dbContext) {
			Guard.NotNull(dbContext, nameof(dbContext));
			this.dbContext = dbContext;
		}

		public IEnumerable<UserAccountModel> List(LoadDataArgs loadDataArgs) {
			var query = dbContext.Users.AsQueryable();

			if(!string.IsNullOrEmpty(loadDataArgs.Filter)) {
				query = query.Where(loadDataArgs.Filter);
			}

			if(!string.IsNullOrEmpty(loadDataArgs.OrderBy)) {
				query = query.OrderBy(loadDataArgs.OrderBy);
			}

			return query
				.Skip(loadDataArgs.Skip.Value)
				.Take(loadDataArgs.Top.Value)
				.Select(x => new UserAccountModel() {
					UserName = x.UserName,
					Email = x.Email,
					AccessFailedCount = x.AccessFailedCount,
					EmailConfirmed = x.EmailConfirmed,
					Role = x.Id
				});
		}
	}
}
