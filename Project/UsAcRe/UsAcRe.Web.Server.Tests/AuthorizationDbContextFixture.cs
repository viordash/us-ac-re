using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Server.Identity;

namespace Tests.Common {
	public class AuthorizationDbContextFixture : DbContextFixture {
		protected Mock<IRoleValidator<ApplicationIdentityRole>> roleValidatorMock;
		protected Mock<ILookupNormalizer> keyNormalizerMock;
		protected Mock<IdentityErrorDescriber> identityErrorDescriberMock;
		protected RoleManager<ApplicationIdentityRole> roleManager;

		public override void SetUp() {
			base.SetUp();
			roleValidatorMock = new Mock<IRoleValidator<ApplicationIdentityRole>>();
			keyNormalizerMock = new Mock<ILookupNormalizer>();
			identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

			var roleStore = new RoleStore<ApplicationIdentityRole, ApplicationDbContext, System.Guid>(DbContext);

			var roleValidators = new List<IRoleValidator<ApplicationIdentityRole>>() { roleValidatorMock.Object };
			var loggerMock = new Mock<ILogger<RoleManager<ApplicationIdentityRole>>>();

			roleManager = new RoleManager<ApplicationIdentityRole>(roleStore, roleValidators, keyNormalizerMock.Object, identityErrorDescriberMock.Object, loggerMock.Object);
		}
	}
}
