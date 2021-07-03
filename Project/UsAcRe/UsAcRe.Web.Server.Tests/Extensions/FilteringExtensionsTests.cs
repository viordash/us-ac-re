using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Radzen;
using Tests.Common;
using UsAcRe.Web.Server.Extensions;
using UsAcRe.Web.Server.Identity;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Tests.ServicesTests {
	[TestFixture]
	public class FilteringExtensionsTests : DbContextFixture {

		public override void SetUp() {
			base.SetUp();
			for(int i = 0; i < guids.Length; i++) {
				var id = guids[i];
				DbContext.Users.Add(new ApplicationIdentityUser() {
					Id = id,
					UserName = $"test{i}",
					Email = $"email{i}",
					AccessFailedCount = i
				});
			}
			DbContext.SaveChanges();
		}


		[Test]
		public void ApplyRolesFilter_Filter_Equals_Test() {
			var users = Enumerable.Repeat(System.Guid.NewGuid(), 5)
				.Select((u, ui) => new UserModel() {
					Id = u,
					UserName = $"user{ui}",
					Email = $"email{ui}",
					Roles = Enumerable.Repeat(System.Guid.NewGuid(), 3)
						.Select((r, ri) => new RoleModel() { Id = r, Name = ui < 4 ? $"role_r{ri}" : $"role_u{ui}_r{ri}" })
						.ToList()
				})
				.ToList();

			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.Equals,
						FilterValue = "role_r0, role_r1, role_r2"
					}
				};
			var filtered = users.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0", "user1", "user2", "user3" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "email0",
					LogicalFilterOperator = Shared.Models.LogicalFilterOperator.Or,
					SecondFilterOperator = Shared.Models.FilterOperator.Equals,
					SecondFilterValue = "email1",
				});
			filtered = users.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0", "user1" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "email0"
				});
			filtered = users.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "incorrect"
				});
			filtered = users.ApplyFilter(filters);
			Assert.That(filtered, Is.Empty);
		}

	}
}
