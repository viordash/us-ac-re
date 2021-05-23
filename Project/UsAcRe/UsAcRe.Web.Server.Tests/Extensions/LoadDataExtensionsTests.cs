using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class LoadDataExtensionsTests : DbContextFixture {

		public override void SetUp() {
			base.SetUp();
			for(int i = 0; i < 10; i++) {
				DbContext.Users.Add(new ApplicationUser() {
					Id = $"{i}",
					UserName = $"test{i}",
					Email = $"email{i}",
				});
			}
			DbContext.SaveChanges();
		}

		[Test]
		public async ValueTask PerformLoadPagedData_Filter_Test() {
			var loadDataArgs = new LoadDataArgs() {
				Filter = "Id=\"2\"",
				Top = 10,
				Skip = 0
			};

			var users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo("2"));
			Assert.That(users[0].UserName, Is.EqualTo("test2"));

			const string ignoreCaseFilter = "iD=\"4\"";
			loadDataArgs.Filter = ignoreCaseFilter;
			users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo("4"));
			Assert.That(users[0].UserName, Is.EqualTo("test4"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_Offset_Test() {
			var loadDataArgs = new LoadDataArgs() {
				Top = 4,
				Skip = 3
			};

			var users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(4));
			Assert.That(users[0].Id, Is.EqualTo("3"));
			Assert.That(users[0].UserName, Is.EqualTo("test3"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_OrderBy_Test() {
			var loadDataArgs = new LoadDataArgs() {
				Top = 10,
				Skip = 0,
				OrderBy = "email desc"
			};

			var users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo("9"));
			Assert.That(users[0].Email, Is.EqualTo("email9"));
			Assert.That(users[9].Id, Is.EqualTo("0"));
			Assert.That(users[9].Email, Is.EqualTo("email0"));

			const string ignoreCaseOrderBy = "USERName asc";
			loadDataArgs.OrderBy = ignoreCaseOrderBy;
			users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo("0"));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo("9"));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}


		[Test]
		public async ValueTask PerformLoadPagedData_For_Default_LoadDataArgs_Test() {
			var loadDataArgs = new LoadDataArgs();

			var users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo("0"));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo("9"));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}
	}
}
