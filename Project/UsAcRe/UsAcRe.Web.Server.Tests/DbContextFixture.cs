﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UsAcRe.Web.Server.Data;

namespace Tests.Common {
	public class DbContextFixture : ServiceProviderFixture {
		protected static System.Guid[] guids = Enumerable.Range(0, 10).Select(x => System.Guid.NewGuid()).ToArray();
		protected ApplicationDbContext DbContext { get; private set; }

		public override void SetUp() {
			base.SetUp();
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "Application Test")
				.Options;
			var operationalStoreOptions = Microsoft.Extensions.Options.Options.Create(new IdentityServer4.EntityFramework.Options.OperationalStoreOptions());

			DbContext = new ApplicationDbContext(options, operationalStoreOptions);
			DbContext.Database.EnsureDeleted();
			DbContext.Database.EnsureCreated();
		}

		public override void TearDown() {
			base.TearDown();
			DbContext.Dispose();
		}


	}
}
