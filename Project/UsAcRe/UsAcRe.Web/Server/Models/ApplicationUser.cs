using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UsAcRe.Web.Server.Models {
	public class ApplicationUser : IdentityUser<string> {
		[MaxLength(100)]
		public override string Id { get; set; }
	}
}
