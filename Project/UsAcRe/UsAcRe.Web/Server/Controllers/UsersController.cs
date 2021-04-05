using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Radzen;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Controllers {
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase {
		readonly ILogger<UsersController> logger;

		public UsersController(ILogger<UsersController> logger) {
			this.logger = logger;
		}

		[HttpPost]
		public IEnumerable<UserAccountModel> Users(LoadDataArgs args) {
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new UserAccountModel {
				UserName = DateTime.Now.AddDays(index).ToShortDateString(),
				Email = rng.Next(-20, 55).ToString()
			})
			.ToArray();
		}
	}
}
