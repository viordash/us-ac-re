using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Tests.Common;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class ApplicationUserStoreTests : AuthorizationDbContextFixture {
		#region inner classes
		class PublicMorozov_ApplicationUserStore : ApplicationUserStore {
			public PublicMorozov_ApplicationUserStore(ApplicationDbContext context, ApplicationRoleStore roleStore)
				: base(context, roleStore) {
			}

			public Task<ApplicationIdentityRole> PublicFindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken) {
				return FindRoleAsync(normalizedRoleName, cancellationToken);
			}
		}
		#endregion
		PublicMorozov_ApplicationUserStore testable;

		public override void SetUp() {
			base.SetUp();

			DbContext.Users.Add(new ApplicationIdentityUser() {
				Id = guids[1],
				UserName = "test1",
				NormalizedUserName = "test1".ToUpper()
			});
			DbContext.Users.Add(new ApplicationIdentityUser() {
				Id = guids[2],
				UserName = "test2",
				NormalizedUserName = "test2".ToUpper()
			});

			DbContext.UserRoles.Add(new ApplicationIdentityUserRole() {
				UserId = guids[1],
				RoleId = roleManager.Roles.ElementAt(0).Id
			});

			DbContext.UserRoles.Add(new ApplicationIdentityUserRole() {
				UserId = guids[1],
				RoleId = roleManager.Roles.ElementAt(1).Id
			});

			DbContext.SaveChanges();

			testable = new PublicMorozov_ApplicationUserStore(DbContext, new ApplicationRoleStore(DbContext, keyNormalizerMock.Object));
		}

		[Test]
		public async ValueTask GetRolesAsync_Test() {
			var roles = await testable.GetRolesAsync(new ApplicationIdentityUser() { Id = guids[1] });
			Assert.IsNotNull(roles);
			Assert.That(roles, Is.EquivalentTo(new[] { "SuperUser", "Administrator" }));
		}

		[Test]
		public async ValueTask GetRolesAsync_For_User_Without_Roles_Test() {
			var roles = await testable.GetRolesAsync(new ApplicationIdentityUser() { Id = guids[2] });
			Assert.IsNotNull(roles);
			Assert.That(roles.Count, Is.EqualTo(0));
		}

		[Test]
		public void GetRolesAsync_Throws_OperationCanceledException() {
			Assert.ThrowsAsync<OperationCanceledException>(async () => await testable.GetRolesAsync(new ApplicationIdentityUser() { Id = guids[2] }, new CancellationToken(true)));
		}

		[Test]
		public void GetRolesAsync_Throws_ObjectDisposedException() {
			testable.Dispose();
			Assert.ThrowsAsync<ObjectDisposedException>(async () => await testable.GetRolesAsync(new ApplicationIdentityUser() { Id = guids[2] }));
		}

		[Test]
		public void GetRolesAsync_Throws_ArgumentNullException_For_Null_User() {
			Assert.ThrowsAsync<ArgumentNullException>(async () => await testable.GetRolesAsync(null));
		}

		[Test]
		public async ValueTask FindRoleAsync_Test() {
			var role = await testable.PublicFindRoleAsync("Administrator", default(CancellationToken));
			Assert.IsNotNull(role);
			Assert.That(role.Name, Is.EqualTo("Administrator"));
		}

		[Test]
		public async ValueTask FindRoleAsync_For_NotExists_Role_Return_Null_Test() {
			var role = await testable.PublicFindRoleAsync("any role", default(CancellationToken));
			Assert.IsNull(role);
		}

	}
}
