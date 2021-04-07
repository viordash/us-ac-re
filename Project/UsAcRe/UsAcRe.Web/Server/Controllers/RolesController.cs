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
	public class RolesController : ControllerBase {
		readonly IRolesManagementService rolesManagementService;

		public RolesController(
			IRolesManagementService rolesManagementService
			) {
			Guard.NotNull(rolesManagementService, nameof(rolesManagementService));
			this.rolesManagementService = rolesManagementService;
		}

		[HttpPost]
		public IEnumerable<RoleModel> List(LoadDataArgs loadDataArgs) {
			return rolesManagementService.List(loadDataArgs);
		}
	}
}
