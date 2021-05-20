using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsAcRe.Web.Server.Models;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationAuthorizationDbContext : ApplicationIdentityDbContext<ApplicationUser, ApplicationIdentityRole>, IPersistedGrantDbContext, IDisposable {

		private readonly IOptions<OperationalStoreOptions> operationalStoreOptions;

		public ApplicationAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options) {
			this.operationalStoreOptions = operationalStoreOptions;
		}

		public DbSet<PersistedGrant> PersistedGrants { get; set; }
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(operationalStoreOptions.Value);
		}
	}
}
