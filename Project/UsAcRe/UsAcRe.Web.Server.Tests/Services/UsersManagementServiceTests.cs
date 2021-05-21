using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Models;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Exceptions;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class UsersManagementServiceTests {
		Mock<IApplicationDbContext> applicationDbContextMock;
		UsersManagementService testable;

		[SetUp]
		public void Setup() {
			applicationDbContextMock = new Mock<IApplicationDbContext>();
			applicationDbContextMock
				.SetupGet(x => x.Users)
				.Returns(() => {
					var users = new FakeDbSet<ApplicationUser>((en, ks) => ks.Any(x => (string)x == en.Id)) {
						new ApplicationUser() {
							Id = "1",
							UserName = "test1"
						},
						new ApplicationUser {
							Id = "2",
							UserName = "test2"
						}
					};
					return users;
				});

			testable = new UsersManagementService(applicationDbContextMock.Object);
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
	}
}
