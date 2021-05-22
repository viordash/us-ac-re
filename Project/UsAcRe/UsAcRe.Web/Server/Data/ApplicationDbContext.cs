using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationDbContext : ApplicationAuthorizationDbContext {
		public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options, operationalStoreOptions) {
		}

	}
}
