using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UsAcRe.Web.Server.Identity;

namespace Tests.Common {
	public class AuthorizationDbContextFixture : DbContextFixture {
		protected Mock<IRoleValidator<ApplicationIdentityRole>> roleValidatorMock;
		protected Mock<ILookupNormalizer> keyNormalizerMock;
		protected Mock<IdentityErrorDescriber> identityErrorDescriberMock;
		protected Mock<IRoleStore<ApplicationIdentityRole>> roleStoreMock;
		protected RoleManager<ApplicationIdentityRole> roleManager;

		public override void SetUp() {
			base.SetUp();
			roleValidatorMock = new Mock<IRoleValidator<ApplicationIdentityRole>>();
			keyNormalizerMock = new Mock<ILookupNormalizer>();
			identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();
			roleStoreMock = new Mock<IRoleStore<ApplicationIdentityRole>>();

			roleStoreMock
				.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns<string, CancellationToken>((roleId, cancellationToken) => {
					return DbContext.Roles.FindAsync(roleId);
				});


			var roleValidators = new List<IRoleValidator<ApplicationIdentityRole>>() { roleValidatorMock.Object };
			var loggerMock = new Mock<ILogger<RoleManager<ApplicationIdentityRole>>>();

			roleManager = new RoleManager<ApplicationIdentityRole>(roleStoreMock.Object, roleValidators, keyNormalizerMock.Object, identityErrorDescriberMock.Object, loggerMock.Object);
		}
	}
}
