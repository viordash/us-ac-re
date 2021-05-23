using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Models;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Exceptions;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class UsersManagementServiceTests : DbContextFixture {
		UsersManagementService testable;

		public override void SetUp() {
			base.SetUp();

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
			DbContext.SaveChanges();

			testable = new UsersManagementService(DbContext);
		}

		[Test]
		public async ValueTask Get_User_Test() {
			var user = await testable.Get("2");
			Assert.IsNotNull(user);
			Assert.That(user.UserName, Is.EqualTo("test2"));
		}

		[Test]
		public void Get_For_Not_Exist_Key_Throws_ObjectNotFoundException() {
			Assert.ThrowsAsync<ObjectNotFoundException>(async () => await testable.Get("not_exists_user"));
		}

		[Test]
		public async ValueTask List_Test() {
			var users = await testable.List(new LoadDataArgs());
			Assert.IsNotNull(users);
			Assert.That(users.Count(), Is.EqualTo(3));
			Assert.That(users.ElementAt(0).Id, Is.EqualTo("1"));
			Assert.That(users.ElementAt(0).UserName, Is.EqualTo("test1"));
		}
	}
}
