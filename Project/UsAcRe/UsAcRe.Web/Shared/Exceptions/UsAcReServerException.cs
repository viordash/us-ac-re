using System;

namespace UsAcRe.Web.Shared.Exceptions {
	public class UsAcReServerException : Exception {
		public UsAcReServerException(string message)
			: base(message) {
		}
	}
}
