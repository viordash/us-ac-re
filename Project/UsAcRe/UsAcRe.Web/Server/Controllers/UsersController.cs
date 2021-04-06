using System;
using System.Collections.Generic;
using System.Linq;
using GuardNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Radzen;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Controllers {
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class UsersController : ControllerBase {
		readonly IUserAccountManagementService userAccountManagementService;

		public UsersController(
			IUserAccountManagementService userAccountManagementService
			) {
			Guard.NotNull(userAccountManagementService, nameof(userAccountManagementService));
			this.userAccountManagementService = userAccountManagementService;
		}

		[HttpPost]
		public IEnumerable<UserAccountModel> List(LoadDataArgs loadDataArgs) {
			return userAccountManagementService.UsersList(loadDataArgs);
		}
	}
}
