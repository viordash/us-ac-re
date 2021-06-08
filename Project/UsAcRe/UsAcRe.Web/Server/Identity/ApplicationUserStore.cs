using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationUserStore : UserStore<ApplicationIdentityUser, ApplicationIdentityRole, ApplicationDbContext, System.Guid,
		ApplicationIdentityUserClaim, ApplicationIdentityUserRole, IdentityUserLogin<System.Guid>, IdentityUserToken<System.Guid>, ApplicationIdentityRoleClaim> {
		public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer) {
		}
	}
}
