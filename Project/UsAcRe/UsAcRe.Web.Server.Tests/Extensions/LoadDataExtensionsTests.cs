using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class LoadDataExtensionsTests : DbContextFixture {

		public override void SetUp() {
			base.SetUp();
			for(int i = 0; i < guids.Length; i++) {
				var id = guids[i];
				DbContext.Users.Add(new ApplicationIdentityUser() {
					Id = id,
					UserName = $"test{i}",
					Email = $"email{i}",
				});
			}
			DbContext.SaveChanges();
		}

		[Test]
		public async ValueTask PerformLoadPagedData_Filter_Test() {
			var dataPaging = new DataPaging() {
				Filter = $"Id=\"{guids[2]}\"",
				Top = 10,
				Skip = 0
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo(guids[2]));
			Assert.That(users[0].UserName, Is.EqualTo($"test2"));

			string ignoreCaseFilter = $"Id=\"{guids[4]}\"";
			dataPaging.Filter = ignoreCaseFilter;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo(guids[4]));
			Assert.That(users[0].UserName, Is.EqualTo($"test4"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_Offset_Test() {
			var dataPaging = new DataPaging() {
				Top = 4,
				Skip = 3
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(4));
			Assert.That(users[0].Id, Is.EqualTo(guids[3]));
			Assert.That(users[0].UserName, Is.EqualTo("test3"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_OrderBy_Test() {
			var dataPaging = new DataPaging() {
				Top = 10,
				Skip = 0,
				OrderBy = "email desc"
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[9]));
			Assert.That(users[0].Email, Is.EqualTo("email9"));
			Assert.That(users[9].Id, Is.EqualTo(guids[0]));
			Assert.That(users[9].Email, Is.EqualTo("email0"));

			const string ignoreCaseOrderBy = "USERName asc";
			dataPaging.OrderBy = ignoreCaseOrderBy;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}


		[Test]
		public async ValueTask PerformLoadPagedData_For_Default_LoadDataArgs_Test() {
			var dataPaging = new DataPaging();

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging, "UserName");
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}
	}
}
