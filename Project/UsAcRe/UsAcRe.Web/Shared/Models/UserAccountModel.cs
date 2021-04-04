namespace UsAcRe.Web.Shared.Models {
	public class UserAccountModel {
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public int AccessFailedCount { get; set; }
		public string Role { get; set; }
	}
}
