using System.Collections.Generic;

namespace UsAcRe.Web.Shared.Models {
	public class UserModel : ConcurrencyModel {
		public System.Guid Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public IEnumerable<string> Roles { get; set; }

		public override string ToString() {
			return $"{UserName} [{Email}]";
		}
	}
}
