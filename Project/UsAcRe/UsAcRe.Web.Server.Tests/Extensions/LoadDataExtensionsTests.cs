using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Models;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class LoadDataExtensionsTests : DbContextFixture {

		[Test]
		public async ValueTask PerformLoadPagedData_Filter_Test() {
			DbContext.Users.Add(new ApplicationUser() {
				Id = "1",
				UserName = "test1"
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = "2",
				UserName = "test2"
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = "22",
				UserName = "test22"
			});

			var loadDataArgs = new LoadDataArgs() {
				Filter = "Id=\"2\"",
				Top = 10,
				Skip = 0
			};
			DbContext.SaveChanges();

			var users = await DbContext.Users.PerformLoadPagedData(loadDataArgs, "UserName");

			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo("2"));
			Assert.That(users[0].UserName, Is.EqualTo("test2"));
		}
	}
}
