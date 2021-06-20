using Microsoft.AspNetCore.Identity;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationIdentityUser : IdentityUser<System.Guid>, IConcurrencyModel {
	}
}
