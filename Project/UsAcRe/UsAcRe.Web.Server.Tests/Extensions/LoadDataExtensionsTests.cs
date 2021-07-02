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
	public class LoadDataExtensionsTests : DbContextFixture {

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
		public async ValueTask PerformLoadPagedData_Offset_Test() {
			var dataPaging = new DataPaging() {
				Top = 4,
				Skip = 3
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(4));
			Assert.That(users[0].Id, Is.EqualTo(guids[3]));
			Assert.That(users[0].UserName, Is.EqualTo("test3"));
		}

		[Test]
		public async ValueTask PerformLoadPagedData_OrderBy_Test() {
			var dataPaging = new DataPaging() {
				Top = 10,
				Skip = 0,
				Sorts = new List<Shared.Models.SortDescriptor>() { new Shared.Models.SortDescriptor() { Field = "email", SortOrder = Shared.Models.SortOrder.Descending } }
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[9]));
			Assert.That(users[0].Email, Is.EqualTo("email9"));
			Assert.That(users[9].Id, Is.EqualTo(guids[0]));
			Assert.That(users[9].Email, Is.EqualTo("email0"));

			const string ignoreCaseOrderBy = "USERName";
			dataPaging.Sorts = new List<Shared.Models.SortDescriptor>() { new Shared.Models.SortDescriptor() { Field = ignoreCaseOrderBy, SortOrder = Shared.Models.SortOrder.Ascending } };

			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}


		[Test]
		public async ValueTask PerformLoadPagedData_For_Default_LoadDataArgs_Test() {
			var dataPaging = new DataPaging();

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(10));
			Assert.That(users[0].Id, Is.EqualTo(guids[0]));
			Assert.That(users[0].UserName, Is.EqualTo("test0"));
			Assert.That(users[9].Id, Is.EqualTo(guids[9]));
			Assert.That(users[9].UserName, Is.EqualTo("test9"));
		}

		#region Filtering
		[Test]
		public async ValueTask ApplyFilter_Single_Filter_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = "Id",
						FilterOperator = Shared.Models.FilterOperator.Equals,
						FilterValue = guids[2]
					}
				},
				Top = 10,
				Skip = 0
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(1));
			Assert.That(users[0].Id, Is.EqualTo(guids[2]));
			Assert.That(users[0].UserName, Is.EqualTo("test2"));

			dataPaging.Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = "ID",
						FilterOperator = Shared.Models.FilterOperator.NotEquals,
						FilterValue = guids[4]
					}
				};
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(9));
			Assert.That(users.FirstOrDefault(x => x.UserName == "test4"), Is.Null);
		}

		[Test]
		public async ValueTask ApplyFilter_Multiple_Filters_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = "UserName",
						FilterOperator = Shared.Models.FilterOperator.Contains,
						FilterValue = "test",
						LogicalFilterOperator = Shared.Models.LogicalFilterOperator.And,
						SecondFilterOperator = Shared.Models.FilterOperator.NotEquals,
						SecondFilterValue = "test5"
					},
					new Shared.Models.FilterDescriptor() {
						Field = "Email",
						FilterOperator = Shared.Models.FilterOperator.NotEquals,
						FilterValue = "email6"
					}
				},
				Top = 10,
				Skip = 0
			};

			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users, Has.Count.EqualTo(8));
			Assert.That(users.FirstOrDefault(x => x.UserName == "test5"), Is.Null);
			Assert.That(users.FirstOrDefault(x => x.UserName == "test6"), Is.Null);
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_Equals_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.Equals, FilterValue = "test7" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test7" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_NotEquals_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.NotEquals, FilterValue = "test5" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3", "test4", "test6", "test7", "test8", "test9" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_LessThan_LessThanOrEquals_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.LessThan, FilterValue = "test4" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3" }));
			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.LessThanOrEquals;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3", "test4" }));

			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.LessThan;
			dataPaging.Filters.ElementAt(0).Field = "AccessFailedCount";
			dataPaging.Filters.ElementAt(0).FilterValue = 2;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1" }));

			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.LessThanOrEquals;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1", "test2" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_GreaterThan_GreaterThanOrEquals_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.GreaterThan, FilterValue = "test7" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test8", "test9" }));
			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.GreaterThanOrEquals;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test7", "test8", "test9" }));

			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.GreaterThan;
			dataPaging.Filters.ElementAt(0).Field = "AccessFailedCount";
			dataPaging.Filters.ElementAt(0).FilterValue = 5;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test6", "test7", "test8", "test9" }));

			dataPaging.Filters.ElementAt(0).FilterOperator = Shared.Models.FilterOperator.GreaterThanOrEquals;
			users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test5", "test6", "test7", "test8", "test9" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_Contains_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.Contains, FilterValue = "t7" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test7" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_StartsWith_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.StartsWith, FilterValue = "tes" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3", "test4", "test5", "test6", "test7", "test8", "test9" }));
		}

		[Test]
		public async ValueTask ApplyFilter_Filter_EndsWith_Test() {
			var dataPaging = new DataPaging() {
				Filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() { Field = "UserName", FilterOperator = Shared.Models.FilterOperator.EndsWith, FilterValue = "st5" }
				},
				Top = 10,
				Skip = 0
			};
			var users = await DbContext.Users.PerformLoadPagedData(dataPaging);
			Assert.That(users.Select(x => x.UserName), Is.EquivalentTo(new[] { "test5" }));
		}

		#endregion
	}
}
