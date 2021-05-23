using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UsAcRe.Web.Server.Identity {
	public class ApplicationIdentityUserToken : IdentityUserToken<string> {
		[MaxLength(256)]
		public override string UserId { get; set; }
		[MaxLength(256)]
		public override string LoginProvider { get; set; }
		[MaxLength(256)]
		public override string Name { get; set; }
	}
}
