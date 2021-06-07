using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Tests.Common;
using UsAcRe.Web.Server.Data;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class ApplicationDbContextTests : ServiceProviderFixture {
		DbContextOptions dbContextOptions;

		public override void SetUp() {
			base.SetUp();
			dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "Application Test")
				.Options;

		}

		public override void TearDown() {
			base.TearDown();
		}

		#region classes
		class TestableDatabaseFacade : DatabaseFacade {
			Mock<IDbContextTransaction> dbContextTransactionMock;
			public TestableDatabaseFacade(DbContext context) : base(context) {
			}

			public override IDbContextTransaction BeginTransaction() {
				dbContextTransactionMock = new Mock<IDbContextTransaction>();
				return dbContextTransactionMock.Object;
			}

			public override IDbContextTransaction CurrentTransaction {
				get {
					return dbContextTransactionMock?.Object;
				}
			}
		}

		class TestableApplicationDbContext : ApplicationDbContext {
			TestableDatabaseFacade database;

			public TestableApplicationDbContext(
					DbContextOptions options)
					: base(options, Options.Create(new OperationalStoreOptions())) {
			}

			public override DatabaseFacade Database { get { return database ??= new TestableDatabaseFacade(this); } }
		}
		#endregion

		[Test]
		public void SaveChanges_Begin_Scoped_Transaction_Test() {
			var dbContext = new TestableApplicationDbContext(dbContextOptions);
			Assert.That(dbContext.Database.CurrentTransaction, Is.Null);

			dbContext.SaveChanges();
			var transaction = dbContext.Database.CurrentTransaction;
			Assert.That(transaction, Is.Not.Null);

			dbContext.SaveChanges();
			dbContext.SaveChanges();

			Assert.AreSame(transaction, dbContext.Database.CurrentTransaction);
		}
	}
}
