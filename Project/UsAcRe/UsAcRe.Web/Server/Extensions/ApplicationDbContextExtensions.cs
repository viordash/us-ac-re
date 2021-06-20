using UsAcRe.Web.Server.Data;
using UsAcRe.Web.Shared.Models;

namespace UsAcRe.Web.Server.Extensions {
	public static class ApplicationDbContextExtensions {
		public static void SetConcurrencyStamp(this IConcurrencyModel concurrencyModel, ApplicationDbContext dbContext, string value) {
			var concurrencyStamp = dbContext.Entry(concurrencyModel).Property(e => e.ConcurrencyStamp);
			concurrencyStamp.OriginalValue = value;
			concurrencyStamp.IsModified = false;
		}
	}
}
