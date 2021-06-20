using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
			DbContext.Users.Add(new ApplicationIdentityUser() {
				Id = guids[5],
				UserName = "test22",
				NormalizedUserName = "test22".ToUpper()
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

			userStoreMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, CancellationToken>((user, ct) => {
					var appUser = DbContext.Users.FirstOrDefault(x => x.Id == user.Id);
					DbContext.Users.Update(appUser);
					DbContext.SaveChanges();
					return Task.FromResult(IdentityResult.Success);
				});

			userStoreMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, CancellationToken>((user, ct) => {
					if(DbContext.Users.Any(x => x.UserName == user.UserName)) {
						return Task.FromResult(IdentityResult.Failed(new IdentityError()));
					}
					DbContext.Users.Add(user);
					DbContext.SaveChanges();
					return Task.FromResult(IdentityResult.Success);
				});
			testable = new UsersManagementService(DbContext, userManager, roleManager);
		}

		#region Get
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
		#endregion

		#region List
		[Test]
		public async ValueTask List_Test() {
			var users = await testable.List(new LoadDataArgs());
			Assert.IsNotNull(users);
			Assert.That(users.Count(), Is.EqualTo(3));
			Assert.That(users.ElementAt(0).Id, Is.EqualTo(guids[1]));
			Assert.That(users.ElementAt(0).UserName, Is.EqualTo("test1"));
			Assert.That(users.ElementAt(0).Roles, Is.EquivalentTo(new[] { "SuperUser", "Administrator" }));
			Assert.That(users.ElementAt(1).Roles, Is.Empty);
			Assert.That(users.ElementAt(2).Roles, Is.Empty);
		}
		#endregion

		#region Edit
		[Test]
		public async ValueTask Edit_User_Test() {
			var appUser = await DbContext.Users.FindAsync(guids[1]);
			await testable.Edit(new UserModel() { Id = guids[1], UserName = "test1_edit", Email = "test1@ttt.tt", ConcurrencyStamp = appUser.ConcurrencyStamp });
			userStoreMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()));
		}

		[Test]
		public void Edit_User_With_Concurrency_Old_Throws_DbUpdateConcurrencyException_Test() {
			Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await testable.Edit(new UserModel() {
				Id = guids[1],
				UserName = "test1_edit",
				Email = "test1@ttt.tt",
				ConcurrencyStamp = "some old ConcurrencyStamp value"
			}));
			userStoreMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()));
		}

		[Test]
		public async ValueTask Edit_For_Not_Exist_User_Throws_ObjectNotFoundException() {
			var appUser = await DbContext.Users.FindAsync(guids[1]);
			Assert.ThrowsAsync<ObjectNotFoundException>(async () => await testable.Edit(new UserModel() {
				Id = System.Guid.NewGuid(),
				UserName = "test1_edit",
				Email = "test1@ttt.tt",
				ConcurrencyStamp = appUser.ConcurrencyStamp
			}));
		}

		[Test]
		public async ValueTask Edit_User_Roles_Test() {
			var appUser = await DbContext.Users.FindAsync(guids[1]);
			await testable.Edit(new UserModel() {
				Id = guids[1],
				UserName = "test1_edit",
				Roles = new List<string>() { "SuperUser" },
				ConcurrencyStamp = appUser.ConcurrencyStamp
			});
			userStoreMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()));
			userStoreMock.Verify(x => x.RemoveFromRoleAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
			userStoreMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

			var users = await testable.List(new LoadDataArgs());
			Assert.That(users.ElementAt(0).Roles, Is.EquivalentTo(new[] { "SuperUser" }));
		}

		[Test]
		public async ValueTask Edit_User__Remove_All_Roles_Test() {
			var appUser = await DbContext.Users.FindAsync(guids[1]);
			await testable.Edit(new UserModel() {
				Id = guids[1],
				UserName = "test1_edit",
				ConcurrencyStamp = appUser.ConcurrencyStamp
			});
			var users = await testable.List(new LoadDataArgs());
			Assert.That(users.ElementAt(0).Roles, Is.Empty);
		}

		[Test]
		public async ValueTask Edit_User_With_Non_Existen_Role_Throws_IdentityErrorException() {
			var appUser = await DbContext.Users.FindAsync(guids[1]);
			userStoreMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()))
			.Returns<ApplicationIdentityUser, CancellationToken>((user, ct) => {
				return Task.FromResult(new List<string>() { "non-existen role" } as IList<string>);
			});
			Assert.ThrowsAsync<IdentityErrorException>(async () => await testable.Edit(new UserModel() {
				Id = guids[1],
				UserName = "test1_edit",
				Roles = new List<string>() { "SuperUser" },
				ConcurrencyStamp = appUser.ConcurrencyStamp
			}));
		}
		#endregion

		#region Create
		[Test]
		public async ValueTask Create_User_Test() {
			await testable.Create(new UserModel() { UserName = "new_test", Email = "new@ttt.tt", Roles = new List<string>() { "SuperUser" } });
			userStoreMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()));

			var users = await testable.List(new LoadDataArgs());
			Assert.That(users.Last().UserName, Is.EqualTo("new_test"));
			Assert.That(users.Last().Roles, Is.EquivalentTo(new[] { "SuperUser" }));
		}

		[Test]
		public void Create_User_That_Already_Exists_Throws_IdentityErrorException() {
			Assert.ThrowsAsync<IdentityErrorException>(async () => await testable.Create(new UserModel() { UserName = "test1" }));
		}
		#endregion
	}
}
