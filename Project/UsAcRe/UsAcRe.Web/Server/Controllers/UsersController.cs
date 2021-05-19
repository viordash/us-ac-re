using System.Collections.Generic;
using System.Threading.Tasks;
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
		public async Task<IActionResult> List(LoadDataArgs loadDataArgs) {
			var users = await userManagementService.List(loadDataArgs);
			return new ObjectResult(users);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id) {
			var user = await userManagementService.Get(id);
			return new ObjectResult(user);
		}
	}
}
