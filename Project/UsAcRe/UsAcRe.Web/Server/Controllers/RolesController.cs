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
	public class RolesController : ControllerBase {
		readonly IRolesManagementService rolesManagementService;

		public RolesController(
			IRolesManagementService rolesManagementService
			) {
			Guard.NotNull(rolesManagementService, nameof(rolesManagementService));
			this.rolesManagementService = rolesManagementService;
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Paged(LoadDataArgs loadDataArgs) {
			var roles = await rolesManagementService.List(loadDataArgs);
			return new ObjectResult(roles);
		}

		[HttpGet]
		public async Task<IActionResult> Get() {
			var roles = await rolesManagementService.List(new LoadDataArgs() {
				Filter = null,
				OrderBy = null,
				Skip = null,
				Top = null
			});
			return new ObjectResult(roles);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id) {
			var user = await rolesManagementService.Get(id);
			return new ObjectResult(user);
		}

		[HttpPut("{Id}")]
		public async Task<IActionResult> Put(string id, RoleModel user) {
			await rolesManagementService.Edit(id, user);
			return new NoContentResult();
		}

		[HttpPost]
		public async Task<IActionResult> Post(RoleModel user) {
			await rolesManagementService.Add(user);
			return new NoContentResult();
		}
	}
}
