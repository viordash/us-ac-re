using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationUserStore : UserStore<ApplicationIdentityUser, ApplicationIdentityRole, ApplicationDbContext, System.Guid,
		ApplicationIdentityUserClaim, ApplicationIdentityUserRole, IdentityUserLogin<System.Guid>, IdentityUserToken<System.Guid>, ApplicationIdentityRoleClaim> {
		readonly ApplicationRoleStore roleStore;

		public ApplicationUserStore(ApplicationDbContext context, ApplicationRoleStore roleStore, IdentityErrorDescriber describer = null)
			: base(context, describer) {
			Guard.NotNull(roleStore, nameof(roleStore));
			this.roleStore = roleStore;
		}

		protected override Task<ApplicationIdentityRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) {
			return Task.FromResult(roleStore.Roles.SingleOrDefault(r => r.NormalizedName == normalizedRoleName));
		}

		public override Task<IList<string>> GetRolesAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default(CancellationToken)) {
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if(user == null) {
				throw new ArgumentNullException(nameof(user));
			}
			var userRoles = Context.UserRoles
				.Where(x => x.UserId == user.Id);

			var roles = roleStore.Roles
				.Where(x => userRoles.Any(ur => ur.RoleId == x.Id))
				.Select(x => x.Name)
				.ToList();
			return Task.FromResult(roles as IList<string>);
		}
	}
}
