using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using Tests.Common;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class ApplicationRoleStoreTests : AuthorizationDbContextFixture {
		#region inner classes
		class PublicMorozov_ApplicationRoleStore : ApplicationRoleStore {
			public PublicMorozov_ApplicationRoleStore(ApplicationDbContext context, ILookupNormalizer keyNormalizer)
				: base(context, keyNormalizer) {
			}

			public ApplicationIdentityRoleClaim PublicCreateRoleClaim(ApplicationIdentityRole role, Claim claim) {
				return CreateRoleClaim(role, claim);
			}
		}
		#endregion

		PublicMorozov_ApplicationRoleStore testable;

		public override void SetUp() {
			base.SetUp();
			testable = new PublicMorozov_ApplicationRoleStore(DbContext, keyNormalizerMock.Object);
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

		[Test]
		public void UpdateAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.UpdateAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", "")));
		}

		[Test]
		public void DeleteAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.DeleteAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", "")));
		}

		[Test]
		public void SetRoleNameAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.SetRoleNameAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", ""), "role"));
		}

		[Test]
		public void AddClaimAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.AddClaimAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", ""),
				new Claim("type", "value")));
		}

		[Test]
		public void RemoveClaimAsync_Throws_NotSupportedException() {
			Assert.ThrowsAsync<NotSupportedException>(async () => await testable.RemoveClaimAsync(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", ""),
				new Claim("type", "value")));
		}

		[Test]
		public void CreateRoleClaim_Throws_NotSupportedException() {
			Assert.Throws<NotSupportedException>(() => testable.PublicCreateRoleClaim(new ApplicationIdentityRole(ApplicationRoleType.Administrator, "", ""),
				new Claim("type", "value")));
		}
	}
}
