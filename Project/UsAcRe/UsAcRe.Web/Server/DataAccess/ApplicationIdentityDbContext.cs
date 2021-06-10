using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationIdentityDbContext : IdentityUserContext<ApplicationIdentityUser, System.Guid, ApplicationIdentityUserClaim,
		ApplicationIdentityUserLogin, ApplicationIdentityUserToken> {

		public ApplicationIdentityDbContext(DbContextOptions options) : base(options) {
		}

		public virtual DbSet<ApplicationIdentityUserRole> UserRoles { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<ApplicationIdentityUser>(b => {
				b.HasMany<ApplicationIdentityUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
			});

			builder.Entity<ApplicationIdentityUserRole>(b => {
				b.HasKey(r => new { r.UserId, r.RoleId });
				b.ToTable("AspNetUserRoles");
			});
		}

	}

}
