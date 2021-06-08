﻿using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationAuthorizationDbContext : TransactionDbContext, IPersistedGrantDbContext {
		readonly IOptions<OperationalStoreOptions> operationalStoreOptions;

		public ApplicationAuthorizationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options) {
			this.operationalStoreOptions = operationalStoreOptions;
		}

		public DbSet<PersistedGrant> PersistedGrants { get; set; }
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() {
			using(BeginTransaction()) {
				return base.SaveChangesAsync();
			}
		}

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(operationalStoreOptions.Value);
		}
	}
}