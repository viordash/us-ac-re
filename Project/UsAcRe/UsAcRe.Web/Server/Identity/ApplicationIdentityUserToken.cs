using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationIdentityUserToken : IdentityUserToken<System.Guid> {
		[MaxLength(256)]
		public override string LoginProvider { get; set; }
		[MaxLength(256)]
		public override string Name { get; set; }
	}
}
