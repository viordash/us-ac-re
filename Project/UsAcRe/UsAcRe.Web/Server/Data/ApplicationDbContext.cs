using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationDbContext : ApplicationIdentityDbContext<ApplicationUser, ApplicationIdentityRole>, IPersistedGrantDbContext, IDisposable {
		private readonly IOptions<OperationalStoreOptions> operationalStoreOptions;

		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options) {
			this.operationalStoreOptions = operationalStoreOptions;
			SavingChanges += ApplicationDbContext_SavingChanges;
		}

		private void ApplicationDbContext_SavingChanges(object sender, SavingChangesEventArgs e) {
			if(Database.CurrentTransaction == null) {
				Database.BeginTransaction();
			}
		}

		public override void Dispose() {
			SavingChanges -= ApplicationDbContext_SavingChanges;
			base.Dispose();
		}

		public override ValueTask DisposeAsync() {
			SavingChanges -= ApplicationDbContext_SavingChanges;
			return base.DisposeAsync();
		}

		public DbSet<PersistedGrant> PersistedGrants { get; set; }
		public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

		Task<int> IPersistedGrantDbContext.SaveChangesAsync() {
			using(var transaction = Database.BeginTransaction()) {
				return base.SaveChangesAsync();
			}
		}

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.ConfigurePersistedGrantContext(operationalStoreOptions.Value);
		}

		public void CommitChanges() {
			Database.CurrentTransaction?.Commit();
		}
	}
}
