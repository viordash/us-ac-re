using System.Collections.Generic;
using GuardNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Controllers {
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase {
		readonly IUsersManagementService userManagementService;

		public UsersController(
			IUsersManagementService userManagementService
			) {
			Guard.NotNull(userManagementService, nameof(userManagementService));
			this.userManagementService = userManagementService;
		}

		[HttpPost("[action]")]
		public IEnumerable<UserModel> List(LoadDataArgs loadDataArgs) {
			return userManagementService.List(loadDataArgs);
		}

		[HttpGet("{id}")]
		public UserModel Get(string id) {
			return userManagementService.Get(id);
		}

	}
}
