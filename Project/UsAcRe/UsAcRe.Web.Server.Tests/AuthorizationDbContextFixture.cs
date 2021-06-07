using System.Collections.Generic;
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
		#region inner classes 
		public interface ITestUserStore : IUserStore<ApplicationIdentityUser>, IUserRoleStore<ApplicationIdentityUser> {
		}
		#endregion

		protected Mock<IRoleValidator<ApplicationIdentityRole>> roleValidatorMock;
		protected Mock<IdentityErrorDescriber> identityErrorDescriberMock;
		protected RoleManager<ApplicationIdentityRole> roleManager;

		protected Mock<IUserValidator<ApplicationIdentityUser>> userValidatorMock;
		protected Mock<IPasswordValidator<ApplicationIdentityUser>> passwordValidatorMock;
		protected Mock<ITestUserStore> userStoreMock;
		protected UserManager<ApplicationIdentityUser> userManager;

		public override void SetUp() {
			base.SetUp();
			roleValidatorMock = new Mock<IRoleValidator<ApplicationIdentityRole>>();
			identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();
			userValidatorMock = new Mock<IUserValidator<ApplicationIdentityUser>>();
			passwordValidatorMock = new Mock<IPasswordValidator<ApplicationIdentityUser>>();
			userStoreMock = new Mock<ITestUserStore>();

			var roleStore = new RoleStore<ApplicationIdentityRole, ApplicationDbContext, System.Guid>(DbContext);
			var roleValidators = new List<IRoleValidator<ApplicationIdentityRole>>() { roleValidatorMock.Object };
			var loggerRoleManagerMock = new Mock<ILogger<RoleManager<ApplicationIdentityRole>>>();

			roleManager = new RoleManager<ApplicationIdentityRole>(roleStore, roleValidators, null, identityErrorDescriberMock.Object, loggerRoleManagerMock.Object);

			var optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
			var passwordHasherMock = new Mock<IPasswordHasher<ApplicationIdentityUser>>();
			var userValidators = new List<IUserValidator<ApplicationIdentityUser>>() { userValidatorMock.Object };
			var passwordValidators = new List<IPasswordValidator<ApplicationIdentityUser>>() { passwordValidatorMock.Object };
			var loggerUserManagerMock = new Mock<ILogger<UserManager<ApplicationIdentityUser>>>();


			userValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<UserManager<ApplicationIdentityUser>>(), It.IsAny<ApplicationIdentityUser>()))
				.Returns(() => {
					return Task.FromResult(IdentityResult.Success);
				});

			userStoreMock.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<string, CancellationToken>((id, ct) => {
					return Task.FromResult(DbContext.Users.FirstOrDefault(x => x.Id.ToString() == id));
				});

			userStoreMock.Setup(x => x.IsInRoleAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, string, CancellationToken>((user, role, ct) => {
					var roleId = DbContext.Roles.FirstOrDefault(x => x.Name == role)?.Id;
					return Task.FromResult(DbContext.UserRoles.Any(x => x.RoleId == roleId && x.UserId == user.Id));
				});

			userStoreMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, string, CancellationToken>((user, role, ct) => {
					var roleId = DbContext.Roles.FirstOrDefault(x => x.Name == role).Id;
					DbContext.UserRoles.Add(new ApplicationIdentityUserRole() { RoleId = roleId, UserId = user.Id });
					DbContext.SaveChanges();
					return Task.CompletedTask;
				});

			userStoreMock.Setup(x => x.RemoveFromRoleAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, string, CancellationToken>((user, role, ct) => {
					var roleId = DbContext.Roles.FirstOrDefault(x => x.Name == role).Id;
					var userRole = DbContext.UserRoles.FirstOrDefault(x => x.RoleId == roleId && x.UserId == user.Id);
					DbContext.UserRoles.Remove(userRole);
					DbContext.SaveChanges();
					return Task.CompletedTask;
				});

			userStoreMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationIdentityUser>(), It.IsAny<CancellationToken>()))
				.Returns<ApplicationIdentityUser, CancellationToken>((user, ct) => {
					var roleIds = DbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId);
					return Task.FromResult(DbContext.Roles.Where(x => roleIds.Contains(x.Id)).Select(x => x.Name).ToList() as IList<string>);
				});

			userManager = new UserManager<ApplicationIdentityUser>(userStoreMock.Object, optionsAccessorMock.Object, passwordHasherMock.Object, userValidators, passwordValidators,
				null, identityErrorDescriberMock.Object, serviceProviderMock.Object, loggerUserManagerMock.Object);


		}
	}
}
