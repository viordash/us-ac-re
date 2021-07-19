using System.Collections.Generic;
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
					AccessFailedCount = i
				});
			}
			DbContext.SaveChanges();
		}

		[Test]
		public async ValueTask PerformLoadPagedData_Offset_Test() {
			var dataPaging = new DataPaging() {
				Take = 4,
				Skip = 3
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(4));
			Assert.That(users[0].Id, Is.EqualTo(guids[3]));
			Assert.That(users[0].UserName, Is.EqualTo("test3"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_OrderBy_Test() {
			var dataPaging = new DataPaging() {
				Take = 10,
				Skip = 0,
				Sorts = new List<Shared.Models.SortDescriptor>() { new Shared.Models.SortDescriptor() { Field = "email", SortOrder = Shared.Models.SortOrder.Descending } }
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[9]));
			Assert.That(users[0].Email, Is.EqualTo("email9"));
			Assert.That(users[9].Id, Is.EqualTo(guids[0]));
			Assert.That(users[9].Email, Is.EqualTo("email0"));

			const string ignoreCaseOrderBy = "USERName";
			dataPaging.Sorts = new List<Shared.Models.SortDescriptor>() { new Shared.Models.SortDescriptor() { Field = ignoreCaseOrderBy, SortOrder = Shared.Models.SortOrder.Ascending } };

			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}


		[Test]
		public async ValueTask PerformLoadPagedData_For_Default_LoadDataArgs_Test() {
			var dataPaging = new DataPaging();

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}
	}
}
