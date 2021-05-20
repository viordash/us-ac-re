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
		}
	}
}
