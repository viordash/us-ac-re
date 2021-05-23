namespace UsAcRe.Web.Shared.Models {
	public class RoleModel {
		public string Id { get; set; }
		public string Name { get; set; }

		public override string ToString() {
			return $"{Name}";
		}
	}
}
