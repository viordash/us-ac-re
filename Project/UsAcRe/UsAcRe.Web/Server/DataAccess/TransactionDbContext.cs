using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsAcRe.Web.Shared.Utils;

namespace UsAcRe.Web.Server.Data {
	public class TransactionDbContext : ApplicationIdentityDbContext, IDisposable {
		protected bool supportTransactions = true;

		public TransactionDbContext(DbContextOptions options) : base(options) {
			SavingChanges += TransactionDbContext_SavingChanges;
		}

		void TransactionDbContext_SavingChanges(object sender, SavingChangesEventArgs e) {
			if(Database.CurrentTransaction == null) {
				BeginTransaction();
			}
		}

		public override void Dispose() {
			SavingChanges -= TransactionDbContext_SavingChanges;
			base.Dispose();
		}

		public override async ValueTask DisposeAsync() {
			SavingChanges -= TransactionDbContext_SavingChanges;
			await base.DisposeAsync();
		}

		public void CommitChanges() {
			Database.CurrentTransaction?.Commit();
		}

		protected IDisposable BeginTransaction() {
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
