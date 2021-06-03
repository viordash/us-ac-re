﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Identity;

namespace Tests.Common {
	public class AuthorizationDbContextFixture : DbContextFixture {
		protected Mock<IRoleValidator<ApplicationIdentityRole>> roleValidatorMock;
		protected Mock<ILookupNormalizer> keyNormalizerMock;
		protected Mock<IdentityErrorDescriber> identityErrorDescriberMock;
		protected RoleManager<ApplicationIdentityRole> roleManager;

		protected Mock<IUserValidator<ApplicationUser>> userValidatorMock;
		protected Mock<IPasswordValidator<ApplicationUser>> passwordValidatorMock;
		protected Mock<IUserStore<ApplicationUser>> userStoreMock;
		protected UserManager<ApplicationUser> userManager;

		public override void SetUp() {
			base.SetUp();
			roleValidatorMock = new Mock<IRoleValidator<ApplicationIdentityRole>>();
			keyNormalizerMock = new Mock<ILookupNormalizer>();
			identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();
			userValidatorMock = new Mock<IUserValidator<ApplicationUser>>();
			passwordValidatorMock = new Mock<IPasswordValidator<ApplicationUser>>();
			userStoreMock = new Mock<IUserStore<ApplicationUser>>();

			var roleStore = new RoleStore<ApplicationIdentityRole, ApplicationDbContext, System.Guid>(DbContext);
			var roleValidators = new List<IRoleValidator<ApplicationIdentityRole>>() { roleValidatorMock.Object };
			var loggerRoleManagerMock = new Mock<ILogger<RoleManager<ApplicationIdentityRole>>>();

			roleManager = new RoleManager<ApplicationIdentityRole>(roleStore, roleValidators, keyNormalizerMock.Object, identityErrorDescriberMock.Object, loggerRoleManagerMock.Object);

			var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
			var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
			var userValidators = new List<IUserValidator<ApplicationUser>>() { userValidatorMock.Object };
			var passwordValidators = new List<IPasswordValidator<ApplicationUser>>() { passwordValidatorMock.Object };
			var loggerUserManagerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

			userValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<UserManager<ApplicationUser>>(), It.IsAny<ApplicationUser>()))
				.Returns(() => {
					return Task.FromResult(IdentityResult.Success);
				});

			userStoreMock.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<string, CancellationToken>((id, ct) => {
					return Task.FromResult(DbContext.Users.FirstOrDefault(x => x.Id.ToString() == id));
				});

			userManager = new UserManager<ApplicationUser>(userStoreMock.Object, optionsAccessorMock.Object, passwordHasherMock.Object, userValidators, passwordValidators,
				keyNormalizerMock.Object, identityErrorDescriberMock.Object, serviceProviderMock.Object, loggerUserManagerMock.Object);

		}
	}
}
