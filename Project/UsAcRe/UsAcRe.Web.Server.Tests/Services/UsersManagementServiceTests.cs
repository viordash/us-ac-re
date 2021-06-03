using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Exceptions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Exceptions;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class UsersManagementServiceTests : AuthorizationDbContextFixture {
		UsersManagementService testable;

		public override void SetUp() {
			base.SetUp();

			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[1],
				UserName = "test1",
				NormalizedUserName = "test1".ToUpper()
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[2],
				UserName = "test2",
				NormalizedUserName = "test2".ToUpper()
			});
			DbContext.Users.Add(new ApplicationUser() {
				Id = guids[5],
				UserName = "test22",
				NormalizedUserName = "test22".ToUpper()
			});

			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = guids[7],
				Name = "role1",
				NormalizedName = "role1".ToUpper()
			});
			DbContext.Roles.Add(new ApplicationIdentityRole() {
				Id = guids[8],
				Name = "role2",
				NormalizedName = "role2".ToUpper()
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

			userStoreMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationUser, CancellationToken>((user, ct) => {
					return Task.FromResult(IdentityResult.Success);
				});
			testable = new UsersManagementService(DbContext, userManager);
		}

		[Test]
		public async ValueTask Get_User_Test() {
			var user = await testable.Get(guids[2]);
			Assert.IsNotNull(user);
			Assert.That(user.UserName, Is.EqualTo("test2"));
		}

		[Test]
		public void Get_For_Not_Exist_User_Throws_ObjectNotFoundException() {
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
			Assert.That(users.ElementAt(1).Roles, Is.Empty);
			Assert.That(users.ElementAt(2).Roles, Is.Empty);
		}

		[Test]
		public async ValueTask Edit_User_Test() {
			await testable.Edit(new UserModel() { Id = guids[1], UserName = "test1_edit", Email = "test1@ttt.tt" });
			userStoreMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()));
		}

		[Test]
		public void Edit_For_Not_Exist_User_Throws_ObjectNotFoundException() {
			Assert.ThrowsAsync<ObjectNotFoundException>(async () => await testable.Edit(new UserModel() { Id = System.Guid.NewGuid(), UserName = "test1_edit", Email = "test1@ttt.tt" }));
		}

		[Test]
		public async ValueTask Edit_User_Roles_Test() {
			await testable.Edit(new UserModel() { Id = guids[1], UserName = "test1_edit", Roles = new List<string>() { "role1" } });
			userStoreMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()));
			userStoreMock.Verify(x => x.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
			userStoreMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

			var users = await testable.List(new LoadDataArgs());
			Assert.That(users.ElementAt(0).Roles, Is.EquivalentTo(new[] { "role1" }));
		}

		[Test]
		public async ValueTask Edit_User__Remove_All_Roles_Test() {
			await testable.Edit(new UserModel() { Id = guids[1], UserName = "test1_edit" });
			var users = await testable.List(new LoadDataArgs());
			Assert.That(users.ElementAt(0).Roles, Is.Empty);
		}

		[Test]
		public void Edit_User_With_Non_Existen_Role_Throws_IdentityErrorException() {
			userStoreMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
			.Returns<ApplicationUser, CancellationToken>((user, ct) => {
				var roleIds = DbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId);
				return Task.FromResult(new List<string>() { "non-existen role" } as IList<string>);
			});
			Assert.ThrowsAsync<IdentityErrorException>(async () => await testable.Edit(new UserModel() { Id = guids[1], UserName = "test1_edit", Roles = new List<string>() { "role1" } }));
		}
	}
}
