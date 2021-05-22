using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsAcRe.Web.Server.Models;

namespace UsAcRe.Web.Server.Data {
	public interface IApplicationDbContext {
		DbSet<ApplicationUser> Users { get; set; }
		DbSet<ApplicationIdentityRole> Roles { get; set; }
		DbSet<ApplicationIdentityUserRole> UserRoles { get; set; }
	}

	public class ApplicationDbContext : ApplicationAuthorizationDbContext, IApplicationDbContext {
		public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
			: base(options, operationalStoreOptions) {
		}

	}
}
