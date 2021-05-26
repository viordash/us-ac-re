using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationUser : IdentityUser<System.Guid> {
		[NotMapped]
		public IEnumerable<string> RoleNames { get; set; }
	}
}
