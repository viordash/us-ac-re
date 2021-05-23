using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationIdentityDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, string, IdentityUserClaim<string>, ApplicationIdentityUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
	where TUser : ApplicationUser
	where TRole : ApplicationIdentityRole {

		public ApplicationIdentityDbContext(DbContextOptions options) : base(options) {
		}
	}

}
