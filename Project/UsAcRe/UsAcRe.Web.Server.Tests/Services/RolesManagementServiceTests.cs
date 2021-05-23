using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Models;
using UsAcRe.Web.Server.Services;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class RolesManagementServiceTests : DbContextFixture {
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

			testable = new RolesManagementService(DbContext);
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
