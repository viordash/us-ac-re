namespace UsAcRe.Web.Shared.Models {
	public class RoleModel {
		public System.Guid Id { get; set; }
		public string Name { get; set; }

		public override string ToString() {
			return $"{Name}";
		}
	}
}
