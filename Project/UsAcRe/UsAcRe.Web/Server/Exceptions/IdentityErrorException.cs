using System.Linq;
using Microsoft.AspNetCore.Identity;
using UsAcRe.Web.Shared.Exceptions;

namespace UsAcRe.Web.Server.Exceptions {
	public class IdentityErrorException : UsAcReServerException {
		public IdentityErrorException(IdentityResult result)
			: base(string.Join(", ", result.Errors
				.Select(error => error.Description))) {
		}
	}
}
