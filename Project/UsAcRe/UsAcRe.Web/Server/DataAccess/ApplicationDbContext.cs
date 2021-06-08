using System;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsAcRe.Web.Shared.Utils;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationDbContext : ApplicationIdentityDbContext, IPersistedGrantDbContext, IDisposable {
		readonly IOptions<OperationalStoreOptions> operationalStoreOptions;
		protected bool supportTransactions = true;

		public ApplicationDbContext(
			DbContextOptions options,
			IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options) {
			this.operationalStoreOptions = operationalStoreOptions;
			SavingChanges += ApplicationDbContext_SavingChanges;
		}

		void ApplicationDbContext_SavingChanges(object sender, SavingChangesEventArgs e) {
			if(Database.CurrentTransaction == null) {
				BeginTransaction();
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
			using(BeginTransaction()) {
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

		IDisposable BeginTransaction() {
			if(supportTransactions) {
				try {
					return Database.BeginTransaction();
				} catch(InvalidOperationException) {
					supportTransactions = false;
				}
			}
			return new EmptyDisposable();
		}
	}
}
