﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsAcRe.Web.Server.Identity;

namespace UsAcRe.Web.Server.Data {
	public class ApplicationIdentityDbContext<TUser, TRole> : IdentityDbContext<TUser, TRole, System.Guid, IdentityUserClaim<System.Guid>,
		ApplicationIdentityUserRole, ApplicationIdentityUserLogin, IdentityRoleClaim<System.Guid>, ApplicationIdentityUserToken>
	where TUser : ApplicationIdentityUser
	where TRole : ApplicationIdentityRole {

		public ApplicationIdentityDbContext(DbContextOptions options) : base(options) {
		}
	}

}
