using System;
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
		class TestFilterModel {
			public int IntField { get; set; }
			public float FloatField { get; set; }
			public string StringField { get; set; }
		}

		List<UserModel> usersList;
		List<TestFilterModel> testFilterList;

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

			usersList = Enumerable.Repeat(System.Guid.NewGuid(), 5)
				.Select((u, ui) => new UserModel() {
					Id = u,
					UserName = $"user{ui}",
					Email = $"email{ui}",
					Roles = Enumerable.Repeat(System.Guid.NewGuid(), 3)
						.Select((r, ri) => new RoleModel() { Id = r, Name = ui < 4 ? $"role_r{ri}" : $"role_u{ui}_r{ri}" })
						.ToList()
				})
				.ToList();


			testFilterList = Enumerable.Repeat("test", 5)
				.Select((u, ui) => new TestFilterModel() {
					IntField = ui,
					FloatField = (float)ui,
					StringField = $"{u}{ui}",
				})
				.ToList();
		}

		[Test]
		public void Enumerable_ApplyFilter_Equals_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.Equals,
						FilterValue = "role_r0, role_r1, role_r2"
					}
				};
			var filtered = usersList.ApplyFilter(filters);
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
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0", "user1" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "email0"
				});
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "incorrect"
				});
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered, Is.Empty);
		}

		[Test]
		public void Enumerable_ApplyFilter_NotEquals_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.NotEquals,
						FilterValue = "role_r0, role_r1, role_r2"
					}
				};
			var filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user4" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_u4_r0", "role_u4_r1", "role_u4_r2" }));

			filters = new List<Shared.Models.FilterDescriptor>() {
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.NotEquals,
					FilterValue = "email4",
					LogicalFilterOperator = Shared.Models.LogicalFilterOperator.And,
					SecondFilterOperator = Shared.Models.FilterOperator.Equals,
					SecondFilterValue = "email1",
				}
			};
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user1" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.Email),
					FilterOperator = Shared.Models.FilterOperator.Equals,
					FilterValue = "email0"
				});
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered, Is.Empty);
		}

		[Test]
		public void Enumerable_ApplyFilter_LessThan_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = nameof(TestFilterModel.IntField),
						FilterOperator = Shared.Models.FilterOperator.LessThan,
						FilterValue = 4
					}
				};
			var filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(TestFilterModel.FloatField),
					FilterOperator = Shared.Models.FilterOperator.LessThan,
					FilterValue = 2
				});

			filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test0", "test1" }));
		}

		[Test]
		public void Enumerable_ApplyFilter_LessThanOrEquals_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = nameof(TestFilterModel.IntField),
						FilterOperator = Shared.Models.FilterOperator.LessThanOrEquals,
						FilterValue = 4
					}
				};
			var filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test0", "test1", "test2", "test3", "test4" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(TestFilterModel.FloatField),
					FilterOperator = Shared.Models.FilterOperator.LessThanOrEquals,
					FilterValue = 2
				});

			filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test0", "test1", "test2" }));
		}

		[Test]
		public void Enumerable_ApplyFilter_GreaterThan_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = nameof(TestFilterModel.IntField),
						FilterOperator = Shared.Models.FilterOperator.GreaterThan,
						FilterValue = 2
					}
				};
			var filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test3", "test4" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(TestFilterModel.FloatField),
					FilterOperator = Shared.Models.FilterOperator.GreaterThan,
					FilterValue = 3
				});

			filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test4" }));
		}

		[Test]
		public void Enumerable_ApplyFilter_GreaterThanOrEquals_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = nameof(TestFilterModel.IntField),
						FilterOperator = Shared.Models.FilterOperator.GreaterThanOrEquals,
						FilterValue = 2
					}
				};
			var filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test2", "test3", "test4" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(TestFilterModel.FloatField),
					FilterOperator = Shared.Models.FilterOperator.GreaterThanOrEquals,
					FilterValue = 3
				});

			filtered = testFilterList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.StringField), Is.EquivalentTo(new[] { "test3", "test4" }));
		}

		[Test]
		public void Enumerable_ApplyFilter_Contains_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.Contains,
						FilterValue = "role_r1"
					}
				};
			var filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0", "user1", "user2", "user3" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_r0", "role_r1", "role_r2" }));

			filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.Contains,
						FilterValue = "role_r1",
						LogicalFilterOperator = Shared.Models.LogicalFilterOperator.Or,
						SecondFilterOperator = Shared.Models.FilterOperator.Contains,
						SecondFilterValue = "role_u4_r1"
					}
				};
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user0", "user1", "user2", "user3", "user4" }));

			filters.Add(
				new Shared.Models.FilterDescriptor() {
					Field = nameof(UserModel.UserName),
					FilterOperator = Shared.Models.FilterOperator.Contains,
					FilterValue = "incorrect"
				});
			filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered, Is.Empty);
		}

		[Test]
		public void Enumerable_ApplyFilter_StartsWith_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = UserModel.RolesNamesField,
						FilterOperator = Shared.Models.FilterOperator.StartsWith,
						FilterValue = "role_u4_"
					}
				};
			var filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user4" }));
			Assert.That(filtered.SelectMany(x => x.Roles).Select(x => x.Name).Distinct(), Is.EquivalentTo(new[] { "role_u4_r0", "role_u4_r1", "role_u4_r2" }));
		}

		[Test]
		public void Enumerable_ApplyFilter_EndsWith_Test() {
			var filters = new List<Shared.Models.FilterDescriptor>() {
					new Shared.Models.FilterDescriptor() {
						Field = nameof(UserModel.UserName),
						FilterOperator = Shared.Models.FilterOperator.EndsWith,
						FilterValue = "er2"
					}
				};
			var filtered = usersList.ApplyFilter(filters);
			Assert.That(filtered.Select(x => x.UserName), Is.EquivalentTo(new[] { "user2" }));
		}

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
	}
}
