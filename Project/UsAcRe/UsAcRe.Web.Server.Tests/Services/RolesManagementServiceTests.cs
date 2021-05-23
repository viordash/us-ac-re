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
	public class RolesManagementServiceTests : AuthorizationDbContextFixture {
		RolesManagementService testable;

		public override void SetUp() {
			base.SetUp();

			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = "1",
				Name = "role1"
			});
			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = "2",
				Name = "role2"
			});
			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = "22",
				Name = "role22"
			});
			DbContext.SaveChanges();

			testable = new RolesManagementService(DbContext, roleManager);
		}

		[Test]
		public async ValueTask Get_Role_Test() {
			var role = await testable.Get("2");
			Assert.IsNotNull(role);
			Assert.That(role.Name, Is.EqualTo("role2"));
		}

		[Test]
		public void Get_For_Not_Exist_Key_Throws_ObjectNotFoundException() {
			Assert.ThrowsAsync<ObjectNotFoundException>(async () => await testable.Get("not_exists_role"));
		}

		[Test]
		public async ValueTask List_Test() {
			var users = await testable.List(new LoadDataArgs());
			Assert.IsNotNull(users);
			Assert.That(users.Count(), Is.EqualTo(3));
			Assert.That(users.ElementAt(0).Id, Is.EqualTo("1"));
			Assert.That(users.ElementAt(0).Name, Is.EqualTo("role1"));
		}
	}
}
