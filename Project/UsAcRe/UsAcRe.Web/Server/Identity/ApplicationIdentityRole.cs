using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UsAcRe.Web.Server.Identity {
	public enum ApplicationRoleType {
		Undef = 0,
		SuperUser,
		Administrator,
		User,
	}

	public static class ApplicationRoleTypeSpecifics {
		public static Dictionary<ApplicationRoleType, string> Names = new Dictionary<ApplicationRoleType, string>() {
			{ApplicationRoleType.SuperUser, "SuperUser" },
			{ ApplicationRoleType.Administrator, "Administrator"},
			{ ApplicationRoleType.User, "User"}
		};
	}

	public class ApplicationIdentityRole : IdentityRole<System.Guid> {
		public readonly ApplicationRoleType RoleType;

		public ApplicationIdentityRole() { }

		public ApplicationIdentityRole(ApplicationRoleType roleType, string name, string normalizedName) : this() {
			RoleType = roleType;
			Name = name;
			NormalizedName = normalizedName;
		}

		public override System.Guid Id {
			get => new System.Guid((int)RoleType, 0x3C2A, 0x400E, 0xAC, 0xAE, 0x50, 0x13, 0xA8, 0xC5, 0x1F, 0x30);
			set => throw new NotSupportedException();
		}
	}
}
