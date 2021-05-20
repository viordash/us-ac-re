namespace UsAcRe.Web.Shared.Models {
	public class UserModel {
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string RoleNames { get; set; }

		public override string ToString() {
			return $"{UserName} [{Email}]";
		}
	}
}
