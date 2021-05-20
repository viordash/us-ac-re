using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using UsAcRe.Web.Server.Services;

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
	}
}
