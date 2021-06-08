using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationRoleStore : RoleStore<ApplicationIdentityRole, ApplicationDbContext, System.Guid, ApplicationIdentityUserRole,
			ApplicationIdentityRoleClaim>, IQueryableRoleStore<ApplicationIdentityRole>, IRoleStore<ApplicationIdentityRole>,
		IDisposable, IRoleClaimStore<ApplicationIdentityRole> {

		readonly IQueryable<ApplicationIdentityRole> predefinedRoles;

		public override IQueryable<ApplicationIdentityRole> Roles => predefinedRoles;


		public ApplicationRoleStore(ApplicationDbContext context, ILookupNormalizer keyNormalizer, IdentityErrorDescriber describer = null)
			: base(context, describer) {
			AutoSaveChanges = false;
			predefinedRoles = ApplicationRoleTypeSpecifics.Names.Select(x =>
						new ApplicationIdentityRole(x.Key, x.Value, (keyNormalizer == null)
							? x.Value
							: keyNormalizer.NormalizeName(x.Value)))
				.AsQueryable();
		}

		public override Task<IdentityResult> CreateAsync(ApplicationIdentityRole role, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}

		public override Task<IdentityResult> UpdateAsync(ApplicationIdentityRole role, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}

		public override Task<IdentityResult> DeleteAsync(ApplicationIdentityRole role, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}


		public override Task SetRoleNameAsync(ApplicationIdentityRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}


		public override Task<IList<Claim>> GetClaimsAsync(ApplicationIdentityRole role, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}

		public override Task AddClaimAsync(ApplicationIdentityRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}

		public override Task RemoveClaimAsync(ApplicationIdentityRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken)) {
			throw new NotSupportedException();
		}

		protected override ApplicationIdentityRoleClaim CreateRoleClaim(ApplicationIdentityRole role, Claim claim) {
			throw new NotSupportedException();
		}
	}
}
