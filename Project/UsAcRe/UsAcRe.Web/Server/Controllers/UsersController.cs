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
		public async Task<IActionResult> Paged(LoadDataArgs loadDataArgs) {
			var users = await userManagementService.List(loadDataArgs);
			return new ObjectResult(users);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(System.Guid id) {
			var user = await userManagementService.Get(id);
			return new ObjectResult(user);
		}

		[HttpPut("{Id}")]
		public async Task<IActionResult> Put(UserModel user) {
			await userManagementService.Edit(user);
			return new NoContentResult();
		}

		[HttpPost]
		public async Task<IActionResult> Post(UserModel user) {
			await userManagementService.Create(user);
			return new NoContentResult();
		}
	}
}
