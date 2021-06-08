using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationIdentityDbContext : IdentityUserContext<ApplicationIdentityUser, System.Guid, ApplicationIdentityUserClaim,
		ApplicationIdentityUserLogin, ApplicationIdentityUserToken> {

		public ApplicationIdentityDbContext(DbContextOptions options) : base(options) {
		}

		public virtual DbSet<ApplicationIdentityUserRole> UserRoles { get; set; }
		public virtual DbSet<ApplicationIdentityRole> Roles { get; set; }
		public virtual DbSet<ApplicationIdentityRoleClaim> RoleClaims { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);
			builder.Entity<ApplicationIdentityUser>(b => {
				b.HasMany<ApplicationIdentityUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
			});

			builder.Entity<ApplicationIdentityRole>(b => {
				b.HasKey(r => r.Id);
				b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
				b.ToTable("AspNetRoles");
				b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

				b.Property(u => u.Name).HasMaxLength(256);
				b.Property(u => u.NormalizedName).HasMaxLength(256);

				b.HasMany<ApplicationIdentityUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
				b.HasMany<ApplicationIdentityRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
			});

			builder.Entity<ApplicationIdentityRoleClaim>(b => {
				b.HasKey(rc => rc.Id);
				b.ToTable("AspNetRoleClaims");
			});

			builder.Entity<ApplicationIdentityUserRole>(b => {
				b.HasKey(r => new { r.UserId, r.RoleId });
				b.ToTable("AspNetUserRoles");
			});
		}

	}

}
