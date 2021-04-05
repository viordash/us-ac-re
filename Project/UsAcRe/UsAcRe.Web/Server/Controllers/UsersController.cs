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
	[Route("[controller]")]
	public class UsersController : ControllerBase {
		readonly ILogger<UsersController> logger;
		readonly IUserAccountManagementService userAccountManagementService;

		public UsersController(
			ILogger<UsersController> logger,
			IUserAccountManagementService userAccountManagementService
			) {
			Guard.NotNull(logger, nameof(logger));
			Guard.NotNull(userAccountManagementService, nameof(userAccountManagementService));
			this.logger = logger;
			this.userAccountManagementService = userAccountManagementService;
		}

		[HttpPost]
		public IEnumerable<UserAccountModel> Users(LoadDataArgs loadDataArgs) {
			return userAccountManagementService.List(loadDataArgs);
		}
	}
}
