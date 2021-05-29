using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Exceptions;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class UsersManagementServiceTests : AuthorizationDbContextFixture {
		UsersManagementService testable;

		public override void SetUp() {
			base.SetUp();

			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[1],
				UserName = "test1"
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[2],
				UserName = "test2"
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[5],
				UserName = "test22"
			});

			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = guids[7],
				Name = "role1"
			});
			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = guids[8],
				Name = "role2"
			});

			DbContext.UserRoles.Add(new ApplicationIdentityUserRole() {
				UserId = guids[1],
				RoleId = guids[7]
			});

			DbContext.UserRoles.Add(new ApplicationIdentityUserRole() {
				UserId = guids[1],
				RoleId = guids[8]
			});

			DbContext.SaveChanges();
			testable = new UsersManagementService(DbContext, userManager);
		}

		[Test]
		public async ValueTask Get_User_Test() {
			var user = await testable.Get(guids[2]);
			Assert.IsNotNull(user);
			Assert.That(user.UserName, Is.EqualTo("test2"));
		}

		[Test]
		public void Get_For_Not_Exist_Key_Throws_ObjectNotFoundException() {
			Assert.ThrowsAsync<ObjectNotFoundException>(async () => await testable.Get(System.Guid.NewGuid()));
		}

		[Test]
		public async ValueTask List_Test() {
			var users = await testable.List(new LoadDataArgs());
			Assert.IsNotNull(users);
			Assert.That(users.Count(), Is.EqualTo(3));
			Assert.That(users.ElementAt(0).Id, Is.EqualTo(guids[1]));
			Assert.That(users.ElementAt(0).UserName, Is.EqualTo("test1"));
			Assert.That(users.ElementAt(0).Roles, Is.EquivalentTo(new[] { "role1", "role2" }));
			Assert.That(users.ElementAt(1).Roles, Is.EquivalentTo(new string[] { null }));
			Assert.That(users.ElementAt(2).Roles, Is.EquivalentTo(new string[] { null }));
		}
	}
}
