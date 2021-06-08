using System.Linq;
using GuardNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Controllers {
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class RolesController : ControllerBase {
		readonly RoleManager<ApplicationIdentityRole> roleManager;

		public RolesController(
			RoleManager<ApplicationIdentityRole> roleManager
			) {
			Guard.NotNull(roleManager, nameof(roleManager));
			this.roleManager = roleManager;
		}

		[HttpGet]
		public IActionResult Get() {
			var roles = roleManager.Roles;
			return new ObjectResult(roles);
		}

		[HttpGet("{id}")]
		public IActionResult Get(System.Guid id) {
			var user = roleManager.Roles.FirstOrDefault(x => x.Id == id);
			return new ObjectResult(user);
		}
	}
}
