using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationRoleStore : RoleStore<ApplicationIdentityRole, ApplicationDbContext, System.Guid, ApplicationIdentityUserRole,
			ApplicationIdentityRoleClaim>, IQueryableRoleStore<ApplicationIdentityRole>, IRoleStore<ApplicationIdentityRole>,
		IDisposable, IRoleClaimStore<ApplicationIdentityRole> {
		public ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber describer = null) : base(context, describer) {
		}
	}
}
