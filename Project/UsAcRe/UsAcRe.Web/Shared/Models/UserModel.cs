using System.Collections.Generic;
using System.Linq;

namespace UsAcRe.Web.Shared.Models {
	public class UserModel : ConcurrencyModel {

		public System.Guid Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public IEnumerable<RoleModel> Roles { get; set; }

		public override string ToString() {
			return $"{UserName} [{Email}]";
		}

		public static object MapField(UserModel model, string fieldName) =>
			fieldName switch {
				nameof(Id) => model.Id,
				nameof(UserName) => model.UserName,
				nameof(Email) => model.Email,
				nameof(Roles) => UserRolesView.Concat(model),
				_ => null,
			};
	}


	public static class UserRolesView {
		public static string Concat(UserModel user) {
			return string.Join(", ", user.Roles.Select(r => r.Name));
		}

	}
}
