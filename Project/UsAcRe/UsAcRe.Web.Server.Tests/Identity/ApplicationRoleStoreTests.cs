using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Server.Services;
using UsAcRe.Web.Shared.Exceptions;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class ApplicationRoleStoreTests : AuthorizationDbContextFixture {
		ApplicationRoleStore testable;

		public override void SetUp() {
			base.SetUp();
			testable = new ApplicationRoleStore(DbContext, keyNormalizerMock.Object);
		}

		[Test]
		public void List_Roles_Test() {
			var roles = testable.Roles;
			Assert.IsNotNull(roles);
			Assert.That(roles.Count, Is.EqualTo(ApplicationRoleTypeSpecifics.Names.Count));
		}

		[Test]
		public void CreateAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.CreateAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", "")));
		}
	}
}
